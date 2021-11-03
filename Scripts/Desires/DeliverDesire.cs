using System;

public class DeliverDesire : Desire {
    string _package;
    string _deliverySpot;

    public DeliverDesire (Entity package, Entity deliverySpot) {
        _package = package.Name;
        _deliverySpot = deliverySpot.Name;
    }

    public override float PriorityScore(WorldState worldState, string agent) {
        return 1;
    }

    public override float Score(WorldState worldState, string agent) {
        Agent a = worldState.Agents[agent];
        Entity package = null;
        if (worldState.Entities.ContainsKey(_package)) package = worldState.Entities[_package];
        Entity deliverySpot = null;
        if (worldState.Entities.ContainsKey(_deliverySpot)) deliverySpot = worldState.Entities[_deliverySpot];


        if (package != null && deliverySpot != null && (Math.Abs(package.X - deliverySpot.X) + Math.Abs(package.Y - deliverySpot.Y)) <= 1) { //The package is on the delivery spot, best score
            return 0;
        } else if (a._pocket != null && a._pocket.Name.Equals(_package)) { //Minimize the distance between the agent and the spot
            return 1 + (Math.Abs(a.X - deliverySpot.X) + Math.Abs(a.Y - deliverySpot.Y));
        } else { //Minimize the distance between the agent and the package
            if (package != null) return (10 + (Math.Abs(a.X - package.X) + Math.Abs(a.Y - package.Y))) * 2;
            return 100; // TODO: modify value
        }
    }
}