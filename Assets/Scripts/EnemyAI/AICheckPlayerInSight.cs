using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AICheckPlayerInSight : Node
{
    public EnemyAI AI;
    public AICheckPlayerInSight(EnemyAI ai)
    {
        AI = ai;
    }

    public override NodeState Evaluate()
    {
        if(AI.currentEnemyTarget != null) state = NodeState.SUCCESS;
        else state = NodeState.FAILURE;
        
        return state;
    }
}
