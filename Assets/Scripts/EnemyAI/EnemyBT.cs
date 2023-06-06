using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class EnemyBT : BTree
{
    
    public EnemyAI AI;

    protected override Node SetUpTree()
    {
        Node root = new Selector(new List<Node>
        {
            //ATTACK
            new Sequence(new List<Node>{
                new AICheckPlayerInSight(AI),
                new AICheckPlayerInAttackRange(AI),
                new AIAttack(AI)
            }),

            //CHASE
            new Sequence(new List<Node>
            {
                new AICheckPlayerInSight(AI),
                new AIChase(AI)
            }),
            //PATROL
            new AIPatrol(AI),
        });

        return root;
    }
}
