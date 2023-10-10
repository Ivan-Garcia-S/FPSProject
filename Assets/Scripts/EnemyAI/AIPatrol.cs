using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;


public class AIPatrol : Node
{
    private BotManager botManager;
    private EnemyAI AI;
    private NavMeshAgent Agent;
    private Animator BotAnimator;
    private GameObject DestinationBox;
    private Transform[] PatrolPoints;
    private float patrolSpeed = 4f;
    
    public AIPatrol(BotManager bot){
        botManager = bot;
        AI = bot.AI;
        Agent = AI.agent;
        BotAnimator = bot.animator; 
        DestinationBox = bot.destinationPointBox;
        PatrolPoints = bot.patrolPoints;
    }

    public override NodeState Evaluate()
    {
        // Set up patrol state
        AI.attackAction = EnemyAI.AttackAction.NONE;
        AI.state = EnemyAI.PlayerState.PATROLING;
        AI.chasePointSet = false;
        AI.agent.speed = patrolSpeed;
        
        //End shooting animation
        BotAnimator.SetBool("shoot",false);
        AI.aiWM.adsAnimComplete = false;
        Debug.Log("PATROLING");
        
        //Want to Patrol standing up
        if(AI.isProne) AI.Prone();
        
        //Find destination to patrol to
        if (!AI.walkPointSet) SearchWalkPoint();

        //Begin patroling
        if (AI.walkPointSet && Agent.destination != AI.walkPoint)//!Agent.hasPath)
        {
            botManager.SetAnimatorState("moveForward");
            Agent.SetDestination(AI.walkPoint);//Agent.destination = AI.walkPoint; //SetDestination(walkPoint);
            DestinationBox.transform.position = AI.walkPoint;
        }
        Vector3 distanceToWalkPoint = AI.transform.position - AI.walkPoint;
        //If walkpoint reached stop moving, add extra stopping distance for more leeway
        if (distanceToWalkPoint.magnitude <= botManager.stoppingDistance + .5f)
        {
            botManager.SetAnimatorState("idle");
            AI.walkPointSet = false;
        }
       //Debug.Log("destination is " + Agent.destination);
        state = NodeState.RUNNING;
        return state;
    }

    //Choose a new patrol point
    public void SearchWalkPoint()
    {
        int pointNum = Random.Range(0, PatrolPoints.Length);
        //Debug.Log("Point" + (pointNum + 1) + " chosen");
        AI.walkPoint = PatrolPoints[pointNum].position;
        AI.walkPointSet = true;
    }
}
