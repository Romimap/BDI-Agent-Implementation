using Godot;
using System;
using System.Collections.Generic;

public class MapViewer : Spatial
{
    private Spatial _visibleMap;
    private Spatial _invisibleMap;
    private Camera _camera;

    private Spatial _startingVisibleMap;
    private Spatial _startingInvisibleMap;

    private WorldState _beliefs;

    // private List<List<List<Spatial>>> instances;

    // Godot map entities prefabs
    private static PackedScene AGENT = (PackedScene)ResourceLoader.Load("res://Entities/Agent.tscn");
    private static PackedScene WALL = (PackedScene)ResourceLoader.Load("res://Entities/Wall.tscn");
    private static PackedScene WALL_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Wall (ghost).tscn");
    private static PackedScene FLOOR = (PackedScene)ResourceLoader.Load("res://Entities/Floor.tscn");
    private static PackedScene FLOOR_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Floor (ghost).tscn");
    private static PackedScene DOOR = (PackedScene)ResourceLoader.Load("res://Entities/Door.tscn");
    private static PackedScene DOOR_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Door (ghost).tscn");
    private static PackedScene DOOR_OPEN = (PackedScene)ResourceLoader.Load("res://Entities/Door (open).tscn");
    private static PackedScene DOOR_OPEN_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Door (open ghost).tscn");
    private static PackedScene PACKAGE = (PackedScene)ResourceLoader.Load("res://Entities/Package.tscn");
    private static PackedScene PACKAGE_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Package (ghost).tscn");
    private static PackedScene DELIVERY_SPOT = (PackedScene)ResourceLoader.Load("res://Entities/Delivery spot.tscn");
    private static PackedScene DELIVERY_SPOT_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Delivery spot (ghost).tscn");


    public MapViewer(Node visibleMap, Node invisibleMap, Node camera)
    {
        _startingVisibleMap = (Spatial)visibleMap;
        _startingInvisibleMap = (Spatial)invisibleMap;
        _camera = (Camera)camera;

        InitCamera();
    }

    public void InitCamera()
    {
        int mapWidth = WorldState.RealWorld.Width;
        int mapHeight = WorldState.RealWorld.Height;

        // Move camera
        Transform transform = _camera.Transform;

        // ORTHOGONAL CAMERA
        int maxWH = Math.Max(mapWidth, mapHeight);

        transform.origin = new Vector3(mapWidth / 2.0f, maxWH, mapHeight / 2.0f);
        transform.origin += new Vector3(maxWH, maxWH, maxWH);
        transform = transform.LookingAt(new Vector3(mapWidth / 2.0f, 0, mapHeight / 2.0f), new Vector3(0, 1, 0));
        _camera.Size = 3 + maxWH * 1.2f;

        _camera.Transform = transform;
    }

    public void UpdateWith(WorldState beliefs)
    {
        _visibleMap = _startingVisibleMap;
        _invisibleMap = _startingInvisibleMap;
        _beliefs = beliefs;

        GD.Print("Visible map childs: " + _visibleMap.GetChildCount());
        GD.Print("Invisible map childs: " + _invisibleMap.GetChildCount());

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
                            if (door.Solid)
                            {
                                instance = (Spatial)DOOR.Instance();
                            }
                            else
                            {
                                instance = (Spatial)DOOR_OPEN.Instance();
                            }

                            if (DoorShouldBeRotated(x, y))
                                rotation.y = Mathf.Pi / 2.0f;
                        }
                        else if (entity is Package)
                        {
                            instance = (Spatial)PACKAGE.Instance();
                        }
                        else if (entity is DeliverySpot)
                        {
                            instance = (Spatial)DELIVERY_SPOT.Instance();
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
                                if (door.Solid)
                                {
                                    instance = (Spatial)DOOR_GHOST.Instance();
                                }
                                else
                                {
                                    instance = (Spatial)DOOR_OPEN_GHOST.Instance();
                                }

                                if (DoorShouldBeRotated(x, y))
                                    rotation.y = Mathf.Pi / 2.0f;
                            }
                            else if (entity is Package)
                            {
                                instance = (Spatial)PACKAGE_GHOST.Instance();
                            }
                            else if (entity is DeliverySpot)
                            {
                                instance = (Spatial)DELIVERY_SPOT_GHOST.Instance();
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