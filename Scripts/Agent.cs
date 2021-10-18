using System;
using System.Collections.Generic;
using Godot;

public class Agent : Entity {
    Random random = new Random();
    private int _visionRange = 1;
    private int _deltaBeliefTreshold = 3;
    
    private WorldState _beliefs;                    public WorldState Beliefs {get {return _beliefs;}}
    private List<Desire> _desires = new List<Desire>();
    private Desire _currentDesire = null;
    private List<Action> _intentions = new List<Action>();
    private List<Coord> _currentPath = new List<Coord>();

    public Agent (string name, int x, int y) : base (name, x, y, false) {
    }

    public Agent (Agent from) : base(from) {
    }

    public void Init () {
        _beliefs = WorldState.DefaultBelief();
    }

    public void Tick () {
        GD.Print("TICK AGENT");
        //Percept
        int deltaBeliefs = _beliefs.AddPercept(WorldState.RealWorld.Percept(X, Y, _visionRange));

        //process desires
        if (_currentDesire == null || deltaBeliefs > _deltaBeliefTreshold) {
            ChooseDesire();
            _intentions = new List<Action>();
        }

        //process intentions
        if (_intentions.Count == 0 || deltaBeliefs > _deltaBeliefTreshold) {
            //TODO
        }
        

        //if we are walking, we walk 
        if (_currentPath.Count > 0) {
            if (_beliefs.PathObstructed(_currentPath)) { //Check if the path is obstructed --> reevaluate desires / intentions
                    _currentPath = new List<Coord>();
                    _currentDesire = null;
                    _intentions = new List<Action>();

            } else { //Walk
                Coord newCord = WorldState.RealWorld.Move(this, _currentPath[0]);
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

    public void ChooseDesire () {
        float score = float.MaxValue;

        foreach (Desire desire in _desires) {
            float desireScore = desire.PriorityScore(this);
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
                //TODO: Free x
                x = xNext;
                if (x != null) 
                    plan.Add(a);
                
                temperature *= alpha;
            }
                    
            float xScore = _currentDesire.Score(x);
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
        List<Action> availableActions = currentWorldState.GetActions();

        for (int i = 0; i < maxCol; i++) {
            Action randomAction = availableActions[random.Next(availableActions.Count)];
            WorldState newWorldState = currentWorldState.Clone();
            
            if (newWorldState.Do(this, randomAction) != null) {//Generate-Neighbor
                float newScore = _currentDesire.Score(newWorldState);
                float currentScore = _currentDesire.Score(currentWorldState);

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