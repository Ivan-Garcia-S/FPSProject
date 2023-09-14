using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using BehaviorTree;

public class AIChase : Node
{
    private BotManager botManager;
    private EnemyAI AI;
    private Animator BotAnimator;
    private NavMeshAgent Agent;
    private GameObject DestinationBox;
    private float chaseSpeed = 7f;

    public AIChase(BotManager bot)
    {
        botManager = bot;
        AI = bot.AI;
        BotAnimator = bot.animator;
        Agent = AI.agent;
        DestinationBox = bot.destinationPointBox;
    }

    public override NodeState Evaluate()
    {
        //Set up chasing state
        AI.attackAction = EnemyAI.AttackAction.NONE;
        AI.state = EnemyAI.PlayerState.CHASING;
        AI.walkPointSet = false;
        AI.agent.speed = chaseSpeed;
        BotAnimator.SetBool("shoot",false);
        
        //Want to Chase standing up
        if(AI.isProne) AI.Prone();
        
        
        Debug.Log("CHASING");

        //Set a destination for the AI to move to
        if(!AI.chasePointSet)
        {
            Agent.destination = AI.currentEnemyTarget.position;
            DestinationBox.transform.position = AI.currentEnemyTarget.position;
            AI.chasePointSet = true;
        }  
        botManager.SetAnimatorState("sprint");
        //TAKE OUT FOR NEW ANIM
        //BotAnimator.SetBool("run", true);

        state = NodeState.RUNNING;
        return state;
    }

}
