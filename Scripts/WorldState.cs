using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;


public class WorldState {
    private List<List<List<Entity>>> _map;

    public WorldState (string bitmapPath) {
        Bitmap img = new Bitmap(bitmapPath);

        _map = new List<List<List<Entity>>>(img.Width);
        for (int x = 0; x < img.Width; x++) {
            _map[x] = new List<List<Entity>>(img.Height);
            for (int y = 0; y < img.Height; y++) {
                _map[x][y] = new List<Entity>();
                Entity entity = EntityFactory.New(img.GetPixel(x, y));
                if (entity != null) 
                    _map[x][y].Add(entity);   
            }
        }
    }

    public Spatial ToSpatial () {
        Spatial scene = new Spatial();

        for (int x = 0; x < _map.Count; x++) {
            for (int y = 0; y < _map[x].Count; y++) {
                for (int z = 0; z < _map[x][y].Count; z++) {
                    Entity entity = _map[x][y][z];
                    if (entity is Wall) {
                        //TODO: Print entity
                    }
                }
            }
        }
        
        return scene;
    }
    
}