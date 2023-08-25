using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AIAttack : Node
{
    private BotManager botManager;
    private EnemyAI AI;
    private AIWeaponManager aiWM;
    private Animator BotAnimator;
    private float shootWalkSpeed = 2.6f;

    public AIAttack(BotManager bot)
    {
        botManager = bot;
        AI = bot.AI;
        aiWM = AI.aiWM;
        BotAnimator = bot.animator;
    }

    public override NodeState Evaluate()
    {
        AI.state = EnemyAI.PlayerState.ATTACKING;
        AI.walkPointSet = false;
        AI.chasePointSet = false;
        AI.agent.speed = shootWalkSpeed;
        Debug.Log("ATTACKING");
        //Make sure enemy doesn't move only if not strafing
        if(AI.attackAction != EnemyAI.AttackAction.STRAFE) 
        {
            AI.agent.SetDestination(AI.transform.position);
            botManager.SetAnimatorState("idle");
        }
      
        aiWM.Shoot(AI.currentEnemyTarget);

        state = NodeState.SUCCESS;
        return state;
    }
}
