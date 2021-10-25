using Godot;
using System;
using System.Collections.Generic;

public class DoorVisuals
{
    public static bool ShouldBeRotated(int x, int y)
    {
        bool foundWall = false;
        Dictionary<string, Entity> entitiesUp = WorldState.RealWorld.GetEntitiesAt(x, y - 1);
        foreach (KeyValuePair<string, Entity> kvp in entitiesUp)
        {
            Entity entity = kvp.Value;
            if (entity is Wall)
            {
                foundWall = true;
                break;
            }
        }
        if (!foundWall)
            return false;

        foundWall = false;
        Dictionary<string, Entity> entitiesDown = WorldState.RealWorld.GetEntitiesAt(x, y + 1);
        foreach (KeyValuePair<string, Entity> kvp in entitiesDown)
        {
            Entity entity = kvp.Value;
            if (entity is Wall)
            {
                foundWall = true;
                break;
            }
        }
        if (!foundWall)
            return false;

        return true;
    }

    public static void UpdateState(string instanceName, Spatial instance, int x, int y)
    {
        foreach (KeyValuePair<string, Entity> kvp in WorldState.RealWorld.GetEntitiesAt(x, y))
        {
            string entityName = kvp.Key;
            Entity entity = kvp.Value;

            if (entityName.Contains("Door"))
            {
                if (entity.Solid)
                {
                    if (instanceName.Contains("open"))
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
                    if (instanceName.Contains("open"))
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