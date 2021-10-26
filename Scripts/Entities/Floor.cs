using System;

public class Floor : Entity {
    
    public Floor (string name, int x, int y) : base(name, x, y, false) {

    }

    public Floor (Floor from, WorldState newWorld) : base(from, newWorld) {

    }

    public override Entity Clone(WorldState newWorld) {
        return new Floor(this, newWorld);
    }    
}