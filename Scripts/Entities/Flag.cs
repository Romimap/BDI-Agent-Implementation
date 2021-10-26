using System;
using System.Collections.Generic;

public class Flag : ActionEntity {

    public Flag (string name, int x, int y) : base(name, x, y, false) {

    }

    public Flag (Flag from, WorldState newWorld) : base(from, newWorld) {

    }

    public override Entity Clone(WorldState newWorld) {
        return new Flag(this, newWorld);
    }

    public override void Do(Agent agent, Action action) {
        
    }

    public override List<string> GetActionNames() {
        return new List<string>{"watch"};
    }
}