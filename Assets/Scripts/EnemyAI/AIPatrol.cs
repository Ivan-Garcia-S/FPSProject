using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;


public class AIPatrol : Node
{
    private EnemyAI AI;
    private NavMeshAgent Agent;
    private Animator BotAnimator;
    private GameObject DestinationBox;
    private Transform[] PatrolPoints;
    
    public AIPatrol(BotManager bot){
        AI = bot.AI;
        Agent = AI.agent;
        BotAnimator = bot.animator; 
        DestinationBox = bot.destinationPointBox;
        PatrolPoints = bot.patrolPoints;
    }

    public override NodeState Evaluate()
    {
        AI.state = EnemyAI.PlayerState.PATROLING;
        Debug.Log("PATROLING");
        AI.chasePointSet = false;
        if (!AI.walkPointSet) SearchWalkPoint();

        if (AI.walkPointSet && !Agent.hasPath)
        {
            BotAnimator.SetBool("idle",false);
            BotAnimator.SetBool("move", true);
            Agent.destination = AI.walkPoint;//SetDestination(walkPoint);
            DestinationBox.transform.position = AI.walkPoint;
            Debug.Log("Path is " + Agent.pathStatus);
        }
        Vector3 distanceToWalkPoint = AI.transform.position - AI.walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            BotAnimator.SetBool("move", false);
            BotAnimator.SetBool("idle", true);
            Debug.Log("Within close proximity of point");
            AI.walkPointSet = false;
        }
        state = NodeState.RUNNING;
        return state;
    }

    public void SearchWalkPoint()
    {
        //Old walkpoint method
        /*Vector3 randDirection = Random.insideUnitSphere * 20f;

        randDirection += AI.transform.position;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, 20f, -1);
        AI.walkPoint = navHit.position;
        AI.walkPointSet = true;
        */

        //Debug.Log("New Walkpoint = " + walkPoint);
    
        //New walkpoint method
        int pointNum = Random.Range(0, PatrolPoints.Length);
        Debug.Log("Point" + (pointNum + 1) + " chosen");
        AI.walkPoint = PatrolPoints[pointNum].position;
        AI.walkPointSet = true;
        
        
    }
}
