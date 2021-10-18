using System;

public class Floor : Entity {
    
    public Floor (string name, int x, int y) : base(name, x, y, false) {

    }

    public Floor (Floor from) : base(from) {

    }

    public override Entity Clone() {
        return new Floor(this);
    }    
}