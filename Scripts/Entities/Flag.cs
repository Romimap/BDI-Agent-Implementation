using System;
using System.Collections.Generic;

public class Flag : ActionEntity {

    public Flag (string name, int x, int y) : base(name, x, y, false) {

    }

    public Flag (Flag from) : base(from) {

    }

    public override Entity Clone() {
        return new Flag(this);
    }

    public override void Do(Agent agent, Action action) {
        
    }

    public override List<string> GetActionNames() {
        return new List<string>{"watch"};
    }
}