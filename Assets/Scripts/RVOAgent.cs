using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using RVO;
using Pathfinding;

public class RVOAgent : MonoBehaviour
{

    [SerializeField]
    public Transform target = null;

    Seeker agentSeeker;
    private List<Vector3> pathNodes = null;
    RVO2Simulator simulator = null;
    int agentIndex = -1;
    int currentNodeInThePath = 0;
    bool isAbleToStart = false;
    public string targetTag;
    public Transform secondTarget;
    public Transform previousTarget;
    public Transform playerTransform;
    public string playerTag = "Player";
    public bool isInterrupted = false;
    [SerializeField] private bool panicMode = false;
    public List<Transform> targetTransforms;
    public List<Transform> emergencyExitTransforms;

    [SerializeField] private GameObject AiModule;

    [SerializeField] private float targetReachDistance = 0.5f;

    //Deneme amacli sabit bir noktayi setliyor. Acil durum cikma noktalari icin bu kullanilabilir.
    [ContextMenu("Set Target")]
    public void SetTarget()
    {
        if(panicMode == false)
        {
            if (secondTarget != null)
            {
                target = secondTarget;
                previousTarget = target;
                currentNodeInThePath = 0;
                //simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
                //pathNodes = new List<Vector3>();
                StartCoroutine(StartPaths());
                //agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
                isAbleToStart = true;
            }
        }
        
        
    }

    //NPC'nin yeni bir hedefi rastgele secip gitmesini sagliyor
    [ContextMenu("Use Random Target")]
    public void RandomTarget()
    {
        if(panicMode == false)
        {
            int targetIndex = UnityEngine.Random.Range(0, targetTransforms.Count - 1);
            target = targetTransforms[targetIndex];
            previousTarget = target;
            currentNodeInThePath = 0;
            //simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
            //pathNodes = new List<Vector3>();
            StartCoroutine(StartPaths());
            //agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
            isAbleToStart = true;
        }
        
    }

    //NPC'nin yeni bir hedefi rastgele secip gitmesini sagliyor
    [ContextMenu("Use Emergency Exit")]
    public void EmergencyExit()
    {
        panicMode = true;
        int targetIndex = UnityEngine.Random.Range(0, emergencyExitTransforms.Count - 1);
        target = emergencyExitTransforms[targetIndex];
        previousTarget = target;
        currentNodeInThePath = 0;
        //simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
        //pathNodes = new List<Vector3>();
        StartCoroutine(StartPaths());
        //agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
        isAbleToStart = true;
    }

    //NPC'nin RVO ve A* hareketlerini durdurur ve oyuncuya bakmasini saglar.
    [ContextMenu("Interrupt NPC")]
    public void InterrputNPC()
    {
        if(panicMode == false)
        {
            isInterrupted = true;
            target = this.gameObject.transform;
            currentNodeInThePath = 0;
            //simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
            //pathNodes = new List<Vector3>();
            StartCoroutine(StartPaths());
            //agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
            if (AiModule != null)
            {
                AiModule.SetActive(true);
            }
            isAbleToStart = true;
        }
        

    }

    //Durdurulan NPC'nin kaldigi yerden tekrardan yola koyulmasini saglar.
    [ContextMenu("Continue Path")]
    public void ContinuePath()
    {
        isInterrupted = false;
        target = previousTarget;
        previousTarget = target;
        currentNodeInThePath = 0;
        //simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
        //pathNodes = new List<Vector3>();
        StartCoroutine(StartPaths());
        //agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
        if (AiModule != null)
        {
            AiModule.SetActive(false);
        }
        isAbleToStart = true;

    }

    IEnumerator Start()
    {
        // Eðer hedef önceden atanmýþsa, tekrar atama
        if (target == null && !string.IsNullOrEmpty(targetTag))
        {
            GameObject[] TargetGObj = GameObject.FindGameObjectsWithTag("Goals");
            foreach (GameObject t in TargetGObj)
            {
                targetTransforms.Add(t.transform);
            }

            GameObject foundTarget = GameObject.FindGameObjectWithTag(targetTag);
            if (foundTarget != null)
            {
                target = foundTarget.transform;
            }
        }

        GameObject[] emergencyExitObj = GameObject.FindGameObjectsWithTag("Emergency Exits");
        foreach (GameObject t in emergencyExitObj)
        {
            emergencyExitTransforms.Add(t.transform);
        }

        if (GameObject.FindGameObjectWithTag(playerTag) != null) playerTransform = GameObject.FindGameObjectWithTag(playerTag).transform;
        else Debug.Log("Player tag bulunamadi.");

        previousTarget = target;
        currentNodeInThePath = 0;
        simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
        pathNodes = new List<Vector3>();
        yield return StartCoroutine(StartPaths());
        agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
        isAbleToStart = true;
    }

    IEnumerator StartPaths()
    {
        agentSeeker = gameObject.GetComponent<Seeker>();
        var path = agentSeeker.StartPath(transform.position, target.position, OnPathComplete);
        yield return StartCoroutine(path.WaitForPath());
    }
    public void OnPathComplete(Path p)
    {
        //We got our path back
        if (p.error)
        {
            Debug.Log("" + this.gameObject.name + " ---- -" + p.errorLog);
        }
        else
        {
            pathNodes = p.vectorPath;
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isAbleToStart && agentIndex != -1 && !isInterrupted)
        {
            transform.position = toUnityVector(simulator.getAgentPosition(agentIndex));

            // Hedefe ulaþýldý mý kontrol et
            if (currentNodeInThePath >= pathNodes.Count && pathNodes.Count > 0)
            {
                Vector3 lastNode = pathNodes[pathNodes.Count - 1];
                float distance = Vector3.Distance(transform.position, lastNode);
                if (distance < targetReachDistance)
                {
                    DespawnAgent();
                }
            }
        }
        if (isInterrupted && playerTransform != null)
        {
            this.gameObject.transform.LookAt(playerTransform);
        }
    }
    public RVO.Vector2 calculateNextStation()
    {

        Vector3 station;
        if (currentNodeInThePath < pathNodes.Count)
        {
            station = pathNodes[currentNodeInThePath];
            float distance = Vector3.Distance(new Vector3(station.x, transform.position.y, station.z), transform.position);
            if (distance >= 0 && distance < 1)
            {
                currentNodeInThePath++;
            }
            station = pathNodes[Mathf.Min(currentNodeInThePath, pathNodes.Count - 1)];
        }
        else
        {
            station = pathNodes[pathNodes.Count - 1];
        }

        return toRVOVector(station);
    }

    public void UpdateAgentIndex(int newIndex)
    {
        agentIndex = newIndex;
    }

    void DespawnAgent()
    {
        // RVO simülasyonundan ajaný çýkar
        if (simulator != null && agentIndex != -1)
        {
            simulator.RemoveAgent(agentIndex, gameObject);
        }
        else
        {
            Debug.LogWarning($"Ajan {gameObject.name} için simulator veya agentIndex geçersiz!");
        }

        // GameObject'i yok et
        Destroy(gameObject);
    }
    Vector3 toUnityVector(RVO.Vector2 param)
    {
        float y = transform.position.y;

        // Eðer pathNodes varsa ve geçerli bir node varsa, yüksekliði oradan al
        if (pathNodes != null && currentNodeInThePath < pathNodes.Count)
        {
            y = pathNodes[currentNodeInThePath].y;
        }

        return new Vector3(param.x(), y, param.y());
    }

    RVO.Vector2 toRVOVector(Vector3 param)
    {
        return new RVO.Vector2(param.x, param.z);
    }

}
