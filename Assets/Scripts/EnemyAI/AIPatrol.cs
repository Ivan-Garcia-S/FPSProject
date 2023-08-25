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
    private float patrolSpeed = 4f;
    
    public AIPatrol(BotManager bot){
        AI = bot.AI;
        Agent = AI.agent;
        BotAnimator = bot.animator; 
        DestinationBox = bot.destinationPointBox;
        PatrolPoints = bot.patrolPoints;
    }

    public override NodeState Evaluate()
    {
        // Set AI states appropriately
        AI.attackAction = EnemyAI.AttackAction.NONE;
        AI.state = EnemyAI.PlayerState.PATROLING;
        Debug.Log("PATROLING");
        
        //Want to Patrol standing up
        if(AI.isProne) AI.Prone();
        AI.chasePointSet = false;
        AI.agent.speed = patrolSpeed;

        if (!AI.walkPointSet) SearchWalkPoint();

        if (AI.walkPointSet && !Agent.hasPath)
        {
            BotAnimator.SetBool("idle",false);
            BotAnimator.SetBool("moveForward", true);
            BotAnimator.SetBool("run", false);
            Agent.destination = AI.walkPoint;//SetDestination(walkPoint);
            DestinationBox.transform.position = AI.walkPoint;
            Debug.Log("Path is " + Agent.pathStatus);
        }
        Vector3 distanceToWalkPoint = AI.transform.position - AI.walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            BotAnimator.SetBool("moveForward", false);
            BotAnimator.SetBool("idle", true);
            AI.walkPointSet = false;
        }
       //Debug.Log("destination is " + Agent.destination);
        state = NodeState.RUNNING;
        return state;
    }

    public void SearchWalkPoint()
    {
        //New walkpoint method
        int pointNum = Random.Range(0, PatrolPoints.Length);
        //Debug.Log("Point" + (pointNum + 1) + " chosen");
        AI.walkPoint = PatrolPoints[pointNum].position;
        AI.walkPointSet = true;
    }
}
