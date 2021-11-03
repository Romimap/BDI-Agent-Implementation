using System;
using System.Drawing;

public class EntityFactory
{
    public static int id = 0;

    public static Entity New(string type, int x, int y)
    {

        Entity entity = null;
        switch (type.ToLower())
        {
            case "wall": // Wall
                entity = new Wall("Wall;" + id++, x, y);
                break;
            case "floor": // Floor
                entity = new Floor("Floor;" + id++, x, y);
                break;
            case "door": // Door
                entity = new Door("Door;" + id++, x, y);
                break;
            case "agent": // Agent
                entity = new Agent("Agent;" + id++, x, y);
                break;
            case "package": // Package
                entity = new Package("Package;" + id++, x, y);
                break;
            case "delivery spot": // Delivery spot
                entity = new DeliverySpot("Delivery spot;" + id++, x, y);
                break;
            default:
                break;
        }
        return entity;
    }

    public static Entity New(Color c, int x, int y)
    {
        string pixelHexValue = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        Entity entity = null;
        switch (pixelHexValue)
        {
            case "#000000": // Wall
                entity = new Wall("Wall;" + id++, x, y);
                break;
            case "#FFFFFF": // Floor
                entity = new Floor("Floor;" + id++, x, y);
                break;
            case "#FF0000": // Door
                entity = new Door("Door;" + id++, x, y);
                break;
            case "#00FF00": // Agent
                entity = new Agent("Agent;" + id++, x, y);
                break;
            case "#FF00FF": // Package
                entity = new Package("Package;" + id++, x, y);
                break;
            case "#00FFFF": // Delivery spot
                entity = new DeliverySpot("Delivery spot;" + id++, x, y);
                break;
            default:
                break;
        }
        return entity;
    }
}