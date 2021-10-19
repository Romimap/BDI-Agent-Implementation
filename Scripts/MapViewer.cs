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

    WorldState _beliefs;

    // Godot map entities prefabs
    static PackedScene AGENT = (PackedScene)ResourceLoader.Load("res://Entities/Agent.tscn");
    static PackedScene WALL = (PackedScene)ResourceLoader.Load("res://Entities/Wall.tscn");
    static PackedScene WALL_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Wall (ghost).tscn");
    static PackedScene FLOOR = (PackedScene)ResourceLoader.Load("res://Entities/Floor.tscn");
    static PackedScene FLOOR_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Floor (ghost).tscn");
    static PackedScene DOOR = (PackedScene)ResourceLoader.Load("res://Entities/Door.tscn");
    static PackedScene DOOR_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Door (ghost).tscn");
    static PackedScene DOOR_OPEN = (PackedScene)ResourceLoader.Load("res://Entities/Door (open).tscn");
    static PackedScene DOOR_OPEN_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Door (open ghost).tscn");


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
        _beliefs = beliefs;

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
                        Vector3 rotation = new Vector3(0, 0, 0);

                        if (entity is Floor)
                        {
                            instance = (Spatial)FLOOR.Instance();
                        }
                        else if (entity is Wall)
                        {
                            instance = (Spatial)WALL.Instance();
                        }
                        else if (entity is Agent)
                        {
                            instance = (Spatial)AGENT.Instance();
                        }
                        else if (entity is Door)
                        {
                            Door door = (Door)entity;
                            if (door.IsOpen)
                            {
                                instance = (Spatial)DOOR_OPEN.Instance();
                            }
                            else
                            {
                                instance = (Spatial)DOOR.Instance();
                            }

                            if (DoorShouldBeRotated(x, y))
                                rotation.y = Mathf.Pi / 2.0f;
                        }

                        if (instance != null)
                        {
                            MoveInstance(instance, new Vector3(x, 0, y), rotation);
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
                            Vector3 rotation = new Vector3(0, 0, 0);

                            if (entity is Floor)
                            {
                                instance = (Spatial)FLOOR_GHOST.Instance();
                            }
                            else if (entity is Wall)
                            {
                                instance = (Spatial)WALL_GHOST.Instance();
                            }
                            else if (entity is Door)
                            {
                                Door door = (Door)entity;
                                if (door.IsOpen)
                                {
                                    instance = (Spatial)DOOR_OPEN_GHOST.Instance();
                                }
                                else
                                {
                                    instance = (Spatial)DOOR_GHOST.Instance();
                                }

                                if (DoorShouldBeRotated(x, y))
                                    rotation.y = Mathf.Pi / 2.0f;
                            }

                            if (instance != null)
                            {
                                MoveInstance(instance, new Vector3(x, 0, y), rotation);
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

    private void MoveInstance(Spatial instance, Vector3 translation, Vector3 rotation)
    {
        instance.Translate(translation);
        instance.RotateX(rotation.x);
        instance.RotateY(rotation.y);
        instance.RotateZ(rotation.z);
    }

    private bool DoorShouldBeRotated(int x, int y)
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
}