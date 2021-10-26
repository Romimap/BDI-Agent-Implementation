using System;
using System.Collections.Generic;

public abstract class ActionEntity : Entity {

    public ActionEntity (string name, int x, int y, bool solid) : base(name, x, y, solid) {
        
    }

    public ActionEntity (ActionEntity from, WorldState newWorld) : base(from, newWorld) {
        
    }

    public abstract void Do (Agent agent, Action action);
    public abstract List<string> GetActionNames();
}