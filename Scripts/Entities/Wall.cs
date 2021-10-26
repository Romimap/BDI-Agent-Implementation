using System;
using Godot;

public class Wall : Entity {
    
    public Wall (string name, int x, int y) : base(name, x, y, true) {
        _visuals = new VisualEntity(x, y, MapViewer.WALL, MapViewer.WALL_GHOST);
    }

    public Wall (Wall from) : base(from) {
        
    }

    public override Entity Clone() {
        return new Wall(this);
    }    
}