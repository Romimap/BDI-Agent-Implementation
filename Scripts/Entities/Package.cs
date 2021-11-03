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
            int x = a.X, y = a.Y;

            if (a._pocket != null && a._pocket.Visuals != null) {
                // Find closest delivery spot position
                foreach (KeyValuePair<string, ActionEntity> kvp in CurrentWorld.ActionEntities) {
                    ActionEntity ae = kvp.Value;
                    if (ae is DeliverySpot) {
                        if (Math.Abs(ae.X - x) <= 1 && Math.Abs(ae.Y - y) <= 1) {
                            x = ae.X;
                            y = ae.Y;
                            break;
                        }
                    }
                }

                a._pocket.Visuals.UpdatePosition(x, y);
                a._pocket.Visuals.SetVisible(true);
                Spatial instance = a._pocket.Visuals.VisibleInstance;
                (instance as PackageNode).DropOff();
            }

            _inPocketOfSomeone = false;
            a._pocket = null;
            CurrentWorld.AddEntity(this, x, y);
        }
        if (!_inPocketOfSomeone && action._actionName.Equals("pickup")) {
            _inPocketOfSomeone = true;
            a._pocket = this;
            CurrentWorld.RemoveEntityAt(X, Y, this);

            if (_visuals != null) {
                Spatial instance = _visuals.VisibleInstance;
                (instance as PackageNode).PickUp();
            }
        }
    }

    public override List<string> GetActionNames() {
        if (_inPocketOfSomeone) return new List<string>{"drop"};
        return new List<string>{"pickup"};
    }
}