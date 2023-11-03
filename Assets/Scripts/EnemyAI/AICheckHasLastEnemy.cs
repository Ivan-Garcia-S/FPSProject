using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AICheckHasLastEnemy : Node
{
    private EnemyAI AI;

    public AICheckHasLastEnemy(BotManager bot){
        AI = bot.AI;
    }

    //Check if enemy is within the attack range radius
    public override NodeState Evaluate()
    {
        state = AI.lastEnemyShotAt == null ? NodeState.FAILURE : NodeState.SUCCESS;
        return state;
    }
}