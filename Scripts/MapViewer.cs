using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MapViewer : Spatial
{
    private Spatial _visibleMap;
    private Spatial _invisibleMap;
    private Camera _camera;

    // private List<List<Dictionary<string, Spatial>>> _visibleInstances;
    // private List<List<Dictionary<string, Spatial>>> _invisibleInstances;
    // private Dictionary<string, Spatial> _agentInstances;
    // private Dictionary<string, Spatial> _packageInstances;

    private static MapViewer _singletonInstance;

    // Godot map entities prefabs
    public static PackedScene AGENT = (PackedScene)ResourceLoader.Load("res://Entities/Agent.tscn");
    public static PackedScene WALL = (PackedScene)ResourceLoader.Load("res://Entities/Wall.tscn");
    public static PackedScene WALL_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Wall (ghost).tscn");
    public static PackedScene FLOOR = (PackedScene)ResourceLoader.Load("res://Entities/Floor.tscn");
    public static PackedScene FLOOR_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Floor (ghost).tscn");
    public static PackedScene DOOR = (PackedScene)ResourceLoader.Load("res://Entities/Door.tscn");
    public static PackedScene DOOR_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Door (ghost).tscn");
    public static PackedScene DOOR_OPEN = (PackedScene)ResourceLoader.Load("res://Entities/Door (open).tscn");
    public static PackedScene DOOR_OPEN_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Door (open ghost).tscn");
    public static PackedScene PACKAGE = (PackedScene)ResourceLoader.Load("res://Entities/Package.tscn");
    public static PackedScene PACKAGE_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Package (ghost).tscn");
    public static PackedScene DELIVERY_SPOT = (PackedScene)ResourceLoader.Load("res://Entities/Delivery spot.tscn");
    public static PackedScene DELIVERY_SPOT_GHOST = (PackedScene)ResourceLoader.Load("res://Entities/Delivery spot (ghost).tscn");


    public MapViewer(Node visibleMap, Node invisibleMap, Node camera)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        _visibleMap = (Spatial)visibleMap;
        _invisibleMap = (Spatial)invisibleMap;
        _camera = (Camera)camera;

        // _visibleInstances = new List<List<List<Spatial>>>();
        // _invisibleInstances = new List<List<List<Spatial>>>();
        // _agentInstances = new Dictionary<string, Spatial>();
        // _packageInstances = new Dictionary<string, Spatial>();

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
            for (int y = 0; y < height; y++)
            {
                foreach (KeyValuePair<string, Entity> kvp in WorldState.RealWorld.Map[x][y])
                {
                    Entity entity = kvp.Value;
                    if (entity.Visuals != null)
                    {
                        if (entity.Visuals.VisibleInstance != null) _visibleMap.AddChild(entity.Visuals.VisibleInstance);
                        if (entity.Visuals.GhostInstance != null) _invisibleMap.AddChild(entity.Visuals.GhostInstance);
                        if (entity.Visuals.VisibleActionnedInstance != null) _visibleMap.AddChild(entity.Visuals.VisibleActionnedInstance);
                        if (entity.Visuals.GhostActionnedInstance != null) _visibleMap.AddChild(entity.Visuals.GhostActionnedInstance);
                    }
                }
            }
        }
    }

    // private void InitMap()
    // {
    //     int width = WorldState.RealWorld.Width;
    //     int height = WorldState.RealWorld.Height;

    //     for (int x = 0; x < width; x++)
    //     {
    //         _visibleInstances.Add(new List<Dictionary<string, Spatial>>());
    //         _invisibleInstances.Add(new List<Dictionary<string, Spatial>>());

    //         for (int y = 0; y < height; y++)
    //         {
    //             _visibleInstances[x].Add(new Dictionary<string, Spatial>());
    //             _invisibleInstances[x].Add(new Dictionary<string, Spatial>());

    //             Dictionary<string, Entity> entities = WorldState.RealWorld.GetEntitiesAt(x, y);
    //             if (entities != null && entities.Count != 0) // Draw normal objects (known world)
    //             {
    //                 foreach (KeyValuePair<string, Entity> kvp in entities)
    //                 {
    //                     Entity entity = kvp.Value;
    //                     Spatial visibleInstance = null;
    //                     Spatial invisibleInstance = null;
    //                     Vector3 rotation = new Vector3(0, 0, 0);

    //                     if (entity is Agent)
    //                     {
    //                         Spatial agentInstance = (Spatial)AGENT.Instance();
    //                         MoveRotateAndAdd(agentInstance, entity.Name, x, y, rotation, _visibleMap, _agentInstances);
    //                     }
    //                     else if (entity is Door)
    //                     {
    //                         if (Door.ShouldBeRotated(x, y))
    //                         {
    //                             rotation.y = Mathf.Pi / 2.0f;
    //                         }

    //                         Spatial instance = (Spatial)DOOR.Instance();
    //                         instance.Visible = false;
    //                         MoveRotateAndAdd(instance, entity.Name, x, y, rotation, _visibleMap, _visibleInstances);

    //                         instance = (Spatial)DOOR_OPEN.Instance();
    //                         instance.Visible = false;
    //                         MoveRotateAndAdd(instance, entity.Name + ";open", x, y, rotation, _visibleMap, _visibleInstances);

    //                         instance = (Spatial)DOOR_GHOST.Instance();
    //                         MoveRotateAndAdd(instance, entity.Name, x, y, rotation, _invisibleMap, _invisibleInstances);

    //                         instance = (Spatial)DOOR_OPEN_GHOST.Instance();
    //                         instance.Visible = false;
    //                         MoveRotateAndAdd(instance, entity.Name + ";open", x, y, rotation, _invisibleMap, _invisibleInstances);
    //                     }
    //                     else if (entity is Floor)
    //                     {
    //                         visibleInstance = (Spatial)FLOOR.Instance();
    //                         invisibleInstance = (Spatial)FLOOR_GHOST.Instance();
    //                     }
    //                     else if (entity is Wall)
    //                     {
    //                         visibleInstance = (Spatial)WALL.Instance();
    //                         invisibleInstance = (Spatial)WALL_GHOST.Instance();
    //                     }
    //                     else if (entity is Package)
    //                     {
    //                         visibleInstance = (Spatial)PACKAGE.Instance();
    //                         invisibleInstance = (Spatial)PACKAGE_GHOST.Instance();
    //                     }
    //                     else if (entity is DeliverySpot)
    //                     {
    //                         visibleInstance = (Spatial)DELIVERY_SPOT.Instance();
    //                         invisibleInstance = (Spatial)DELIVERY_SPOT_GHOST.Instance();
    //                     }

    //                     if (visibleInstance != null)
    //                     {
    //                         visibleInstance.Visible = false;
    //                         MoveRotateAndAdd(visibleInstance, entity.Name, x, y, rotation, _visibleMap, _visibleInstances);
    //                     }
    //                     if (invisibleInstance != null)
    //                     {
    //                         MoveRotateAndAdd(invisibleInstance, entity.Name, x, y, rotation, _invisibleMap, _invisibleInstances);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }

    // private void MoveAndRotate(Spatial instance, Vector3 translation, Vector3 rotation)
    // {
    //     instance.Translate(translation);
    //     instance.RotateX(rotation.x);
    //     instance.RotateY(rotation.y);
    //     instance.RotateZ(rotation.z);
    // }

    // private void MoveRotateAndAdd(Spatial instance, string name, int x, int y, Vector3 rotation, Spatial map, List<List<Dictionary<string, Spatial>>> instances)
    // {
    //     MoveAndRotate(instance, new Vector3(x, 0, y), rotation);
    //     map.AddChild(instance);
    //     instances[x][y].Add(name, instance);
    // }

    // private void MoveRotateAndAdd(Spatial instance, string name, int x, int y, Vector3 rotation, Spatial map, Dictionary<string, Spatial> instances)
    // {
    //     MoveAndRotate(instance, new Vector3(x, 0, y), rotation);
    //     map.AddChild(instance);
    //     instances.Add(name, instance);
    // }

    private void UpdateTile(int x, int y)
    {
        Dictionary<string, Entity> entities = WorldState.RealWorld.GetEntitiesAt(x, y);
        foreach (KeyValuePair<string, Entity> kvp in entities)
        {
            Entity entity = kvp.Value;
            if (entity.Visuals != null)
                entity.Visuals.SetVisible(true);
        }

        // Dictionary<string, Spatial> visibleEntities = _visibleInstances[x][y];
        // foreach (KeyValuePair<string, Spatial> kvp in visibleEntities)
        // {
        //     string instanceName = kvp.Key;
        //     Spatial instance = kvp.Value;
        //     instance.Visible = true;

        //     if (instanceName.ToLower().Contains("door"))
        //     {
        //         // DoorVisuals.UpdateState(instanceName, instance, x, y);
        //     }
        // }
    }

    public static void UpdateTilesAround(Agent agent)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        MapViewer MV = MapViewer._singletonInstance;
        // AgentVisuals.ChangePosition(agent, MV._agentInstances);

        int minX = Math.Max(agent.X - agent.VisionRange, 0);
        int maxX = Math.Min(agent.X + agent.VisionRange, WorldState.RealWorld.Width - 1);

        int minY = Math.Max(agent.Y - agent.VisionRange, 0);
        int maxY = Math.Min(agent.Y + agent.VisionRange, WorldState.RealWorld.Height - 1);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                MV.UpdateTile(x, y);
            }
        }

        stopwatch.Stop();
        Console.WriteLine("MapViewer update done in: " + stopwatch.ElapsedMilliseconds + "ms");
    }

    // public static Spatial GetPackageInstanceAt(int x, int y)
    // {
    //     MapViewer MV = MapViewer._singletonInstance;
    //     foreach (KeyValuePair<string, Spatial> kvp in MV._visibleInstances[x][y])
    //     {
    //         if (kvp.Key.ToLower().Contains("package"))
    //         {
    //             return kvp.Value;
    //         }
    //     }
    //     return null;
    // }

    // public static Spatial GetAgentInstance(Agent agent)
    // {
    //     MapViewer MV = MapViewer._singletonInstance;
    //     foreach (KeyValuePair<string, Spatial> kvp in MV._agentInstances)
    //     {
    //         if (kvp.Key == agent.Name)
    //         {
    //             return kvp.Value;
    //         }
    //     }
    //     return null;
    // }

    // public static Spatial CreateInstance(PackedScene packedScene, Vector3 coordinates)
    // {
    //     Spatial instance = (Spatial)packedScene.Instance();
    //     instance.Translate(coordinates);
    //     return instance;
    // }

    // public static Spatial CreateInstance(PackedScene packedScene, Vector3 coordinates, Vector3 rotation)
    // {
    //     Spatial instance = (Spatial)packedScene.Instance();
    //     instance.Translate(coordinates);
    //     instance.RotateX(rotation.x);
    //     instance.RotateY(rotation.y);
    //     instance.RotateZ(rotation.z);
    //     return instance;
    // }
}