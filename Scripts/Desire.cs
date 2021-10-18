public abstract class Desire {
    public abstract float PriorityScore (Agent agent); //The lower the better
    public abstract float Score (WorldState worldState); //The lower the better
}