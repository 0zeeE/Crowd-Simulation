using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RVO;

public class RVO2Simulator : MonoBehaviour
{

    List<RVO.Vector2> agentPositions;
    List<GameObject> rvoGameObj;
    public float agentRadius = 15f;
    public float neighbourDistance= 0.5f;
    public int maxNeighbors = 10;
    public float timeHorizon = 5f;
    public float timeHorizonObst = 5f;


    // Use this for initialization
    void Start()
    {
        agentPositions = new List<RVO.Vector2>();
        rvoGameObj = new List<GameObject>();

        Simulator.Instance.setTimeStep(0.01f);
        Simulator.Instance.setAgentDefaults(neighbourDistance, maxNeighbors, timeHorizon, timeHorizonObst, agentRadius, 2.0f, new RVO.Vector2(0.0f, 0.0f));
        //Simulator.Instance.setAgentDefaults(neighbourDistance, 10, 5.0f, 5.0f, agentRadius, 2.0f, new RVO.Vector2(0.0f, 0.0f));

    }

    public Simulator getSimulator()
    {
        return Simulator.Instance;
    }

    Vector3 toUnityVector(RVO.Vector2 param)
    {
        return new Vector3(param.x(), transform.position.y, param.y());
    }

    RVO.Vector2 toRVOVector(Vector3 param)
    {
        return new RVO.Vector2(param.x, param.z);
    }

    public RVO.Vector2 getAgentPosition(int agentIndex)
    {
        return Simulator.Instance.getAgentPosition(agentIndex);
    }


    public int addAgentToSim(Vector3 pos, GameObject ag, List<Vector3> paths)
    {
        //remove the initial position since the agent is already on it
        if (paths != null && paths.Count > 0)
            paths.Remove(paths[0]);

        //clear the simulation
        Simulator.Instance.Clear();
        //re initialize the simulation
        Simulator.Instance.setTimeStep(0.10f);
        Simulator.Instance.setAgentDefaults(neighbourDistance, maxNeighbors, timeHorizon, timeHorizonObst, agentRadius, 2.0f, new RVO.Vector2(0.0f, 0.0f));
        //Simulator.Instance.setAgentDefaults(neighbourDistance, 10, 5.0f, 5.0f, agentRadius, 2.0f, new RVO.Vector2(0.0f, 0.0f));

        //add all the previous agents
        int agentCount = agentPositions.Count;
        for (int i = 0; i < agentCount; i++)
        {
            Simulator.Instance.addAgent(agentPositions[i]);
        }

        //add the new agent
        rvoGameObj.Add(ag);
        agentPositions.Add(toRVOVector(pos));

        //return the index of the new added agent
        return Simulator.Instance.addAgent(toRVOVector(pos));
    }

    public void RemoveAgent(int agentIndex, GameObject agentObj)
    {
        // Listelerden ajan� kald�r
        if (agentIndex >= 0 && agentIndex < agentPositions.Count)
        {
            agentPositions.RemoveAt(agentIndex);
            rvoGameObj.Remove(agentObj);
        }
        else
        {
            Debug.LogWarning($"Agent index {agentIndex} ge�ersiz veya listede yok!");
            return;
        }

        // Sim�lasyonu s�f�rla
        Simulator.Instance.Clear();

        // Sim�lasyon ayarlar�n� yeniden yap�land�r
        Simulator.Instance.setTimeStep(0.10f);
        Simulator.Instance.setAgentDefaults(neighbourDistance, maxNeighbors, timeHorizon, timeHorizonObst, agentRadius, 2.0f, new RVO.Vector2(0.0f, 0.0f));
        //Simulator.Instance.setAgentDefaults(neighbourDistance, 10, 5.0f, 5.0f, agentRadius, 2.0f, new RVO.Vector2(0.0f, 0.0f));

        // Kalan ajanlar� yeniden ekle
        for (int i = 0; i < agentPositions.Count; i++)
        {
            Simulator.Instance.addAgent(agentPositions[i]);
        }

        // RVOAgent' lar�n agentIndex'lerini g�ncelle
        for (int i = 0; i < rvoGameObj.Count; i++)
        {
            var rvoAgent = rvoGameObj[i].GetComponent<RVOAgent>();
            if (rvoAgent != null)
            {
                rvoAgent.UpdateAgentIndex(i); 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int agentNUmber = Simulator.Instance.getNumAgents();
        try
        {
            for (int i = 0; i < agentNUmber; i++)
            {
                RVO.Vector2 agentLoc = Simulator.Instance.getAgentPosition(i);
                RVO.Vector2 station = rvoGameObj[i].GetComponent<RVOAgent>().calculateNextStation() - agentLoc;

                if (RVOMath.absSq(station) > 1.0f)
                {
                    station = RVOMath.normalize(station);
                }

                Simulator.Instance.setAgentPrefVelocity(i, station);
                agentPositions[i] = Simulator.Instance.getAgentPosition(i);
            }
            Simulator.Instance.doStep();
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.StackTrace);
        }
    }
}
