using Godot;
using System;
using System.Collections.Generic;

public class MapViewer : Spatial
{
    Spatial _visibleMap;
    Spatial _invisibleMap;
    Camera _camera;

    Spatial _startingVisibleMap;
    Spatial _startingInvisibleMap;

    // Godot map entities prefabs
    static PackedScene WALL = (PackedScene)ResourceLoader.Load("res://Entities/Wall.tscn");
    static PackedScene WALL_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Wall (ghost).tscn");
    static PackedScene FLOOR = (PackedScene)ResourceLoader.Load("res://Entities/Floor.tscn");
    static PackedScene FLOOR_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Floor (ghost).tscn");


    public MapViewer(Node visibleMap, Node invisibleMap, Node camera)
    {
        _startingVisibleMap = (Spatial)visibleMap;
        _startingInvisibleMap = (Spatial)invisibleMap;
        _camera = (Camera)camera;
    }

    public void DisplayMap(WorldState beliefs)
    {
        _visibleMap = _startingVisibleMap;
        _invisibleMap = _startingInvisibleMap;

        Spatial instance = null;
        int width = WorldState.RealWorld.Width;
        int height = WorldState.RealWorld.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Dictionary<string, Entity> entities = beliefs.GetEntitiesAt(x, y);
                if (entities != null && entities.Count != 0) // Draw normal objects (known world)
                {
                    foreach (KeyValuePair<string, Entity> kvp in entities)
                    {
                        Entity entity = kvp.Value;
                        instance = null;
                        if (entity is Floor)
                        {
                            instance = (Spatial)FLOOR.Instance();
                            System.Console.WriteLine("Added floor at (" + x + ", " + y + ")");
                        }
                        else if (entity is Wall)
                        {
                            instance = (Spatial)WALL.Instance();
                            System.Console.WriteLine("Added wall at (" + x + ", " + y + ")");
                        }

                        if (instance != null)
                        {
                            instance.Translate(new Vector3(x, 0, y));
                            _visibleMap.AddChild(instance);
                        }
                    }
                }
                else // Draw ghost objects (unknown world)
                {
                    entities = WorldState.RealWorld.GetEntitiesAt(x, y);
                    if (entities != null)
                    {
                        foreach (KeyValuePair<string, Entity> kvp in entities)
                        {
                            Entity entity = kvp.Value;
                            instance = null;
                            if (entity is Floor)
                            {
                                instance = (Spatial)FLOOR_GHOST.Instance();
                                System.Console.WriteLine("Added floor (ghost) at (" + x + ", " + y + ")");
                            }
                            else if (entity is Wall)
                            {
                                instance = (Spatial)WALL_GHOST.Instance();
                                System.Console.WriteLine("Added wall (ghost) at (" + x + ", " + y + ")");
                            }

                            if (instance != null)
                            {
                                instance.Translate(new Vector3(x, 0, y));
                                _invisibleMap.AddChild(instance);
                            }
                        }
                    }
                }
            }
        }

        // Move camera
        Transform transform = _camera.Transform;

        // ORTHOGONAL CAMERA
        int maxWH = Math.Max(width, height);

        transform.origin = new Vector3(width / 2.0f, maxWH, height / 2.0f);
        transform.origin += new Vector3(maxWH, maxWH, maxWH);
        transform = transform.LookingAt(new Vector3(width / 2.0f, 0, height / 2.0f), new Vector3(0, 1, 0));
        _camera.Size = 3 + maxWH * 1.2f;

        _camera.Transform = transform;
    }
}