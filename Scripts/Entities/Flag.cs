using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Flag : ActionEntity {

    public Flag (string name, int x, int y) : base(name, x, y, false) {

    }

    public Flag (Flag from, WorldState newWorld) : base(from, newWorld) {

    }

    public override Entity Clone(WorldState newWorld) {
        return new Flag(this, newWorld);
    }

    public override void Do(Agent agent, Action action,
    [CallerFilePath] string callerFilePath = "", 
    [CallerLineNumber] long callerLineNumber = 0,
    [CallerMemberName] string callerMember= "") {
        
    }

    public override List<string> GetActionNames() {
        return new List<string>{"go to"};
    }
}