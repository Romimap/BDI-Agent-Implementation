using System;

public class Wall : Entity {
    
    public Wall (string name, int x, int y) : base(name, x, y, true) {

    }

    public Wall (Wall from) : base(from) {

    }

    public override Entity Clone() {
        return new Wall(this);
    }    
}