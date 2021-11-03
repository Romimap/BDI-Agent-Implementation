using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Main : Spatial {	
	MapViewer mapViewer;
	public static RichTextLabel richTextLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		richTextLabel = (RichTextLabel)GetNode("/root/World/Control/VBoxContainer/RichTextLabel");
		WorldState realWorld = new WorldState("./Maps/map001.png");


		DeliverySpot ds = new DeliverySpot("delivery spot", 2, 13);
		realWorld.AddEntityWithoutCloning(ds, 2, 13);

		Package p = new Package("package", 1, 7);
		realWorld.AddEntityWithoutCloning(p, 1, 7);

		Agent a = new Agent("deliveryBot", 7, 1);
		a.AddDesire(new DeliverDesire(p, ds));
		realWorld.AddEntityWithoutCloning(a, 7, 1);

		realWorld.Init();

		//WorldState percept = WorldState.RealWorld.Percept(2, 13, 1);
		//a.Beliefs.AddPercept(percept);

		mapViewer = new MapViewer(GetNode("VisibleMap"), GetNode("InvisibleMap"), GetNode("./Camera"));
		MapViewer.UpdateTilesAround(a);
	}

	float timer = 0.5f;
	public override void _Process(float delta) {
		timer -= delta;
		if (timer <= 0) {
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			timer += 0.5f;
			WorldState.RealWorld.Tick();
			// System.Console.WriteLine("WORLD STATE \n" + WorldState.RealWorld);
			
			foreach (KeyValuePair<string, Agent> kvp in WorldState.RealWorld.Agents) {
				Agent agent = kvp.Value;
				GD.Print("Agent pos: (" + agent.X + ", " + agent.Y + ")");
				MapViewer.UpdateTilesAround(agent);
				// System.Console.WriteLine(agent.Name + " pocket : " + agent._pocket?.Name);
			}


			stopwatch.Stop();
			GD.Print("Tick time: " + stopwatch.ElapsedMilliseconds + "ms\n");
		}
	}
}
