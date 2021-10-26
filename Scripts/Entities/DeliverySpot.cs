using System;

public class DeliverySpot : Entity {
    
    public DeliverySpot (string name, int x, int y) : base(name, x, y, true) {

    }

    public DeliverySpot (DeliverySpot from, WorldState newWorld) : base(from, newWorld) {

    }

    public override Entity Clone(WorldState newWorld) {
        return new DeliverySpot(this, newWorld);
    }    
}