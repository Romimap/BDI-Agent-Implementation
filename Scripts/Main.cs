using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Main : Spatial
{
	public static RichTextLabel richTextLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		richTextLabel = (RichTextLabel)GetNode("/root/World/Control/VBoxContainer/RichTextLabel");
		WorldState realWorld = new WorldState("./Maps/map001.png");

		Flag f = new Flag("flag", 2, 13);
		realWorld.AddEntityWithoutCloning(f, 2, 13);

		Agent a = new Agent("gotoAgent", 7, 1);
		a.AddDesire(new GoToDesire(2, 13));
		realWorld.AddEntityWithoutCloning(a, 7, 1);

		Package p = new Package("package", 2, 13);
		realWorld.AddEntityWithoutCloning(p, 2, 13);

		DeliverySpot d = new DeliverySpot("deliverySpot", 14, 7);
		realWorld.AddEntityWithoutCloning(d, 14, 7);

		realWorld.Init();

		//WorldState percept = WorldState.RealWorld.Percept(2, 13, 1);
		//a.Beliefs.AddPercept(percept);

		// GD.Print(realWorld);
		MapViewer mapViewer = new MapViewer(GetNode("VisibleMap"), GetNode("InvisibleMap"), GetNode("./Camera"));
		MapViewer.UpdateTilesAround(a);
	}

	float timer = 0.5f;
	public override void _Process(float delta)
	{
		timer -= delta;
		if (timer <= 0) {
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			timer += 0.5f;
			WorldState.RealWorld.Tick();
			foreach (KeyValuePair<string, Agent> kvp in WorldState.RealWorld.Agents)
			{
				Agent agent = kvp.Value;
				// GD.Print(agent.PrintBeliefs());
				GD.Print("Agent pos: (" + agent.X + ", " + agent.Y + ")");
				MapViewer.UpdateTilesAround(agent);
			}
			stopwatch.Stop();
			GD.Print("Tick time : " + stopwatch.ElapsedMilliseconds + "ms");
		}
	}
}
