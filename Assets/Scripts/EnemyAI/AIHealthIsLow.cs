using UnityEngine;
using BehaviorTree;


public class AIHealthIsLow : Node
{
    public EnemyAI AI;
    public BotManager botManager;
    public AIHealthIsLow(BotManager bot)
    {
       botManager = bot;
    }
    
    //Return if the AI is within the critical health state
    public override NodeState Evaluate()
    {
        if(botManager.currentHealth < botManager.criticalState) state = NodeState.SUCCESS;
        else state = NodeState.FAILURE;
        return state;
    }
}