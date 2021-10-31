using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class Package : ActionEntity {
    bool _inPocketOfSomeone = false;

    public Package (string name, int x, int y) : base(name, x, y, true) {
        _visuals = new VisualEntity(x, y, MapViewer.PACKAGE, MapViewer.PACKAGE_GHOST);
    }

    public Package (Package from, WorldState newWorld) : base(from, newWorld) {
        _inPocketOfSomeone = from._inPocketOfSomeone;
    }

    public override Entity Clone(WorldState newWorld) {
        return new Package(this, newWorld);
    }

    public override void Do(Agent agent, Action action,
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] long callerLineNumber = 0,
        [CallerMemberName] string callerMember= "") {
        if (CurrentWorld == WorldState.RealWorld) {

            System.Console.WriteLine(" + + + + + + + + + + + + in package : " + action + " by " + agent.Name + " World : " + CurrentWorld._id + " Agent World : " + agent.CurrentWorld._id);
            Debug.WriteLine(
                "{0}:{1}, {2}", 
                callerFilePath,
                callerLineNumber,
                callerMember);
        } 
        Agent a = CurrentWorld.Agents[agent.Name];
        if (_inPocketOfSomeone && action._actionName.Equals("drop")) {
            _inPocketOfSomeone = false;
            a._pocket = null;
            CurrentWorld.AddEntity(this, a.X, a.Y);
        }
        if (!_inPocketOfSomeone && action._actionName.Equals("pickup")) {
            _inPocketOfSomeone = true;
            a._pocket = this;
            CurrentWorld.RemoveEntityAt(X, Y, this);
        }
    }

    public override List<string> GetActionNames() {
        if (_inPocketOfSomeone) return new List<string>{"drop"};
        return new List<string>{"pickup"};
    }
}