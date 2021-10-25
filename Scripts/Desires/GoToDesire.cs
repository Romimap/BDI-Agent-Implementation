using System;

public class GoToDesire : Desire {
    private int _x;
    private int _y;
    public GoToDesire (int x, int y) {
        _x = x;
        _y = y;
    }

    public override float Score(WorldState worldState, string agent) {
        //System.Console.WriteLine("AGENT COORD  = " + worldState.Agents[agent].X + ", " + worldState.Agents[agent].Y);
        //System.Console.WriteLine("TARGET COORD = " + _x + ", " + _y);
        float dx = worldState.Agents[agent].X - _x;
        float dy = worldState.Agents[agent].Y - _y;
        float score = (float)Math.Sqrt(dx*dx + dy*dy);
        //System.Console.WriteLine(worldState);
        //System.Console.WriteLine("SCORE = " + score);
        return score;
    }

    public override float PriorityScore(WorldState worldState, string agent) {
        return 1;
    }
}