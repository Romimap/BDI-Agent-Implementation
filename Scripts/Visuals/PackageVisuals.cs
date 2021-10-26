// using Godot;
// using System;
// using System.Collections.Generic;

// public class PackageVisuals
// {
//     private static Entity packageEntity;
//     private static Spatial packageInstance;

//     public static bool AgentPocketHasChanged(Agent agent)
//     {
//         if (agent._pocket != null)
//         {
//             if (packageEntity == null) packageEntity = agent._pocket;
//             if (packageInstance == null)
//             {
//                 int minX = Math.Max(agent.X - agent.VisionRange, 0);
//                 int maxX = Math.Min(agent.X + agent.VisionRange, WorldState.RealWorld.Width - 1);

//                 int minY = Math.Max(agent.Y - agent.VisionRange, 0);
//                 int maxY = Math.Min(agent.Y + agent.VisionRange, WorldState.RealWorld.Height - 1);

//                 for (int x = minX; x <= maxX; x++)
//                 {
//                     for (int y = minY; y <= maxY; y++)
//                     {
//                         packageInstance = MapViewer.GetPackageInstanceAt(x, y);
//                         if (packageInstance != null)
//                         {
//                             return true;
//                         }
//                     }
//                 }
//             }
//         }
//         else
//         {
//             if (packageEntity != null && packageInstance != null)
//             {
//                 GD.Print("(PackageVisuals.cs) Should drop a package");
//                 packageEntity = null;
//                 packageInstance = null;
//                 return true;
//             }
//         }

//         return false;
//     }

//     public static void PickUpPackage(Agent agent)
//     {
//         Spatial agentInstance = MapViewer.GetAgentInstance(agent);
//     }

//     public static void Attach(Agent agent, Spatial packageInstance, Dictionary<string, Spatial> agentInstances, List<List<Dictionary<string, Spatial>>> visibleInstances)
//     {
//         foreach (KeyValuePair<string, Spatial> kvp in agentInstances)
//         {
//             string agentName = kvp.Key;
//             if (agent.Name == agentName)
//             {
//                 Spatial agentInstance = kvp.Value;

//                 packageInstance.Scale = new Vector3(0.5f, 0.5f, 0.5f);
//                 packageInstance.GetParent().RemoveChild(packageInstance);
//                 agentInstance.AddChild(packageInstance);
//                 packageInstance.Translate(new Vector3(0, 1.0f, 0));


//             }
//         }
//     }

//     public static void Detach(Spatial packageInstance, int x, int y, List<List<Dictionary<string, Spatial>>> visibleInstances)
//     {
//         packageInstance.GetParent().RemoveChild(packageInstance);

//     }
// }