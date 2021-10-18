using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;


public class WorldState {
    public static WorldState RealWorld = null;
    private Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();
    private List<List<Dictionary<string, Entity>>> _map;
    private Dictionary<string, Agent> _agents = new Dictionary<string, Agent>();
    public Dictionary<string, Agent> Agents {get{return _agents;}}
    private Dictionary<string, ActionEntity> _actionEntities = new Dictionary<string, ActionEntity>();
    public int Width {get{return _map.Count;}}
    public int Height {get{return _map[0].Count;}}

    public WorldState (string bitmapPath) {
        if (RealWorld == null) RealWorld = this;
        
        Bitmap img = new Bitmap(bitmapPath);

        _map = new List<List<Dictionary<string, Entity>>>(img.Width);
        for (int x = 0; x < img.Width; x++) {
            _map.Add(new List<Dictionary<string, Entity>>(img.Height));
            for (int y = 0; y < img.Height; y++) {
                _map[x].Add(new Dictionary<string, Entity>());
                Entity entity = EntityFactory.New(img.GetPixel(x, y));
                if (entity != null) { 
                    _map[x][y].Add(entity.Name, entity);
                    entity.SetCoord(x, y);
                }
                if (entity is Agent) {
                    _agents.Add(entity.Name, (Agent)entity);
                }

                if (entity is ActionEntity) {
                    _actionEntities.Add(entity.Name, (ActionEntity)entity);
                }
            }
        }

        Init();
    }


    private WorldState (WorldState from) {
         _map = new List<List<Dictionary<string, Entity>>>(from._map.Count);
        for (int x = 0; x < from._map.Count; x++) {
            _map.Add(new List<Dictionary<string, Entity>>(from._map[x].Count));
            for (int y = 0; y < from._map[x].Count; y++) {
                _map[x].Add(new Dictionary<string, Entity>());
                foreach (KeyValuePair<string, Entity> kvp in from._map[x][y]) {
                    Entity clone = kvp.Value.Clone();
                    _map[x][y].Add(clone.Name, clone);
                    clone.SetCoord(x, y);

                    if (clone is Agent) {
                        _agents.Add(clone.Name, (Agent)clone);
                    }

                    if (clone is ActionEntity) {
                        _actionEntities.Add(clone.Name, (ActionEntity)clone);
                    }
                }
            }
        }
    }

    private WorldState (WorldState from, int xPos, int yPos, int radius) {
        _map = new List<List<Dictionary<string, Entity>>>(from._map.Count);
        for (int x = 0; x < from._map.Count; x++) {
            _map.Add(new List<Dictionary<string, Entity>>(from._map[x].Count));
            for (int y = 0; y < from._map[x].Count; y++) {

                //Only fill blocs that are less than R from xPos, yPos
                if (Mathf.Abs(x - xPos) <= radius && Mathf.Abs(y - yPos) <= radius) {
                    _map[x].Add(new Dictionary<string, Entity>());
                    foreach (KeyValuePair<string, Entity> kvp in from._map[x][y]) {
                        Entity clone = kvp.Value.Clone();
                        _map[x][y].Add(clone.Name, clone);
                        clone.SetCoord(x, y);
                    
                        if (clone is Agent) {
                            _agents.Add(clone.Name, (Agent)clone);
                        }

                        if (clone is ActionEntity) {
                            _actionEntities.Add(clone.Name, (ActionEntity)clone);
                        }
                    }
                } else {
                    _map[x].Add(null);
                }
            }
        }
    }

    private WorldState (int width, int height) {
        _map = new List<List<Dictionary<string, Entity>>>(width);
        for (int x = 0; x < width; x++) {
            _map.Add(new List<Dictionary<string, Entity>>(height));
            for (int y = 0; y < height; y++) {
                _map[x].Add(new Dictionary<string, Entity>());
            }
        }
    }

    private void Init () {
        foreach (KeyValuePair<string, Agent> kvp in _agents) {
            kvp.Value.Init();
        }
    }

    public void Tick () {
        GD.Print("TICK");
        GD.Print(_agents);
        foreach(KeyValuePair<string, Agent> kvp in _agents) {
            GD.Print(kvp);
            GD.Print(kvp.Value);
            kvp.Value.Tick();
        }
    }

    public static WorldState DefaultBelief () {
        return new WorldState(RealWorld._map.Count, RealWorld._map[0].Count);
    }

    public List<Coord> Do (Agent agent, Action action) {
        if (!_actionEntities.ContainsKey(action._entityName)) return null;
        ActionEntity actionEntity = _actionEntities[action._entityName];
        List<Coord> path = PathFind(agent, action);
        if (path == null) return null;

        Coord end = path[path.Count - 1];
        Move(agent, end);

        actionEntity.Do(agent, action);
        return path;
    }

    public List<Coord> PathFind (Agent agent, Action action) {
        if (!_actionEntities.ContainsKey(action._entityName)) return null;
        
        ActionEntity actionEntity = _actionEntities[action._entityName];
        Coord start = agent.GetCoord();
        Coord end = actionEntity.GetCoord();


        List<Coord> ans = PathFinding.PathFind(this, start, end);

        return ans;
    }

    public bool PathObstructed (List<Coord> path) {
        foreach(Coord coord in path) {
            foreach(KeyValuePair<string, Entity> kvp in _map[coord.X][coord.Y]) {
                if (kvp.Value.Solid) return true;
            }
        }

        return false;
    }

    public WorldState Percept (int x, int y, int r) {
        return new WorldState(RealWorld, x, y, r);
    }

    public WorldState Clone () {
        return new WorldState(this);
    }

    public Coord Move (Entity entity, Coord coord) {
        if (!_entities.ContainsKey(entity.Name)) return new Coord(-1, -1);
        Entity toMove = _entities[entity.Name];
        _map[entity.X][entity.Y].Remove(toMove.Name);
        _map[coord.X][coord.Y].Add(toMove.Name, toMove);
        toMove.SetCoord(coord);
        return coord;
    }
    public Coord Move (Entity entity, int x, int y) {
        if (!_entities.ContainsKey(entity.Name)) return new Coord(-1, -1);
        Entity toMove = _entities[entity.Name];
        _map[entity.X][entity.Y].Remove(toMove.Name);
        _map[x][y].Add(toMove.Name, toMove);
        toMove.SetCoord(new Coord(x, y));
        return new Coord(x, y);
    }

    public Coord GetCoord (Entity entity) {
        if (!_entities.ContainsKey(entity.Name)) return new Coord(-1, -1);
        Entity getEntity = _entities[entity.Name];
        return getEntity.GetCoord();
    }

    public List<Action> GetActions () {
        List<Action> ans = new List<Action>();

        foreach (KeyValuePair<string, ActionEntity> kvp in _actionEntities) {
            ActionEntity actionEntity = kvp.Value;
            foreach(string actionName in actionEntity.GetActionNames()) {
                Action action = new Action(actionEntity.Name, actionName);
                ans.Add(action);
            }
        }

        return ans;
    }

    public int AddPercept (WorldState percept) {
        int deltaBeliefs = 0;
        for (int x = 0; x < _map.Count; x++) {
            for (int y = 0; y < _map[x].Count; y++) {
                if (percept._map[x][y] != null) {
                    //Something new
                    foreach (KeyValuePair<string, Entity> kvp in percept._map[x][y]) {
                        if (!_map[x][y].ContainsKey(kvp.Key)) {
                            if (_entities.ContainsKey(kvp.Key)) { //Something moved Here
                                Move(_entities[kvp.Key], x, y);
                            } else { //A new object is discovered
                                Entity clone = kvp.Value.Clone();

                                _map[x][y].Add(clone.Name, clone);
                                _entities.Add(clone.Name, clone);
                            }
                            deltaBeliefs++;

                        }
                    }

                    //Something missing
                    foreach (KeyValuePair<string, Entity> kvp in _map[x][y]) {
                         if (!percept._map[x][y].ContainsKey(kvp.Key)) {
                            _map[x][y].Remove(kvp.Key);
                            _entities.Remove(kvp.Key);
                            if (kvp.Key is ActionEntity)
                                _actionEntities.Remove(kvp.Key);
                            deltaBeliefs++;
                        }
                    }

                }
            }
        }
        return deltaBeliefs;
    }

    public Spatial ToSpatial () {
        Spatial scene = new Spatial();

        for (int x = 0; x < _map.Count; x++) {
            for (int y = 0; y < _map[x].Count; y++) {
                foreach (KeyValuePair<string, Entity> kvp in _map[x][y]) {
                    Entity e = kvp.Value;

                    //TODO

                }
            }
        }
        
        return scene;
    }

    public override string ToString() {
        string ans = "";
        for (int x = 0; x < _map.Count; x++) {
            for (int y = 0; y < _map[x].Count; y++) {
                if (_map[x][y] != null && _map[x][y].Count > 0) {
                    foreach(KeyValuePair<string, Entity> kvp in _map[x][y]) {
                        if (kvp.Value is Agent) {
                            ans += "A";
                        } else if (kvp.Value is Floor) {
                            ans += ".";
                        } else if (kvp.Value is Wall) {
                            ans += "#";
                        } else {
                            ans += "?";
                        }
                    }
                } else {
                    ans += " ";
                }
                ans += " ";
            }
            ans += "\n";
        }

        return ans;
    }
}