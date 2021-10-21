using System;
using System.Collections.Generic;

public class Door : ActionEntity {
    public Door (string name, int x, int y) : base (name, x, y, true) {

    }

    public Door (Door from) : base (from) {

    }

    public override Entity Clone() {
        return new Door(this);
    }

    public override void Do(Agent agent, Action action) {
        if (action._actionName.Equals("open")) _solid = false;
        if (action._actionName.Equals("close")) _solid = true;
    }

    public override List<string> GetActionNames() {
        if (Solid) return new List<string>{"open"};
        return new List<string>{"close"};
    }
}