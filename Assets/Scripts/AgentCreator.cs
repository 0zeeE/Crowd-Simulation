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
    [SerializeField] private int count = 0;
    [SerializeField] private bool canSpawn = false;

    private void Awake()
    {
        // SAHNE YENÝDEN BAÞLATILDIÐINDA STATÝK LOCKLARI TEMÝZLE
        if (!string.IsNullOrEmpty(groupName) && spawnLocks.ContainsKey(groupName))
        {
            spawnLocks[groupName] = false;
        }

        count = 0;
    }

    private void Start()
    {
        canSpawn = true;
        StartCoroutine(SpawnAgents());
    }

    IEnumerator SpawnAgents()
    {
        
        if(canSpawn == true)
        {

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

                // Rastgele hedef seç
                Transform target = targetPositions[Random.Range(0, targetPositions.Count)];

                GameObject agent = Instantiate(prefab, transform.position, Quaternion.identity);
                agent.name = prefab.name + "_Spawned";
                //spawner kendisine ait her objeye MyAgentCreator scriptini atiyor ve buradan kendisine bagliyor.
                agent.AddComponent<MyAgentCreator>();
                agent.GetComponent<MyAgentCreator>().setAgentCreator(this.gameObject.GetComponent<AgentSpawner>());

                var rvo = agent.GetComponent<RVOAgent>();
                if (rvo != null)
                {
                    rvo.target = target;
                    rvo.previousTarget = target;
                }

                count++;
                yield return new WaitForSeconds(spawnInterval);
            }

            if (!string.IsNullOrEmpty(groupName))
            {
                spawnLocks[groupName] = false;
            }
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

    //Spawnlanan obje yok edildiginde calismasi icin konuldu.
    public void SpawnedObjectDestroyed()
    {
        if(canSpawn == true)
        {
            --count;
            StartCoroutine(SpawnAgents());
        }
        
    }

    public bool GetSpawnBool()
    {
        return canSpawn;
    }

    //UI uzerinden sahneyi kapatma ya da yeniden baslatma yapmadan once buradan kontrol saglanmali. Yoksa MyAgentCreator'daki OnDestroy Memory Leak yapacak.
    public void SetSpawnBool(bool value)
    {
        canSpawn = value;
    }

    private void OnApplicationQuit()
    {
        canSpawn = false;
        spawnLocks[groupName] = false;
    }

    
}
