using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentSpawner : MonoBehaviour
{
    public List<GameObject> agentPrefabs;
    public List<Transform> targetPositions;
    public float spawnInterval = 1f;
    public int agentCount = 5;
    public string groupName = "";
    public static Dictionary<string, bool> spawnLocks = new Dictionary<string, bool>();

    private void Start()
    {
        StartCoroutine(SpawnAgents());
    }

    IEnumerator SpawnAgents()
    {
        int count = 0;

        while (count < agentCount)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                if (!spawnLocks.ContainsKey(groupName)) spawnLocks[groupName] = false;

                if (spawnLocks[groupName])
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

                spawnLocks[groupName] = true;
            }

            GameObject prefab = agentPrefabs[Random.Range(0, agentPrefabs.Count)];
            Transform target = targetPositions[Random.Range(0, targetPositions.Count)];

            GameObject agent = Instantiate(prefab, transform.position, Quaternion.identity);
            agent.name = prefab.name + "_Spawned";

            var rvo = agent.GetComponent<RVOAgent>();
            if (rvo != null) rvo.target = target;

            count++;
            yield return new WaitForSeconds(spawnInterval);
        }

        if (!string.IsNullOrEmpty(groupName))
        {
            spawnLocks[groupName] = false;
        }
    }
}
