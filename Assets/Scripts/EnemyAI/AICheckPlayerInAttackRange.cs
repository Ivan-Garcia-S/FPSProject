using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


using BehaviorTree;

public class AICheckPlayerInAttackRange : Node
{
    private EnemyAI AI;

    public AICheckPlayerInAttackRange(EnemyAI ai){
        AI = ai;
    }

    public override NodeState Evaluate()
    {
        Collider[] enemiesInRadius = Physics.OverlapSphere(AI.transform.position, AI.attackRange, AI.whatIsPlayer);
        if(enemiesInRadius.Contains(AI.currentEnemyTarget.GetComponent<Collider>())) state = NodeState.SUCCESS;
        else state = NodeState.FAILURE;

        return state;
    }
}
