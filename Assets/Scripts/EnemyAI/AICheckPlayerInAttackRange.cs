using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


using BehaviorTree;

public class AICheckPlayerInAttackRange : Node
{
    private EnemyAI AI;

    public AICheckPlayerInAttackRange(BotManager bot){
        AI = bot.AI;
    }

    //Check if enemy is within the attack range radius
    public override NodeState Evaluate()
    {
        float range = AI.attackRange;
        //If already attacking an enemy, make range slightly larger so enemy still visible even if they slightly move away
        if(AI.state == EnemyAI.PlayerState.ATTACKING){
            range += 2.3f;
        }
        Collider[] enemyCollidersInSight = Physics.OverlapSphere(AI.transform.position, range, AI.whatIsPlayer);
        foreach(Collider c in enemyCollidersInSight)
        {
            try
            {
                if(c.GetComponentInParent<CharacterController>().transform == AI.currentEnemyTarget)
                {
                    state = NodeState.SUCCESS;
                    return state;
                }
            }
            catch(NullReferenceException){}
            
        }
            state = NodeState.FAILURE;
            /*if(state == NodeState.FAILURE){
                Debug.Log("InAttackRange failed at " + Time.time);
            }
            */
            return state;
    }
}
