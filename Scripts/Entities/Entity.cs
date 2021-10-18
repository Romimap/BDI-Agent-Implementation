using Godot;
using System;
using System.Drawing;

public abstract class Entity {
    protected string _name = "none";              public string Name {get{return _name;}}
    protected int _x = 0;                         public int X {get{return _x;}}
    protected int _y = 0;                         public int Y {get{return _y;}}
    protected bool _solid = true;                 public bool Solid {get{return _solid;}}

    // Godot visual representation
    protected Spatial _instance;
    public bool _isVisible;

    // Godot map entities prefabs
    public static PackedScene WALL = (PackedScene)ResourceLoader.Load("res://Entities/Wall.tscn");
    public static PackedScene WALL_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Wall (ghost).tscn");
    public static PackedScene FLOOR = (PackedScene)ResourceLoader.Load("res://Entities/Floor.tscn");
    public static PackedScene FLOOR_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Floor (ghost).tscn");

    public Entity (string name, int x, int y, bool solid) {
        _name = name;
        _x = x;
        _y = y;
        _solid = solid;
    }

    protected Entity (Entity from) {
        _name = from._name;
        _x = from._x;
        _y = from._y;
        _solid = Solid;
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

    public abstract Entity Clone ();
}