using Godot;
using System;
using System.Collections.Generic;

public class AgentVisuals
{
    public static void ChangePosition(Agent agent, Dictionary<string, Spatial> agentInstances)
    {
        foreach (KeyValuePair<string, Spatial> kvp in agentInstances)
        {
            string agentName = kvp.Key;
            if (agent.Name == agentName)
            {
                Spatial agentInstance = kvp.Value;
                agentInstance.Translation = new Vector3(agent.X, agentInstance.Translation.y, agent.Y);
            }
        }
    }
}