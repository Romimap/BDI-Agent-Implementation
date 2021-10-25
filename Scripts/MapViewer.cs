using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MapViewer : Spatial
{
    private Spatial _visibleMap;
    private Spatial _invisibleMap;
    private Camera _camera;

    private List<List<Dictionary<string, Spatial>>> _visibleInstances;
    private List<List<Dictionary<string, Spatial>>> _invisibleInstances;
    private Dictionary<string, Spatial> _agentInstances;

    private static MapViewer _singletonInstance;

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
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        _visibleMap = (Spatial)visibleMap;
        _invisibleMap = (Spatial)invisibleMap;
        _camera = (Camera)camera;

        _visibleInstances = new List<List<Dictionary<string, Spatial>>>();
        _invisibleInstances = new List<List<Dictionary<string, Spatial>>>();
        _agentInstances = new Dictionary<string, Spatial>();

        InitCamera();
        InitMap();

        _singletonInstance = this;

        stopwatch.Stop();
        Console.WriteLine("MapViewer creation done in: " + (stopwatch.ElapsedMilliseconds / 1000.0f) + "s");
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
            _visibleInstances.Add(new List<Dictionary<string, Spatial>>());
            _invisibleInstances.Add(new List<Dictionary<string, Spatial>>());

            for (int y = 0; y < height; y++)
            {
                _visibleInstances[x].Add(new Dictionary<string, Spatial>());
                _invisibleInstances[x].Add(new Dictionary<string, Spatial>());

                Dictionary<string, Entity> entities = WorldState.RealWorld.GetEntitiesAt(x, y);
                if (entities != null && entities.Count != 0) // Draw normal objects (known world)
                {
                    foreach (KeyValuePair<string, Entity> kvp in entities)
                    {
                        Entity entity = kvp.Value;
                        Spatial visibleInstance = null;
                        Spatial invisibleInstance = null;
                        Vector3 rotation = new Vector3(0, 0, 0);

                        if (entity is Agent)
                        {
                            Spatial agentInstance = (Spatial)AGENT.Instance();

                            MoveInstance(agentInstance, new Vector3(x, 0, y), rotation);
                            _visibleMap.AddChild(agentInstance);
                            _agentInstances.Add(entity.Name, agentInstance);
                        }
                        else if (entity is Door)
                        {
                            if (DoorShouldBeRotated(x, y))
                            {
                                rotation.y = Mathf.Pi / 2.0f;
                            }

                            Spatial instance = (Spatial)DOOR.Instance();
                            instance.Visible = false;
                            MoveAndAdd(instance, entity.Name, x, y, rotation, _visibleMap, _visibleInstances);

                            instance = (Spatial)DOOR_OPEN.Instance();
                            instance.Visible = false;
                            MoveAndAdd(instance, entity.Name + ";open", x, y, rotation, _visibleMap, _visibleInstances);

                            instance = (Spatial)DOOR_GHOST.Instance();
                            MoveAndAdd(instance, entity.Name, x, y, rotation, _invisibleMap, _invisibleInstances);

                            instance = (Spatial)DOOR_OPEN_GHOST.Instance();
                            instance.Visible = false;
                            MoveAndAdd(instance, entity.Name + ";open", x, y, rotation, _invisibleMap, _invisibleInstances);
                        }
                        else if (entity is Floor)
                        {
                            visibleInstance = (Spatial)FLOOR.Instance();
                            invisibleInstance = (Spatial)FLOOR_GHOST.Instance();
                        }
                        else if (entity is Wall)
                        {
                            visibleInstance = (Spatial)WALL.Instance();
                            invisibleInstance = (Spatial)WALL_GHOST.Instance();
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
                            _visibleMap.AddChild(visibleInstance);
                            _visibleInstances[x][y].Add(entity.Name, visibleInstance);
                        }
                        if (invisibleInstance != null)
                        {
                            MoveInstance(invisibleInstance, new Vector3(x, 0, y), rotation);
                            _invisibleMap.AddChild(invisibleInstance);
                            _invisibleInstances[x][y].Add(entity.Name, invisibleInstance);
                        }
                    }
                }
            }
        }
    }

    private void MoveAndAdd(Spatial instance, string name, int x, int y, Vector3 rotation, Spatial map, List<List<Dictionary<string, Spatial>>> instances)
    {
        MoveInstance(instance, new Vector3(x, 0, y), rotation);
        map.AddChild(instance);
        instances[x][y].Add(name, instance);
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
        Dictionary<string, Spatial> invisibleEntities = _invisibleInstances[x][y];
        foreach (KeyValuePair<string, Spatial> kvp in invisibleEntities)
        {
            Spatial instance = kvp.Value;
            instance.Visible = false;
        }

        Dictionary<string, Spatial> visibleEntities = _visibleInstances[x][y];
        foreach (KeyValuePair<string, Spatial> kvp in visibleEntities)
        {
            string instanceName = kvp.Key;
            Spatial instance = kvp.Value;
            instance.Visible = true;

            if (instanceName.Contains("Door"))
            {
                OpenCloseVisualDoor(instanceName, instance, x, y);
            }
        }
    }

    private void OpenCloseVisualDoor(string instanceName, Spatial instance, int x, int y)
    {
        foreach (KeyValuePair<string, Entity> kvp2 in WorldState.RealWorld.GetEntitiesAt(x, y))
        {
            string entityName = kvp2.Key;
            Entity entity = kvp2.Value;

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

    private void ChangeAgentVisualPos(Agent agent)
    {
        foreach (KeyValuePair<string, Spatial> kvp in _agentInstances)
        {
            string agentName = kvp.Key;
            if (agent.Name == agentName) {
                Spatial agentInstance = kvp.Value;
                (agentInstance as AgentNode).MoveTo(new Vector3(agent.X, agentInstance.Translation.y, agent.Y));
            }
        }
    }

    public static void ChangeVisibility(Agent agent)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        MapViewer MV = MapViewer._singletonInstance;
        MV.ChangeAgentVisualPos(agent);

        int minX = Math.Max(agent.X - agent.VisionRange, 0);
        int maxX = Math.Min(agent.X + agent.VisionRange, WorldState.RealWorld.Width - 1);

        int minY = Math.Max(agent.Y - agent.VisionRange, 0);
        int maxY = Math.Min(agent.Y + agent.VisionRange, WorldState.RealWorld.Height - 1);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                MV.ChangeVisibility(x, y);
            }
        }

        stopwatch.Stop();
        Console.WriteLine("MapViewer update done in: " + stopwatch.ElapsedMilliseconds + "ms");
    }
}