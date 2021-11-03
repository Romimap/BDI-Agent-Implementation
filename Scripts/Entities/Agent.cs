using System;
using System.Collections.Generic;
using Godot;

public class Agent : Entity {
	Random random = new Random();
	private int _visionRange = 1;                       public int VisionRange {get {return _visionRange;}}
	private int _deltaBeliefTreshold = 1000;
	private bool _generateIntentions = false;
	private bool _generateDesire = false;
	
	private WorldState _beliefs = null;                 public WorldState Beliefs {get {return _beliefs;}}
	private List<Desire> _desires = new List<Desire>();
	private Desire _currentDesire = null;
	private List<Action> _intentions = new List<Action>();
	private List<Coord> _currentPath = new List<Coord>();
	public Entity _pocket = null;


	public Agent (string name, int x, int y) : base (name, x, y, false) {
		_visuals = new VisualEntity(x, y, MapViewer.AGENT);
	}

	public Agent (Agent from, WorldState newWorld) : base(from, newWorld) {
		if (from._pocket != null) _pocket = from._pocket.Clone(newWorld);
		else _pocket = null;

		_beliefs = from._beliefs;
		_desires = from._desires;
		_currentDesire = from._currentDesire;
		_intentions = from._intentions;
		_currentPath = from._currentPath;
	}

	public void Init () {
		_beliefs = WorldState.DefaultBelief();
	}

	public void Tick () {
		//Percept
		int deltaBeliefs = _beliefs.AddPercept(WorldState.RealWorld.Percept(X, Y, _visionRange));
		foreach (KeyValuePair<string, Entity> kvp in WorldState.RealWorld.Entities) {
			Entity entity = kvp.Value;
			if (entity is Package || entity is DeliverySpot) {
				_beliefs.AddPercept(WorldState.RealWorld.Percept(entity.X, entity.Y, _visionRange));
			}
		}

		//Check if the path is obstructed --> reevaluate desires / intentions
		if (_currentPath.Count > 0 && _beliefs.PathObstructed(_currentPath)) { 
			_currentPath = new List<Coord>();
			_generateDesire = true;
		}

		//process desires
		if (_currentDesire == null || _generateDesire || deltaBeliefs > _deltaBeliefTreshold) {
			Desire d = _currentDesire;
			ChooseDesire();
			//if (d != _currentDesire)
			_generateIntentions = true;
			_generateDesire = false;
		}

		//process intentions
		if (_intentions.Count == 0 || _generateIntentions || deltaBeliefs > _deltaBeliefTreshold) {
			_intentions = MakePlans(15, 8);
			_generateIntentions = false;
		}

		//Check if we need a path
		if (_currentPath.Count == 0 && _intentions.Count > 0) {
			_currentPath = _beliefs.PathFind(this, _intentions[0]);
			if (_currentPath == null) _currentPath = new List<Coord>();
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
		public float _score;

		public MakePlanActionStruct(Action action, WorldState worldState, float cost, float score) {
			_action = action;
			_worldState = worldState;
			_cost = cost;
			_score = score;
		}

		public void SetCost (float cost) {
			_cost = cost;
		}

		public float Evaluate () {
			return (_cost + 1) * (_score + 1);
		}
	}

	public List<Action> MakePlans (int maxTries, int maxMoves) {
		List<List<MakePlanActionStruct>> allRuns = new List<List<MakePlanActionStruct>>();

		for (int i = 0; i < maxTries; i++) {
			List<MakePlanActionStruct> plan = new List<MakePlanActionStruct>();

			WorldState currentWorldState = _beliefs.Clone();

			float temperature = 0.1f;
			float alpha = 0.5f;
			float baseCost = 0;
			for (int j = 0; j < maxMoves; j++) {
				MakePlanActionStruct generatedAction = GenerateMove(currentWorldState, temperature, baseCost, plan);
				baseCost += generatedAction._cost;
				if (generatedAction._worldState == null) break;

				plan.Add(generatedAction);
				temperature *= alpha;
				currentWorldState = generatedAction._worldState;
			}
			allRuns.Add(plan);
		}

		//Get the best plan
		float minimumScoreCost = float.PositiveInfinity;
		int iMinimum = 0;
		for (int i = 0; i < allRuns.Count; i++) {
			int last = allRuns[i].Count - 1;
			if (last > 0) {
				float score = allRuns[i][last].Evaluate();

				if (score < minimumScoreCost) {
					iMinimum = i;
					minimumScoreCost = score;
				}
			}
		}

		//Make it a list
		List<Action> bestPlan = new List<Action>();
		foreach(MakePlanActionStruct makePlanAction in allRuns[iMinimum]) {
			bestPlan.Add(makePlanAction._action);
		}

		return bestPlan;
	}

	public MakePlanActionStruct GenerateMove (WorldState currentWorldState, float t, float baseCost, List<MakePlanActionStruct> currentRun) {
		if (_currentDesire.Score(currentWorldState, Name) == 0) { //We achieved our desire during the last itération, returning null !
			return new MakePlanActionStruct(new Action(null, null), null, float.PositiveInfinity, float.PositiveInfinity);
		}
		//Get all possible actions
		List<Action> rawAvailableActions = currentWorldState.GetActions();
		 if (_pocket != null && _pocket is ActionEntity) {
			foreach(string actionName in (_pocket as ActionEntity).GetActionNames()) {
				rawAvailableActions.Add(new Action(_pocket.Name, actionName));
			}
		}
		
		//Get all action that i have not already done, and that i can find a path to
		List<Action> availableActions = new List<Action>();
		foreach(Action a in rawAvailableActions) {
			bool inRun = false;

			foreach(MakePlanActionStruct makePlanAction in currentRun) {
				if (a.ToString().Equals(makePlanAction._action.ToString())) {
					inRun = true;
					break;
				}
			}

			if (!inRun) {
				List<Coord> path = currentWorldState.PathFind(this, a);
				if (path != null) {
					availableActions.Add(a);
				}
			}
		}

		//If no actions are available, return "null"
		if (availableActions.Count == 0) {
			return new MakePlanActionStruct(new Action(null, null), null, float.PositiveInfinity, float.PositiveInfinity);
		}


		//SA-Move
		float bestScore = float.PositiveInfinity;
		MakePlanActionStruct bestAction = new MakePlanActionStruct(new Action(null, null), null, float.PositiveInfinity, float.PositiveInfinity);
		for (int i = 0; i < 10; i++) {
			WorldState next = currentWorldState.Clone();

			Action action = availableActions[random.Next(availableActions.Count)];
			List<Coord> actionPath = next.Do(this, action);

			if (actionPath == null) actionPath = new List<Coord>();

			float cost = baseCost + actionPath.Count + 1;
			float score = _currentDesire.Score(next, Name);
			MakePlanActionStruct currentAction = new MakePlanActionStruct(action, next, cost, score);

			if (currentAction.Evaluate() < bestScore) {
				bestAction = currentAction;
				bestScore = currentAction.Evaluate();
			}

			if (random.NextDouble() < t) {
				return currentAction;
			}
		}

		return bestAction;
	}

	public override Entity Clone (WorldState newWorld) {
		return new Agent(this, newWorld);
	}

	public string PrintBeliefs() {
		string ans = "AGENT BELIEFS: \n";

		ans += _beliefs.ToString();

		return ans;
	}
}
