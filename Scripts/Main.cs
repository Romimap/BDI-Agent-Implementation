using Godot;
using System;
using System.Collections.Generic;

public class Main : Spatial {	
	MapViewer mapViewer;
	public static RichTextLabel richTextLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		richTextLabel = (RichTextLabel)GetNode("/root/World/Control/VBoxContainer/RichTextLabel");
		WorldState realWorld = new WorldState("./Maps/map001.png");

		Flag f = new Flag("flag", 0, 0);
		realWorld.AddEntity(f, 2, 13);

		Agent a = new Agent("gotoAgent", 0, 0);
		a.AddDesire(new GoToDesire(2, 13));
		realWorld.AddEntity(a, 7, 1);

		realWorld.Init();

		//WorldState percept = WorldState.RealWorld.Percept(2, 13, 1);
		//a.Beliefs.AddPercept(percept);

		// GD.Print(realWorld);
		mapViewer = new MapViewer(GetNode("VisibleMap"), GetNode("InvisibleMap"), GetNode("./Camera"));
	}

	float timer = 0.5f;
	public override void _Process(float delta) {
		timer -= delta;
		if (timer <= 0) {
			timer += 0.5f;
			WorldState.RealWorld.Tick();
			foreach (KeyValuePair<string, Agent> kvp in WorldState.RealWorld.Agents) {
				Agent agent = kvp.Value;
				// GD.Print(agent.PrintBeliefs());
				GD.Print("Agent pos: (" + agent.X + ", " + agent.Y + ")");
				MapViewer.ChangeVisibility(agent);
			}
		}
	}
}
