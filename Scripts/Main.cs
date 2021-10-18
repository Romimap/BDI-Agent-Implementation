using Godot;
using System;
using System.Collections.Generic;

public class Main : Spatial
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GD.Print("Hello World !");
		WorldState realWorld = new WorldState("./Maps/map001.png");
		GD.Print(realWorld);
		System.Console.WriteLine(realWorld);
		foreach(KeyValuePair<string, Agent> kvp in realWorld.Agents) {
			GD.Print(kvp.Value.PrintBeliefs());    
		}

		realWorld.Tick();
		foreach(KeyValuePair<string, Agent> kvp in realWorld.Agents) {
			GD.Print(kvp.Value.PrintBeliefs());    
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		
	}
}
