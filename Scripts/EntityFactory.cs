using System;
using System.Drawing;

public class EntityFactory {
    public static int id = 0;

    public static Entity New(Color c) {
        string pixelHexValue = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        System.Console.WriteLine(pixelHexValue);
        
        Entity entity = null;
        switch (pixelHexValue) {
            case "#000000": // Wall
                entity = new Wall("Wall;" + id++, 0, 0);
                break;
            case "#FFFFFF": // Floor
                entity = new Floor("Floor;" + id++, 0, 0);
                break;
            case "#FF0000": // Door
                break;
            case "#00FF00": // Agent
                entity = new Agent("Agent" + id++, 0, 0);
                break;
            case "#FF00FF": // Package
                break;
            case "#00FFFF": // Delivery spot
                break;
            default:
                break;
        }
        return entity;
    }
}