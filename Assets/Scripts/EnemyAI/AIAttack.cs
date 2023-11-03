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
    private float firstBulletDelayMin = 0.6f;
    private float firstBulletDelayMax = 2.3f;

    public AIAttack(BotManager bot)
    {
        botManager = bot;
        AI = bot.AI;
        aiWM = AI.aiWM;
        BotAnimator = bot.animator;
    }

    public override NodeState Evaluate()
    {
        //if(AI.state != EnemyAI.PlayerState.ATTACKING) aiWM.canShootFirstBullet = false;
        
        //Set up attacking state
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
        //Set delay for first shot fired or shoot if already shooting
        if(AI.lastEnemyShotAt == null || AI.lastEnemyShotAt != AI.currentEnemyTarget)
        //if(aiWM.aiNerfOn == true)  CHANGED
        {
            float waitTime = Random.Range(firstBulletDelayMin, firstBulletDelayMax);
            //Debug.Log("Wait time = " + waitTime);
            
            aiWM.StartCountdown(waitTime);
            //aiWM.SetLastTargetToCurrent(waitTime);
            //Debug.Log("Cant shoot YET");
        } 
        
        else 
        {
            aiWM.Shoot(AI.currentEnemyTarget);
        }

        //aiWM.Shoot(AI.currentEnemyTarget);
        state = NodeState.SUCCESS;
        return state;
    }
    

}
