using System;
using System.Drawing;

public class EntityFactory {

    public static Entity New(Color c) {
        string pixelHexValue = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        Entity entity = null;
        switch (pixelHexValue) {
            case "#000000": // Wall
                break;
            case "#FFFFFF": // Floor
                break;
            case "#FF0000": // Door
                break;
            case "#00FF00": // Agent
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