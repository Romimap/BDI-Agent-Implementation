using Godot;
using System;
using System.Collections.Generic;

public class MapViewer : Spatial
{
    private Spatial _visibleMap;
    private Spatial _invisibleMap;
    private Camera _camera;

    private List<List<Dictionary<string, Spatial>>> visibleInstances;
    private List<List<Dictionary<string, Spatial>>> invisibleInstances;

    private static MapViewer singletonInstance;

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
        _visibleMap = (Spatial)visibleMap;
        _invisibleMap = (Spatial)invisibleMap;
        _camera = (Camera)camera;

        visibleInstances = new List<List<Dictionary<string, Spatial>>>();
        invisibleInstances = new List<List<Dictionary<string, Spatial>>>();

        InitCamera();
        InitMap();

        singletonInstance = this;
    }

    private void InitCamera()
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

    private void InitMap()
    {
        int width = WorldState.RealWorld.Width;
        int height = WorldState.RealWorld.Height;

        for (int x = 0; x < width; x++)
        {
            visibleInstances.Add(new List<Dictionary<string, Spatial>>());
            invisibleInstances.Add(new List<Dictionary<string, Spatial>>());

            for (int y = 0; y < height; y++)
            {
                visibleInstances[x].Add(new Dictionary<string, Spatial>());
                invisibleInstances[x].Add(new Dictionary<string, Spatial>());

                Dictionary<string, Entity> entities = WorldState.RealWorld.GetEntitiesAt(x, y);
                if (entities != null && entities.Count != 0) // Draw normal objects (known world)
                {
                    foreach (KeyValuePair<string, Entity> kvp in entities)
                    {
                        Entity entity = kvp.Value;
                        Spatial visibleInstance = null;
                        Spatial invisibleInstance = null;
                        Vector3 rotation = new Vector3(0, 0, 0);

                        if (entity is Floor)
                        {
                            visibleInstance = (Spatial)FLOOR.Instance();
                            invisibleInstance = (Spatial)FLOOR_GHOST.Instance();
                        }
                        else if (entity is Wall)
                        {
                            visibleInstance = (Spatial)WALL.Instance();
                            invisibleInstance = (Spatial)WALL_GHOST.Instance();
                        }
                        else if (entity is Agent)
                        {
                            visibleInstance = (Spatial)AGENT.Instance();
                        }
                        else if (entity is Door)
                        {
                            if (entity.Solid)
                            {
                                visibleInstance = (Spatial)DOOR.Instance();
                                invisibleInstance = (Spatial)DOOR_GHOST.Instance();
                            }
                            else
                            {
                                visibleInstance = (Spatial)DOOR_OPEN.Instance();
                                invisibleInstance = (Spatial)DOOR_OPEN_GHOST.Instance();
                            }

                            if (DoorShouldBeRotated(x, y))
                            {
                                rotation.y = Mathf.Pi / 2.0f;
                            }
                        }
                        else if (entity is Package)
                        {
                            visibleInstance = (Spatial)PACKAGE.Instance();
                            invisibleInstance = (Spatial)PACKAGE_GHOST.Instance();
                        }
                        else if (entity is DeliverySpot)
                        {
                            visibleInstance = (Spatial)DELIVERY_SPOT.Instance();
                            invisibleInstance = (Spatial)DELIVERY_SPOT_GHOST.Instance();
                        }

                        if (visibleInstance != null)
                        {
                            visibleInstance.Visible = false;

                            MoveInstance(visibleInstance, new Vector3(x, 0, y), rotation);
                            MoveInstance(invisibleInstance, new Vector3(x, 0, y), rotation);

                            _visibleMap.AddChild(visibleInstance);
                            _invisibleMap.AddChild(invisibleInstance);

                            visibleInstances[x][y].Add(entity.Name, visibleInstance);
                            invisibleInstances[x][y].Add(entity.Name, invisibleInstance);
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

    private void ChangeVisibility(int x, int y)
    {

        Dictionary<string, Spatial> invisibleEntities = invisibleInstances[x][y];
        foreach (KeyValuePair<string, Spatial> kvp in invisibleEntities)
        {
            Spatial instance = kvp.Value;
            instance.Visible = false;
        }

        Dictionary<string, Spatial> visibleEntities = visibleInstances[x][y];
        foreach (KeyValuePair<string, Spatial> kvp in visibleEntities)
        {
            Spatial instance = kvp.Value;
            instance.Visible = true;
        }
    }

    public static void ChangeVisibility(int fromX, int fromY, int visibilityRange)
    {
        int minX = Math.Max(fromX - visibilityRange, 0);
        int maxX = Math.Min(fromX + visibilityRange, WorldState.RealWorld.Width - 1);

        int minY = Math.Max(fromY - visibilityRange, 0);
        int maxY = Math.Min(fromY + visibilityRange, WorldState.RealWorld.Height - 1);

        MapViewer MV = MapViewer.singletonInstance;
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                MV.ChangeVisibility(x, y);
            }
        }
    }
}