using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class EnemyBT : BTree
{
    public BotManager botManager;
    //public EnemyAI AI;

    protected override Node SetUpTree()
    {
        Node root = new Selector(new List<Node>
        {
            //ATTACK
            new Sequence(new List<Node>{
                new AICheckPlayerInSight(botManager),
                new AICheckPlayerInAttackRange(botManager),
                new AIAttack(botManager)
            }),

            //CHASE
            new Sequence(new List<Node>
            {
                new AICheckPlayerInSight(botManager),
                new AIChase(botManager)
            }),
            //PATROL
            new AIPatrol(botManager),
        });

        return root;
    }
}
