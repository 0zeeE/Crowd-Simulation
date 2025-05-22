using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;

public class SimulationManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] RVO_agents;
    public GameObject[] agentCreators;
    public GameObject[] graphLinks;
    public GameObject aStar;
    public GameObject fireEffectParent;
    public bool useNodeLinkDestruction = false;
    [SerializeField] private bool isActivated = false;
    public string agentTag = "agent";
    public string agentCreatorTag = "agent creator";
    public string linkToBeDeleted = "delete link";



    [ContextMenu("Start Emergency Mode")]
    public void StartEmergencyMode()
    {
        if(!isActivated) StartCoroutine(EmergencyMode());
        
    }

    IEnumerator EmergencyMode()
    {
        agentCreators = GameObject.FindGameObjectsWithTag(agentCreatorTag);
        foreach(GameObject creator in agentCreators )
        {
            creator.GetComponent<AgentSpawner>().SetSpawnBool(false);
        }
        yield return new WaitForSeconds(1);
        if (useNodeLinkDestruction)
        {
            graphLinks = GameObject.FindGameObjectsWithTag(linkToBeDeleted);
            foreach (GameObject linkNodes in graphLinks)
            {
                linkNodes.GetComponent<NodeLink>().deleteConnection = true;
            }
            aStar.GetComponent<AstarPath>().Scan();
        }
        yield return new WaitForSeconds(1);
        fireEffectParent.SetActive(true);
        RVO_agents = GameObject.FindGameObjectsWithTag(agentTag);
        foreach(GameObject RVOagent in RVO_agents)
        {
            RVOagent.GetComponent<RVOAgent>().EmergencyExit();
        }
        yield return new WaitForSeconds(1);
    }

    IEnumerator BeforeSceneChange()
    {
        agentCreators = GameObject.FindGameObjectsWithTag(agentCreatorTag);
        foreach (GameObject creator in agentCreators)
        {
            creator.GetComponent<AgentSpawner>().SetSpawnBool(false);
        }
        yield return new WaitForSeconds(1);
    }


    public void RestartScene()
    {
        StartCoroutine(BeforeSceneChange());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuScene()
    {
        StartCoroutine(BeforeSceneChange());
        SceneManager.LoadScene(0); //0. index ana menu olacak.
    }
}
