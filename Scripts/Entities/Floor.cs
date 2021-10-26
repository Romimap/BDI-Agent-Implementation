using System;
using Godot;

public class Floor : Entity {
    
    public Floor (string name, int x, int y) : base(name, x, y, false) {
        _visuals = new VisualEntity(x, y, MapViewer.FLOOR, MapViewer.FLOOR_GHOST);
    }

    public Floor (Floor from) : base(from) {
        
    }

    public override Entity Clone() {
        return new Floor(this);
    }    
}