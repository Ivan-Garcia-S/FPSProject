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

    public AIChase(BotManager bot)
    {
        AI = bot.AI;
        BotAnimator = bot.animator;
        Agent = AI.agent;
        DestinationBox = bot.destinationPointBox;
    }

    public override NodeState Evaluate()
    {
        AI.state = EnemyAI.PlayerState.CHASING;
        AI.walkPointSet = false;
        Debug.Log("CHASING");

        if(!AI.chasePointSet)
        {
            Agent.destination = AI.currentEnemyTarget.position;
            DestinationBox.transform.position = AI.currentEnemyTarget.position;
            AI.chasePointSet = true;
        }  
        BotAnimator.SetBool("shoot", false);
        BotAnimator.SetBool("idle", false);
        BotAnimator.SetBool("move", true);

        state = NodeState.RUNNING;
        return state;
    }

}
