using System;
using System.Collections.Generic;

public class Package : ActionEntity {
    bool _inPocketOfSomeone = false;

    public Package (string name, int x, int y) : base(name, x, y, true) {

    }

    public Package (Package from, WorldState newWorld) : base(from, newWorld) {
        _inPocketOfSomeone = from._inPocketOfSomeone;
    }

    public override Entity Clone(WorldState newWorld) {
        return new Package(this, newWorld);
    }

    public override void Do(Agent agent, Action action) {
        if (_inPocketOfSomeone && action._actionName.Equals("drop")) {
            _inPocketOfSomeone = false;
            CurrentWorld.AddEntity(this, agent.X, agent.Y);
        }
        if (!_inPocketOfSomeone && action._actionName.Equals("pickup")) {
            _inPocketOfSomeone = true;
            agent._pocket = this;
            CurrentWorld.RemoveEntityAt(X, Y, this);
        }
    }

    public override List<string> GetActionNames() {
        if (_inPocketOfSomeone) return new List<string>{"drop"};
        return new List<string>{"pickup"};
    }
}