using Godot;
using System;
using System.Collections.Generic;

public class DoorVisuals
{
	// Return true if a wall is on the y+1 tile
	public static bool ShouldBeRotated(int x, int y)
	{
		Dictionary<string, Entity> entitiesUp = WorldState.RealWorld.GetEntitiesAt(x, y - 1);
		if (entitiesUp == null) return false;
		
		foreach (KeyValuePair<string, Entity> kvp in entitiesUp)
		{
			Entity entity = kvp.Value;
			if (entity is Wall)
			{
				return true;
			}
		}
		return false;
	}
}
