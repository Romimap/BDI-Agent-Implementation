using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MapViewer : Spatial
{
    private Spatial _visibleMap;
    private Spatial _invisibleMap;
    private Camera _camera;

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

    private void AnimateAgent(Agent agent)
    {
        Spatial agentInstance = agent.Visuals.VisibleInstance;
        (agentInstance as AgentNode).MoveTo(new Vector3(agent.X, agentInstance.Translation.y, agent.Y));
    }

    private void UpdateTile(int x, int y)
    {
        Dictionary<string, Entity> entities = WorldState.RealWorld.GetEntitiesAt(x, y);
        foreach (KeyValuePair<string, Entity> kvp in entities)
        {
            Entity entity = kvp.Value;
            if (entity.Visuals != null)
                entity.Visuals.SetVisible(true);
        }
    }

    public static void UpdateTilesAround(Agent agent)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        MapViewer MV = MapViewer._singletonInstance;
        MV.AnimateAgent(agent);

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
    }
}