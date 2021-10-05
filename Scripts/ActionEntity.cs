using System;
using System.Collections.Generic;

public abstract class ActionEntity : Entity {

    public ActionEntity (string name, int x, int y, bool solid) : base(name, x, y, solid) {
        
    }

    public abstract void Do (string action, List<object> parameters);
    public abstract List<string> GetActionList();
}