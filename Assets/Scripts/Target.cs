using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    private NavMeshAgent[] navAgents;
    public Transform targetMarker;
    public float verticalOffset = 10.0f;

    void Start()
    {
        // Get every NavMeshAgent in the scene
        navAgents = FindObjectsOfType<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 targetPos = hitInfo.point;

                // Move all agents
                UpdateTargets(targetPos);

                // Move target visual marker
                if (targetMarker != null)
                    targetMarker.position = targetPos + new Vector3(0, verticalOffset, 0);
            }
        }
    }

    void UpdateTargets(Vector3 targetPosition)
    {
        foreach (NavMeshAgent agent in navAgents)
        {
            if (agent != null && agent.isActiveAndEnabled)
                agent.SetDestination(targetPosition);
        }
    }
}

