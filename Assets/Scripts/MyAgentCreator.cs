using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAgentCreator : MonoBehaviour
{
    [SerializeField] private AgentSpawner agentCreator;
    
    public void setAgentCreator(AgentSpawner spawner)
    {
        this.agentCreator = spawner;
    }

    
    //yok edilen agentlar kendisini yok etmeden once agentCreator'a kendi varligini tanitiyor.
    public void OnDestroy()
    {
        if (agentCreator != null && agentCreator.GetSpawnBool())
        {
            
            agentCreator.SpawnedObjectDestroyed();
            
        }
    }

    
}
