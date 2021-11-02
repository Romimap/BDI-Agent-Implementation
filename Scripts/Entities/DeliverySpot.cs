using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class DeliverySpot : ActionEntity {
    
    public DeliverySpot (string name, int x, int y) : base(name, x, y, true) {
        _visuals = new VisualEntity(x, y, MapViewer.DELIVERY_SPOT, MapViewer.DELIVERY_SPOT_GHOST);
    }

    public DeliverySpot (DeliverySpot from, WorldState newWorld) : base(from, newWorld) {

    }

    public override Entity Clone(WorldState newWorld) {
        return new DeliverySpot(this, newWorld);
    }

    public override void Do(Agent agent, Action action, 
    [CallerFilePath] string callerFilePath = "", 
    [CallerLineNumber] long callerLineNumber = 0, 
    [CallerMemberName] string callerMember = ""){
        
    }

    public override List<string> GetActionNames()
    {
        return new List<string>{"go to"};
    }
}