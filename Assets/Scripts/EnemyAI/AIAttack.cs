using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AIAttack : Node
{
    private EnemyAI AI;
    private Animator BotAnimator;

    public AIAttack(BotManager bot)
    {
        AI = bot.AI;
        BotAnimator = bot.animator;
    }

    public override NodeState Evaluate()
    {
        AI.state = EnemyAI.PlayerState.ATTACKING;
        AI.walkPointSet = false;
        AI.chasePointSet = false;
        Debug.Log("ATTACKING");
        //Make sure enemy doesn't move
        AI.agent.SetDestination(AI.transform.position);
        BotAnimator.SetBool("move", false);
        BotAnimator.SetBool("idle", true);

        //Old look at
        //AI.transform.LookAt(AI.currentEnemyTarget);

        //New look at
        Vector3 rotation = Quaternion.LookRotation(AI.currentEnemyTarget.position - AI.transform.position).eulerAngles;
        rotation.x = rotation.z = 0f;
        AI.transform.rotation = Quaternion.Euler(rotation);

        if (!AI.alreadyAttacked)
        {
            Vector3 shootPoint = AI.GetComponentInChildren<AIWeaponManager>().transform.Find("Shoot Point").transform.position;
            //Vector3 shootPoint = AI.transform.Find("pistol/Shoot Point").transform.position;
            //Calculate direction from attackPoint to the targetPoint
            Vector3 directionNoSpread = AI.currentEnemyTarget.position - shootPoint;

             //Calculate spread
            float spreadX = Random.Range(-AI.spread, AI.spread);
            float spreadY = Random.Range(-AI.spread, AI.spread);

            //Direction with spread
            Vector3 directionWithSpread = directionNoSpread + new Vector3(spreadX,spreadY,0); 
            
            BotAnimator.SetBool("idle", false);
            BotAnimator.SetBool("move", false);
            BotAnimator.SetBool("shoot",true);
            ///Create bullet and add force to make it shoot forward
            GameObject bullet = GameObject.Instantiate(AI.projectile, shootPoint, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().SendMessage("SetBulletInfo", AI.tag);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            rb.AddForce(directionWithSpread.normalized * 100f, ForceMode.Impulse);
            rb.AddForce(AI.GetComponentInChildren<AIWeaponManager>().transform.up * AI.upwardForce, ForceMode.Impulse);
            //rb.AddForce(AI.transform.Find("pistol").transform.up * AI.upwardForce, ForceMode.Impulse);

            //Set delay for the next time AI can shoot
            AI.alreadyAttacked = true;
            AI.InvokeResetAttack();
        }
        else{
            BotAnimator.SetBool("shoot",false);
            BotAnimator.SetBool("idle", true);
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
