using System;

public class GoToDesire : Desire {
    private int _x, _y;
    public GoToDesire (int x, int y) {
        _x = x;
        _y = y;

    }

    public override float Score(WorldState worldState, string agent) {
        int dx = worldState.Agents[agent].X - _x;
        int dy = worldState.Agents[agent].X - _y;
        return Math.Abs(dx) + Math.Abs(dy);
    }

    public override float PriorityScore(WorldState worldState, string agent) {
        return 1;
    }
}