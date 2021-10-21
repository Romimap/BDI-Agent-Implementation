public abstract class Desire {
    public abstract float PriorityScore (WorldState worldState, string agent); //The lower the better
    public abstract float Score (WorldState worldState, string agent); //The lower the better
}