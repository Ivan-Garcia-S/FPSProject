using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;


public class AIPatrol : Node
{
    private EnemyAI AI;
    private NavMeshAgent Agent;
    
    public AIPatrol(EnemyAI ai){
        AI = ai;
        Agent = AI.agent;
    }

    public override NodeState Evaluate()
    {
        AI.state = EnemyAI.PlayerState.PATROLING;
        Debug.Log("PATROLING");
        if (!AI.walkPointSet) SearchWalkPoint();

        if (AI.walkPointSet && !Agent.hasPath)

            Agent.destination = AI.walkPoint;//SetDestination(walkPoint);
            Debug.Log("Path is " + Agent.pathStatus);

        Vector3 distanceToWalkPoint = AI.transform.position - AI.walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            Debug.Log("Within close proximity of point");
            AI.walkPointSet = false;

        state = NodeState.RUNNING;
        return state;
    }

    public void SearchWalkPoint()
    {
        //Old walkpoint method
        Vector3 randDirection = Random.insideUnitSphere * 20f;

        randDirection += AI.transform.position;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, 20f, -1);
        AI.walkPoint = navHit.position;
        AI.walkPointSet = true;

        //Debug.Log("New Walkpoint = " + walkPoint);
    
        //New walkpoint method
        /*int pointNum = Random.Range(0, patrolPoints.Length);
        Debug.Log("Point" + (pointNum + 1) + " chosen");
        walkPoint = patrolPoints[pointNum].position;
        walkPointSet = true;
        */
        
    }
}
