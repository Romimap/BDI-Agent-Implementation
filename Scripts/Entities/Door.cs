using System;

public class Door : Entity {

    private bool _isOpen = false;       public bool IsOpen {get {return _isOpen;}}
    
    public Door (string name, int x, int y) : base(name, x, y, true) {

    }

    public Door (Door from) : base(from) {

    }

    public override Entity Clone() {
        return new Door(this);
    }

    public bool Open() {
        if (_isOpen)
            return false;
        _isOpen = true;
        return true;
    }

    public bool Close() {
        if (!_isOpen)
            return false;
        _isOpen = false;
        return true;
    }
}