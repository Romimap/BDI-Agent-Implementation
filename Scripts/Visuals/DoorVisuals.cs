using Godot;
using System;
using System.Collections.Generic;

public class DoorVisuals
{
    // Return true if a wall is on the y+1 tile
    public static bool ShouldBeRotated(int x, int y)
    {
        Dictionary<string, Entity> entitiesUp = WorldState.RealWorld.GetEntitiesAt(x, y - 1);
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

    // TODO : remove
    public static void UpdateState(string instanceName, Spatial instance, int x, int y)
    {
        foreach (KeyValuePair<string, Entity> kvp in WorldState.RealWorld.GetEntitiesAt(x, y))
        {
            string entityName = kvp.Key;
            Entity entity = kvp.Value;

            if (entityName.ToLower().Contains("door"))
            {
                if (entity.Solid)
                {
                    if (instanceName.ToLower().Contains("open"))
                    {
                        instance.Visible = false;
                    }
                    else
                    {
                        instance.Visible = true;
                    }
                }
                else
                {
                    if (instanceName.ToLower().Contains("open"))
                    {
                        instance.Visible = true;
                    }
                    else
                    {
                        instance.Visible = false;
                    }
                }
            }
        }
    }
}