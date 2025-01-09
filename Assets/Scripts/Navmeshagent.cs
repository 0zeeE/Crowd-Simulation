using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    public NavMeshAgent agent; // NavMeshAgent referansý
    public Transform targetPoint; // Hedef nokta (Inspector'dan ayarlanabilir)

    void Update()
    {
        if (targetPoint != null)
        {
            // Hedef noktaya ilerle
            agent.SetDestination(targetPoint.position);
        }
    }

    // Hedefi dinamik olarak deðiþtirmek için bir metod
    public void SetNewTarget(Vector3 newTarget)
    {
        agent.SetDestination(newTarget);
    }
}
