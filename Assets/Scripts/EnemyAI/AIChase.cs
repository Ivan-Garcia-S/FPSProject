using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using BehaviorTree;

public class AIChase : Node
{
    private EnemyAI AI;
    private Animator BotAnimator;
    private NavMeshAgent Agent;
    private GameObject DestinationBox;
    private float chaseSpeed = 7f;

    public AIChase(BotManager bot)
    {
        AI = bot.AI;
        BotAnimator = bot.animator;
        Agent = AI.agent;
        DestinationBox = bot.destinationPointBox;
    }

    public override NodeState Evaluate()
    {
        //Set AI states appropriately
        AI.attackAction = EnemyAI.AttackAction.NONE;
        AI.state = EnemyAI.PlayerState.CHASING;
        
        //Want to Chase standing up
        if(AI.isProne) AI.Prone();
        
        AI.walkPointSet = false;
        AI.agent.speed = chaseSpeed;
        Debug.Log("CHASING");

        if(!AI.chasePointSet)
        {
            Agent.destination = AI.currentEnemyTarget.position;
            DestinationBox.transform.position = AI.currentEnemyTarget.position;
            AI.chasePointSet = true;
        }  
        BotAnimator.SetBool("shoot", false);
        BotAnimator.SetBool("idle", false);
        BotAnimator.SetBool("run", true);

        state = NodeState.RUNNING;
        return state;
    }

}
