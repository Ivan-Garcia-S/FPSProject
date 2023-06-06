using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AIAttack : Node
{
    private EnemyAI AI;

    public AIAttack(EnemyAI ai)
    {
        AI = ai;
    }

    public override NodeState Evaluate()
    {
        AI.state = EnemyAI.PlayerState.ATTACKING;
        AI.walkPointSet = false;
        Debug.Log("ATTACKING");
        //Make sure enemy doesn't move
        AI.agent.SetDestination(AI.transform.position);

        AI.transform.LookAt(AI.currentEnemyTarget);

        if (!AI.alreadyAttacked)
        {
            Vector3 shootPoint = AI.transform.Find("pistol3/Shoot Point").transform.position;
            //Calculate direction from attackPoint to the targetPoint
            Vector3 directionNoSpread = AI.currentEnemyTarget.position - shootPoint;

             //Calculate spread
            float spreadX = Random.Range(-AI.spread, AI.spread);
            float spreadY = Random.Range(-AI.spread, AI.spread);

            //Direction with spread
            Vector3 directionWithSpread = directionNoSpread + new Vector3(spreadX,spreadY,0); 
            
           
            ///Create bullet and add force to make it shoot forward
            GameObject bullet = GameObject.Instantiate(AI.projectile, shootPoint, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().SendMessage("SetBulletInfo", AI.myTag);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            rb.AddForce(directionWithSpread.normalized * 100f, ForceMode.Impulse);
            rb.AddForce(AI.transform.Find("pistol3").transform.up * AI.upwardForce, ForceMode.Impulse);

            //Set delay for the next time AI can shoot
            AI.alreadyAttacked = true;
            AI.InvokeResetAttack();
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
