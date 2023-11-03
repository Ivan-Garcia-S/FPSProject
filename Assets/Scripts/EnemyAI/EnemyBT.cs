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
            //HIDE
            new Sequence(new List<Node>{
                new AIHealthIsLow(botManager),
                new AITakeCover(botManager)
            }),

            //ATTACK
            new Sequence(new List<Node>{
                new AICheckPlayerInSight(botManager),
                new AICheckPlayerInAttackRange(botManager),
                new AIStrafe(botManager),
                new AIGoProne(botManager),
                new AIAttack(botManager)
            }),

            //CHASE
            new Sequence(new List<Node>
            {
                new Selector(new List<Node>{
                    new AICheckPlayerInSight(botManager), 
                    new AICheckHasLastEnemy(botManager)}),
                new AIChase(botManager)
            }),
            //PATROL
            new AIPatrol(botManager),
        });

        return root;
    }
}
