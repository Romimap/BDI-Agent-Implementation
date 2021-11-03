using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

public class Package : ActionEntity {
    bool _inPocketOfSomeone = false;

    public Package (string name, int x, int y) : base(name, x, y, true) {
        _visuals = new VisualEntity(x, y, MapViewer.PACKAGE, MapViewer.PACKAGE_GHOST);
    }

    public Package (Package from, WorldState newWorld) : base(from, newWorld) {
        _inPocketOfSomeone = from._inPocketOfSomeone;
        _visuals = new VisualEntity(from.X, from.Y, MapViewer.PACKAGE, MapViewer.PACKAGE_GHOST);
    }

    public override Entity Clone(WorldState newWorld) {
        return new Package(this, newWorld);
    }

    public override void Do(Agent agent, Action action,
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] long callerLineNumber = 0,
        [CallerMemberName] string callerMember= "") {
            
        Agent a = CurrentWorld.Agents[agent.Name];
        if (_inPocketOfSomeone && action._actionName.Equals("drop")) {
            _inPocketOfSomeone = false;
            a._pocket = null;
            CurrentWorld.AddEntity(this, a.X, a.Y);
            GD.Print("DROP");

            Visuals.UpdatePosition(a.X, a.Y);
            Visuals.SetVisible(true);
            Spatial instance = Visuals.VisibleInstance;
            (instance as PackageNode).DropOff();
        }
        if (!_inPocketOfSomeone && action._actionName.Equals("pickup")) {
            _inPocketOfSomeone = true;
            a._pocket = this;
            CurrentWorld.RemoveEntityAt(X, Y, this);
            GD.Print("PICKUP");

            Spatial instance = Visuals.VisibleInstance;
            (instance as PackageNode).PickUp();
        }
    }

    public override List<string> GetActionNames() {
        if (_inPocketOfSomeone) return new List<string>{"drop"};
        return new List<string>{"pickup"};
    }
}