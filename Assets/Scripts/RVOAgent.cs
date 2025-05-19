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
    public bool isInterrupted = false;
    public List<Transform> targetTransforms;

    
    //Deneme amacli sabit bir noktayi setliyor. Acil durum cikma noktalari icin bu kullanilabilir.
    [ContextMenu("Set Target")]
    public void SetTarget()
    {
        if(secondTarget != null)
        {
            target = secondTarget;
            previousTarget = target;
            currentNodeInThePath = 0;
            simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
            pathNodes = new List<Vector3>();
            StartCoroutine(StartPaths());
            agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
            isAbleToStart = true;
        }
        
    }

    //NPC'nin yeni bir hedefi rastgele secip gitmesini sagliyor
    [ContextMenu("Use Random Target")]
    public void RandomTarget()
    {
        int targetIndex = UnityEngine.Random.Range(0, targetTransforms.Count - 1);
        target = targetTransforms[targetIndex];
        previousTarget = target;
        currentNodeInThePath = 0;
        simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
        pathNodes = new List<Vector3>();
        StartCoroutine(StartPaths());
        agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
        isAbleToStart = true;
    }
    
    //NPC'nin RVO ve A* hareketlerini durdurur ve oyuncuya bakmasini saglar.
    [ContextMenu("Interrupt NPC")]
    public void InterrputNPC()
    {
        isInterrupted = true;
        target = this.gameObject.transform;
        currentNodeInThePath = 0;
        simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
        pathNodes = new List<Vector3>();
        StartCoroutine(StartPaths());
        agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
        isAbleToStart = true;

    }

    //Durdurulan NPC'nin kaldigi yerden tekrardan yola koyulmasini saglar.
    [ContextMenu("Continue Path")]
    public void ContinuePath()
    {
        isInterrupted = false;
        target = previousTarget;
        previousTarget = target;
        currentNodeInThePath = 0;
        simulator = GameObject.FindGameObjectWithTag("RVOSim").GetComponent<RVO2Simulator>();
        pathNodes = new List<Vector3>();
        StartCoroutine(StartPaths());
        agentIndex = simulator.addAgentToSim(transform.position, gameObject, pathNodes);
        isAbleToStart = true;

    }

    IEnumerator Start()
    {
        GameObject[] TargetGObj = GameObject.FindGameObjectsWithTag("Goals");

        foreach (GameObject target in TargetGObj)
        {
            targetTransforms.Add(target.transform);
        }
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
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
        
        if (isAbleToStart && agentIndex != -1 &&!isInterrupted)
        {
           
            transform.position = toUnityVector(simulator.getAgentPosition(agentIndex));
        }
        if(isInterrupted && playerTransform != null)
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
