using System;
using System.Collections.Generic;
using Godot;

public class Agent : Entity {
    Random random = new Random();
    private int _visionRange = 1;
    private int _deltaBeliefTreshold = 1000;
    
    private WorldState _beliefs = null;                 public WorldState Beliefs {get {return _beliefs;}}
    private List<Desire> _desires = new List<Desire>();
    private Desire _currentDesire = null;
    private List<Action> _intentions = new List<Action>();
    private List<Coord> _currentPath = new List<Coord>();
    public Entity _pocket = null;

    public Agent (string name, int x, int y) : base (name, x, y, false) {
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
        GD.Print("INIT AGENT");
        _beliefs = WorldState.DefaultBelief();
    }

    public void Tick () {
        GD.Print("TICK AGENT");
        //Percept
        int deltaBeliefs = _beliefs.AddPercept(WorldState.RealWorld.Percept(X, Y, _visionRange));
        deltaBeliefs += _beliefs.AddPercept(WorldState.RealWorld.Percept(6, 7, _visionRange));

        GD.Print(PrintBeliefs());

        //process desires
        if (_currentDesire == null || deltaBeliefs > _deltaBeliefTreshold) {
            ChooseDesire();
            _intentions.Clear();
        }

        //process intentions
        if (_intentions.Count == 0 || deltaBeliefs > _deltaBeliefTreshold) {
            _intentions = MakePlans(5, 5, 5, 2, .8f);
            _currentPath = _beliefs.PathFind(this, _intentions[0]);
        }

        //if we are walking, we walk 
        foreach(Action a in _intentions) {
            GD.Print("i will " + a);
        }
        
        
        if (_currentPath.Count > 0) {
            if (_beliefs.PathObstructed(_currentPath)) { //Check if the path is obstructed --> reevaluate desires / intentions
                    _currentPath = new List<Coord>();
                    _currentDesire = null;
                    _intentions = new List<Action>();

                    GD.Print("OBSTRUCTED");

            } else { //Walk
                Coord newCord = WorldState.RealWorld.Move(this, _currentPath[0]);
                GD.Print("WALK TO " + newCord.X + ", " + newCord.Y);
                _beliefs.Move(this, newCord);
                _currentPath.RemoveAt(0);
            }
        } else if (_intentions.Count > 0) {
            //do our action
            WorldState.RealWorld.Do(this, _intentions[0]);
            _intentions.RemoveAt(0);

            //Process a new path
            if (_intentions.Count > 0) {
                _beliefs.PathFind(this, _intentions[0]);
            }
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

    public List<Action> MakePlans (int maxCol, int maxTries, int maxMoves, float baseTemp, float alpha) {
        List<Action> bestPlan = null;
        float bestScore = float.MaxValue;

        for (int i = 0; i < maxTries; i++) {
            WorldState x = Beliefs.Clone();
            List<Action> plan = new List<Action>();

            float temperature = baseTemp;
            for (int j = 0; j < maxMoves; j++) {

                Action a;
                WorldState xNext;
                (xNext, a) = SimulatedAnhealingMove(x, maxCol, temperature);

                if (xNext != null) 
                    //TODO: Free x
                    x = xNext;
                    plan.Add(a);
                
                temperature *= alpha;
            }
            float xScore = _currentDesire.Score(x, this.Name);
            if (xScore < bestScore) {
                bestScore = xScore;
                bestPlan = plan;
            }
        }

        return bestPlan;
    }

    public (WorldState, Action) SimulatedAnhealingMove (WorldState currentWorldState, int maxCol, float temperature) {
        float bestScore = float.MaxValue;
        WorldState bestWorldState = currentWorldState;
        Action bestAction = new Action(null, null);
        bool accepted = false; //Accepted?(cur, x) : cost(x) ≤ cost(cur) or Random()< exp(−∆/T)
        
        List<Action> rawAvailableActions = currentWorldState.GetActions();
        List<Action> availableActions = new List<Action>();
        Dictionary<Action, int> availableActionsCost = new Dictionary<Action, int>();
        foreach(Action a in rawAvailableActions) {
            List<Coord> path = currentWorldState.PathFind(this, a);
            if (path != null) {
                availableActions.Add(a);
                availableActionsCost.Add(a, path.Count);
            }
        }



        if (availableActions.Count == 0) return (null, new Action(null, null));

        for (int i = 0; i < maxCol; i++) {
            Action randomAction = availableActions[random.Next(availableActions.Count)];
            WorldState newWorldState = currentWorldState.Clone();
            
            if (newWorldState.Do(this, randomAction) != null) {//Generate-Neighbor
                float newScore = _currentDesire.Score(newWorldState, this.Name);
                float currentScore = _currentDesire.Score(currentWorldState, this.Name);

                if (newScore <= currentScore || random.NextDouble() < Mathf.Exp(-(Mathf.Abs(newScore - currentScore)) / temperature)) {
                    accepted = true;
                }
                if (newScore < bestScore) {
                    //TODO, free bestWorldState
                    bestWorldState = newWorldState;
                    bestScore = newScore;
                    bestAction = randomAction;
                }
            } 
        }
        
        if (accepted) return (bestWorldState, bestAction);
        else return (currentWorldState, new Action(null, null));
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