using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AIStrafe : Node
{
    public BotManager botManager;
    public EnemyAI AI;
    public Animator animator;
    private float startStrafeRate = 0.012f;
    private float strafeAgainRate = 0.75f;
    private float maxStrafeDistance = 1.8f;

    public AIStrafe(BotManager bot)
    {
        botManager = bot;
        AI = bot.AI;
        animator = bot.animator;
    }

    public override NodeState Evaluate()
    {
        if(AI.attackAction == EnemyAI.AttackAction.NONE){   // If AI currently has no Attack Action
            if (Random.Range(0f, 1f) <= startStrafeRate)  //Randomly choose to either strafe or not
            {
                AI.attackAction = EnemyAI.AttackAction.STRAFE;
                AI.strafingRight = Random.Range(0f, 1f) <= 0.5f ? true : false; // Set strafe direction to either right or left
                SetStrafeDestination();
            }
        }
        // If strafe left/right has ended, change direction and strafe again or stop strafing
        else if(AI.attackAction == EnemyAI.AttackAction.STRAFE && !AI.agent.hasPath) 
        {
            if(Random.Range(0f, 1f) <= strafeAgainRate)
            {
                AI.strafingRight = !AI.strafingRight;
                SetStrafeDestination();
            }
            else AI.attackAction = EnemyAI.AttackAction.NONE;
        }
        state = NodeState.SUCCESS;
        return state;
    }

    //Choose how far to strafe
    public void SetStrafeDestination()
    {
        float strafeXDistance = Random.Range(0.2f, maxStrafeDistance);
        strafeXDistance = AI.strafingRight == true ? strafeXDistance : strafeXDistance * -1;
        AI.strafeDestination = AI.transform.position + AI.transform.right * strafeXDistance; // Set strafe destination
        AI.agent.SetDestination(AI.strafeDestination);
        if(AI.strafingRight) botManager.StopMovementExceptFor("strafeRight");
        else if(!AI.strafingRight) botManager.StopMovementExceptFor("strafeLeft");
        else Debug.LogWarning("AI.strafingRight is NULL");
    }
}