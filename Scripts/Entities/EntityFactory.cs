using System;
using System.Drawing;

public class EntityFactory
{
    public static int id = 0;

    public static Entity New(string type)
    {

        Entity entity = null;
        switch (type.ToLower())
        {
            case "wall": // Wall
                entity = new Wall("Wall;" + id++, 0, 0);
                break;
            case "floor": // Floor
                entity = new Floor("Floor;" + id++, 0, 0);
                break;
            case "door": // Door
                entity = new Door("Door;" + id++, 0, 0);
                break;
            case "agent": // Agent
                entity = new Agent("Agent;" + id++, 0, 0);
                break;
            case "package": // Package
                entity = new Package("Package;" + id++, 0, 0);
                break;
            case "delivery spot": // Delivery spot
                entity = new DeliverySpot("Delivery spot;" + id++, 0, 0);
                break;
            default:
                break;
        }
        return entity;
    }

    public static Entity New(Color c)
    {
        string pixelHexValue = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        System.Console.WriteLine(pixelHexValue);

        Entity entity = null;
        switch (pixelHexValue)
        {
            case "#000000": // Wall
                entity = new Wall("Wall;" + id++, 0, 0);
                break;
            case "#FFFFFF": // Floor
                entity = new Floor("Floor;" + id++, 0, 0);
                break;
            case "#FF0000": // Door
                entity = new Door("Door;" + id++, 0, 0);
                break;
            case "#00FF00": // Agent
                entity = new Agent("Agent;" + id++, 0, 0);
                break;
            case "#FF00FF": // Package
                entity = new Package("Package;" + id++, 0, 0);
                break;
            case "#00FFFF": // Delivery spot
                entity = new DeliverySpot("Delivery spot;" + id++, 0, 0);
                break;
            default:
                break;
        }
        return entity;
    }
}