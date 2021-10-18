using Godot;
using System;
using System.Collections.Generic;

public class Main : Spatial
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		
		WorldState realWorld = new WorldState("./Maps/map001.png");
		GD.Print("REAL WORLD:");
		GD.Print(realWorld);
		System.Console.WriteLine(realWorld);
		GD.Print("BELIEFS BEFORE TICK:");
		foreach(KeyValuePair<string, Agent> kvp in realWorld.Agents) {
			GD.Print(kvp.Value.PrintBeliefs());    
		}

		realWorld.Tick();
		GD.Print("BELIEFS AFTER TICK:");
		foreach(KeyValuePair<string, Agent> kvp in realWorld.Agents) {
			GD.Print(kvp.Value.PrintBeliefs());    
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		
	}
}
