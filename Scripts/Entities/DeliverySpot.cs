using System;

public class DeliverySpot : Entity {
    
    public DeliverySpot (string name, int x, int y) : base(name, x, y, true) {

    }

    public DeliverySpot (DeliverySpot from) : base(from) {

    }

    public override Entity Clone() {
        return new DeliverySpot(this);
    }    
}