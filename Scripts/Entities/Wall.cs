using System;

public class Wall : Entity {
    
    public Wall (string name, int x, int y) : base(name, x, y, true) {
        _visuals = new VisualEntity(x, y, MapViewer.WALL, MapViewer.WALL_GHOST);
    }

    public Wall (Wall from, WorldState newWorld) : base(from, newWorld) {

    }

    public override Entity Clone(WorldState newWorld) {
        return new Wall(this, newWorld);
    }    
}