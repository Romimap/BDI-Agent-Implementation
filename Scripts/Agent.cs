using System;
using System.Collections.Generic;
using Godot;

public class Agent : Entity {
    Random random = new Random();
    private int _visionRange = 1;                       public int VisionRange {get {return _visionRange;}}
    private int _deltaBeliefTreshold = 1000;
    private bool _generateIntentions = true;
    
    private WorldState _beliefs = null;                 public WorldState Beliefs {get {return _beliefs;}}
    private List<Desire> _desires = new List<Desire>();
    private Desire _currentDesire = null;
    private List<Action> _intentions = new List<Action>();
    private List<Coord> _currentPath = new List<Coord>();
    public Entity _pocket = null;

    public Agent (string name, int x, int y) : base (name, x, y, false) {
        _visuals = new VisualEntity(x, y, MapViewer.AGENT);
    }

    public Agent (Agent from) : base(from) {
        if (from._pocket != null) _pocket = _pocket.Clone();
        _beliefs = from._beliefs;
        _desires = from._desires;
        _currentDesire = from._currentDesire;
        _intentions = from._intentions;
        _currentPath = from._currentPath;
    }

    public void Init () {
        // GD.Print("INIT AGENT");
        _beliefs = WorldState.DefaultBelief();
    }

    public void Tick () {
        // GD.Print("TICK AGENT");
        //Percept
        int deltaBeliefs = _beliefs.AddPercept(WorldState.RealWorld.Percept(X, Y, _visionRange));
        deltaBeliefs += _beliefs.AddPercept(WorldState.RealWorld.Percept(2, 13, _visionRange));

        // GD.Print(PrintBeliefs());

        //Check if the path is obstructed --> reevaluate desires / intentions
        if (_currentPath.Count > 0 && _beliefs.PathObstructed(_currentPath)) { 
            _currentPath = new List<Coord>();
            _currentDesire = null;
            _generateIntentions = true;
        }

        //process desires
        if (_currentDesire == null || deltaBeliefs > _deltaBeliefTreshold) {
            ChooseDesire();
            _intentions.Clear();
        }

        //process intentions
        if (_intentions.Count == 0 || _generateIntentions || deltaBeliefs > _deltaBeliefTreshold) {
            _intentions = MakePlans(_intentions, 15, 4);
            _generateIntentions = false;
        }

        //Check if we need a path
        if (_currentPath.Count == 0 && _intentions.Count > 0) {
            _currentPath = _beliefs.PathFind(this, _intentions[0]);
            if (_currentPath.Count > 0) {
                _currentPath.RemoveAt(0);
            }
        }

        //Walk
        if (_currentPath.Count > 0) {
            Coord newCord = WorldState.RealWorld.Move(this, _currentPath[0]);
            _beliefs.Move(this, newCord);
            _currentPath.RemoveAt(0);
        } else if (_intentions.Count > 0) { //Or do our action
            WorldState.RealWorld.Do(this, _intentions[0]);
            _intentions.RemoveAt(0);
        }

        Main.richTextLabel.Text = "";
        foreach(Action a in _intentions) {
            Main.richTextLabel.Text += a + "\n";
        }
    }

    public void AddDesire (Desire d) {
        _desires.Add(d);
    }

    public void ChooseDesire () {
        float score = float.PositiveInfinity;
        foreach (Desire desire in _desires) {
            float desireScore = desire.PriorityScore(_beliefs, this.Name);
            if (desireScore < score) {
                _currentDesire = desire;
                score = desireScore;
            }
        }
    }

    public struct MakePlanActionStruct {
        public Action _action;
        public WorldState _worldState;
        public float _cost;

        public MakePlanActionStruct(Action action, WorldState worldState, float cost) {
            _action = action;
            _worldState = worldState;
            _cost = cost;
        }

        public void SetCost (float cost) {
            _cost = cost;
        }
    }

    public List<Action> MakePlans (List<Action> currentPlan, int maxTries, int maxMoves) {
        List<List<MakePlanActionStruct>> allRuns = new List<List<MakePlanActionStruct>>();
        //GD.Print("\n\n\n\n\nGENERATING INTENTIONS !");

        for (int i = 0; i < maxTries; i++) {
            //GD.Print("GENERATING PLAN !");
            List<MakePlanActionStruct> plan = new List<MakePlanActionStruct>();

            WorldState currentWorldState = _beliefs;

            for (int j = 0; j < maxMoves; j++) {
                Action a;
                WorldState xNext;
                float cost;
                (xNext, a, cost) = GenerateMove(currentWorldState);

                if (xNext == null) break;

                plan.Add(new MakePlanActionStruct(a, xNext, cost));
            }

            //Truncate the plan so it does not have doubles
            int current = 1;
            while (current < plan.Count) {
                if (plan[current - 1]._action.ToString().Equals(plan[current]._action.ToString())) {
                    plan.RemoveAt(current);
                } else {
                    current++;
                }
            }

            //Truncate the plan so it stops at the best position
            int currentJ = -1;
            float minimumScore = float.PositiveInfinity;
            for (int j = 0; j < plan.Count; j++) {
                float score = _currentDesire.Score(plan[j]._worldState, Name);
                if (score < minimumScore) {
                    currentJ = j;
                    minimumScore = score;
                }
            }
            while (plan.Count > currentJ + 1) {
                plan.RemoveAt(currentJ + 1);
            }

            //Bake the costs, here we add the cost of every action
            float totalCost = 0;
            for (int j = 0; j < plan.Count; j++) {
                totalCost += plan[j]._cost;
                plan[j].SetCost(totalCost);
            }

            //GD.Print("ONE PLAN IS TO : ");
            //foreach(MakePlanActionStruct makePlanAction in plan) {
            //    GD.Print(makePlanAction._action);
            //}

            allRuns.Add(plan);
        }

        //Get the best plan
        float minimumScoreCost = float.PositiveInfinity;
        int iMinimum = 0;
        for (int i = 0; i < allRuns.Count; i++) {
            int last = allRuns[i].Count - 1;
            float score = _currentDesire.Score(allRuns[i][last]._worldState, Name) * allRuns[i][last]._cost;
            //GD.Print("x               " + allRuns[i][last]._action + "(" + score + ")");

            if (score < minimumScoreCost) {
                iMinimum = i;
                minimumScoreCost = score;
            }
        }

        //Make it a list
        List<Action> bestPlan = new List<Action>();
        foreach(MakePlanActionStruct makePlanAction in allRuns[iMinimum]) {
            bestPlan.Add(makePlanAction._action);
        }

        //Generate a final worldstate for our current plan
        WorldState predicted = _beliefs.Clone();
        foreach(Action a in currentPlan) {
            predicted.Do(this, a);
            predicted = predicted.Clone();
        } 

        //If the new plan is better than the current plan
        if (_currentDesire.Score(predicted, Name) < _currentDesire.Score(allRuns[iMinimum][allRuns[iMinimum].Count - 1]._worldState, Name)) {
            return currentPlan;
        }
        //Else
        return bestPlan;
    }

    public (WorldState, Action, float) GenerateMove (WorldState currentWorldState) {
        List<Action> rawAvailableActions = currentWorldState.GetActions();
        List<Action> availableActions = new List<Action>();
        //GD.Print("  AVAILABLE : ");
        foreach(Action a in rawAvailableActions) {
            List<Coord> path = currentWorldState.PathFind(this, a);
            if (path != null) {
                //GD.Print("  " + a);
                availableActions.Add(a);
            }
        }

        if (availableActions.Count == 0) return (null, new Action(null, null), float.PositiveInfinity);

        WorldState next = currentWorldState.Clone();

        Action action = availableActions[random.Next(availableActions.Count)];
        //GD.Print("  choosed to do " + action);
        List<Coord> actionPath = next.Do(this, action);

        return (next, action, actionPath.Count);
    }

    public override Entity Clone () {
        return new Agent(this);
    }

    public string PrintBeliefs() {
        string ans = "AGENT BELIEFS: \n";

        ans += _beliefs.ToString();

        return ans;
    }
}