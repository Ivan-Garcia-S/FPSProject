using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AIChase : Node
{
    EnemyAI AI;

    public AIChase(EnemyAI ai)
    {
        AI = ai;
    }

    public override NodeState Evaluate()
    {
        AI.state = EnemyAI.PlayerState.CHASING;
        AI.walkPointSet = false;
        Debug.Log("CHASING");
        AI.agent.SetDestination(AI.currentEnemyTarget.position);

        state = NodeState.RUNNING;
        return state;
    }

}
