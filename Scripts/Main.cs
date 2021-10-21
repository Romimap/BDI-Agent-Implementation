using Godot;
using System;
using System.Collections.Generic;

public class Main : Spatial {	
	MapViewer mapViewer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		WorldState realWorld = new WorldState("./Maps/map001.png");
		mapViewer = new MapViewer(GetNode("VisibleMap"), GetNode("InvisibleMap"), GetNode("./Camera"));

		Flag f = new Flag("flag", 0, 0);
		realWorld.AddEntity(f, 6, 7);

		Agent a = new Agent("gotoAgent", 0, 0);
		a.AddDesire(new GoToDesire(f.X, f.Y));
		realWorld.AddEntity(a, 7, 1);

		realWorld.Init();

		//WorldState percept = WorldState.RealWorld.Percept(6, 7, 1);
		//a.Beliefs.AddPercept(percept);

		GD.Print(realWorld);
	}

	float timer = 1;
	public override void _Process(float delta) {
		timer -= delta;
		if (timer <= 0) {
			timer += 1;
			WorldState.RealWorld.Tick();
			foreach (KeyValuePair<string, Agent> kvp in WorldState.RealWorld.Agents) {
				Agent agent = kvp.Value;
				GD.Print(agent.PrintBeliefs());
				MapViewer.ChangeVisibility(agent.X, agent.Y, 1);
			}
		}
	}
}
