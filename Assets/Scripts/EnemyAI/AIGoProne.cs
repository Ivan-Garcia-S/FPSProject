using UnityEngine;
using BehaviorTree;


public class AIGoProne : Node
{
    public EnemyAI AI;
    public Animator animator;
    private float proneRate = 0.0009f;
    public AIGoProne(BotManager bot)
    {
        AI = bot.AI;
        animator = bot.animator;
    }
    
    public override NodeState Evaluate()
    {
        //Only possibly prone if not already doing an Attack Action
        if(AI.attackAction == EnemyAI.AttackAction.NONE)
        {
            //Dropshot only a a percent of the time when attacking
            if (Random.Range(0f, 1f) <= proneRate)
            {
                AI.isProne = true;
                AI.attackAction = EnemyAI.AttackAction.DROPSHOT;
                animator.SetBool("prone", true);
            }
        
        }
        state = NodeState.SUCCESS;
        return state;
    }
}
