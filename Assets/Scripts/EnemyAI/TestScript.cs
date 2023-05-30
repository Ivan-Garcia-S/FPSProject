using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestScript : MonoBehaviour
{
    NavMeshAgent agent;
    Transform[] patrolPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        patrolPoints = GameObject.Find("Patrol Points").GetComponentsInChildren<Transform>();
        foreach(Transform point in patrolPoints) Debug.Log(point.position);    
        GoToPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void GoToPoints()
    {
        foreach(Transform point in patrolPoints)
        {
            agent.destination = point.position;

            Vector3 distanceToWalkPoint = agent.transform.position - point.position;
            while(distanceToWalkPoint.magnitude > 1.5f){
                distanceToWalkPoint = agent.transform.position - point.position;
            }
        }
    }
}
