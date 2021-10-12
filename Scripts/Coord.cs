using System;

public struct Coord {
    private int _x;
    private int _y;

    public int X {get{return _x;}}
    public int Y {get{return _y;}}

    public Coord (int x, int y) {
        _x = x;
        _y = y;
    }

    public float distance (Coord other) {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }
}

