using Godot;
using System;
using System.Collections.Generic;

public class Main : Spatial
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		WorldState realWorld = new WorldState("./Maps/map001.png");
		MapViewer mapViewer = new MapViewer(GetNode("VisibleMap"), GetNode("InvisibleMap"), GetNode("./Camera"));

		GD.Print(realWorld);
		System.Console.WriteLine(realWorld);

		foreach (KeyValuePair<string, Agent> kvp in realWorld.Agents)
		{
			GD.Print(kvp.Value.PrintBeliefs());
		}

		realWorld.Tick();

		foreach (KeyValuePair<string, Agent> kvp in realWorld.Agents)
		{
			GD.Print(kvp.Value.PrintBeliefs());
			mapViewer.DisplayMap(kvp.Value.Beliefs);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

	}
}
