using System;

public class Package : Entity {
    
    public Package (string name, int x, int y) : base(name, x, y, true) {

    }

    public Package (Package from) : base(from) {

    }

    public override Entity Clone() {
        return new Package(this);
    }    
}