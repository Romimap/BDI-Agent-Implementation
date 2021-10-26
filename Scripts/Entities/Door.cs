using System;
using System.Collections.Generic;

public class Door : ActionEntity {
    public Door (string name, int x, int y) : base (name, x, y, true) {
        _visuals = new VisualEntity(x, y, MapViewer.DOOR, MapViewer.DOOR_GHOST, MapViewer.DOOR_OPEN, MapViewer.DOOR_OPEN_GHOST);
        if (DoorVisuals.ShouldBeRotated(x, y)) _visuals.ApplyRotation(0, (float)(Math.PI) / 2.0f, 0);
    }

    public Door (Door from) : base (from) {
        //System.Console.WriteLine("CLONE " + from._name + " : " + from._solid);
    }

    public override Entity Clone() {
        return new Door(this);
    }

    public override void Do(Agent agent, Action action) {
        //System.Console.WriteLine("      xx        ." + action._actionName +".");
        if (action._actionName.Equals("open")) {
            _solid = false;
            if (_visuals != null)
                _visuals.SetActionned(true);
        }
        if (action._actionName.Equals("close")) {
            _solid = true;
            if (_visuals != null)
                _visuals.SetActionned(false);
        }
    }

    public override List<string> GetActionNames() {
        if (Solid) return new List<string>{"open"};
        return new List<string>{"close"};
    }

    public static bool ShouldBeRotated(int x, int y)
    {
        bool foundWall = false;
        Dictionary<string, Entity> entitiesUp = WorldState.RealWorld.GetEntitiesAt(x, y - 1);
        foreach (KeyValuePair<string, Entity> kvp in entitiesUp)
        {
            Entity entity = kvp.Value;
            if (entity is Wall)
            {
                foundWall = true;
                break;
            }
        }
        if (!foundWall)
            return false;

        foundWall = false;
        Dictionary<string, Entity> entitiesDown = WorldState.RealWorld.GetEntitiesAt(x, y + 1);
        foreach (KeyValuePair<string, Entity> kvp in entitiesDown)
        {
            Entity entity = kvp.Value;
            if (entity is Wall)
            {
                foundWall = true;
                break;
            }
        }
        if (!foundWall)
            return false;

        return true;
    }
}