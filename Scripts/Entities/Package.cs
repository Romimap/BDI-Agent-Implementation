using System;
using System.Collections.Generic;

public class Package : ActionEntity {
    
    public Package (string name, int x, int y) : base(name, x, y, true) {

    }

    public Package (Package from) : base(from) {

    }

    public override Entity Clone() {
        return new Package(this);
    }

    public override void Do(Agent agent, Action action)
    {
        throw new NotImplementedException();
    }

    public override List<string> GetActionNames()
    {
        return new List<string>();
        // throw new NotImplementedException();
    }
}