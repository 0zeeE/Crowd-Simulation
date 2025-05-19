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

        // Rastgele hedef sýralamasý oluþtur
        List<Transform> shuffledTargets = new List<Transform>(targetPositions);
        ShuffleList(shuffledTargets);

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

            // Eðer hedef sayýsý yetmezse baþa sar
            Transform target = shuffledTargets[count % shuffledTargets.Count];

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

    // Listeyi karýþtýrmak için yardýmcý fonksiyon
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
