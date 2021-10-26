using Godot;
using System;
using System.Drawing;

public abstract class Entity {
    protected string _name = "none";              public string Name {get{return _name;}}
    protected int _x = 0;                         public int X {get{return _x;}}
    protected int _y = 0;                         public int Y {get{return _y;}}
    protected bool _solid = true;                 public bool Solid {get{return _solid;}}
    
    private WorldState _currentWorld = null;            public WorldState CurrentWorld {get {return _currentWorld;}}


    public Entity (string name, int x, int y, bool solid) {
        _name = name;
        _x = x;
        _y = y;
        _solid = solid;
        _currentWorld = WorldState.RealWorld;
    }

    protected Entity (Entity from, WorldState newWorld) {
        _name = from._name;
        _x = from._x;
        _y = from._y;
        _solid = from.Solid;
        _currentWorld = newWorld;
    }

    public Coord GetCoord () {
        return new Coord(X, Y);
    }

    public void SetCoord(int x, int y) {
        _x = x;
        _y = y;
    }

    public void SetCoord(Coord coord) {
        _x = coord.X;
        _y = coord.Y;
    }
    
    public virtual void Step () {} 

    public abstract Entity Clone (WorldState newWorld);
}