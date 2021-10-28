using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Door : ActionEntity {
    public Door (string name, int x, int y) : base (name, x, y, true) {

    }

    public Door (Door from, WorldState newWorld) : base (from, newWorld) {
        //System.Console.WriteLine("CLONE " + from._name + " : " + from._solid);
    }

    public override Entity Clone(WorldState newWorld) {
        return new Door(this, newWorld);
    }

    public override void Do(Agent agent, Action action,
    [CallerFilePath] string callerFilePath = "", 
    [CallerLineNumber] long callerLineNumber = 0,
    [CallerMemberName] string callerMember= "") {
        //System.Console.WriteLine("      xx        ." + action._actionName +".");
        if (action._actionName.Equals("open")) _solid = false;
        if (action._actionName.Equals("close")) _solid = true;        
    }

    public override List<string> GetActionNames() {
        if (Solid) return new List<string>{"open"};
        return new List<string>{"close"};
    }
}