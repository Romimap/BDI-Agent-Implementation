using System;

public class DeliverySpot : Entity {
    
    public DeliverySpot (string name, int x, int y) : base(name, x, y, true) {
        _visuals = new VisualEntity(x, y, MapViewer.DELIVERY_SPOT, MapViewer.DELIVERY_SPOT_GHOST);
    }

    public DeliverySpot (DeliverySpot from, WorldState newWorld) : base(from, newWorld) {

    }

    public override Entity Clone(WorldState newWorld) {
        return new DeliverySpot(this, newWorld);
    }    
}