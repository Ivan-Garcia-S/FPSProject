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

    public override NodeState Evaluate()
    {
        Collider[] enemyCollidersInSight = Physics.OverlapSphere(AI.transform.position, AI.attackRange, AI.whatIsPlayer);
        foreach(Collider c in enemyCollidersInSight)
        {
            try
            {
                if(c.GetComponentInParent<CharacterController>().transform == AI.currentEnemyTarget)
                {
                    state = NodeState.SUCCESS;
                    goto EndCheck;
                }
            }
            catch(NullReferenceException){}
            
        }
        EndCheck:
            state = state == NodeState.SUCCESS ? state : NodeState.FAILURE;
            return state;
    }
}
