using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public GameObject bot;
    public EnemyAI AI;
    public CapsuleCollider botCollider;
    public Animator animator;
    public GameObject destinationPointBox;
    public GameObject patrolPointObj;
    public Transform[] patrolPoints;
    public float health = 100f;
    
    // Start is called before the first frame update
    void Awake()
    {
        botCollider = GetComponent<CapsuleCollider>();
        patrolPoints = new Transform[6];
        AI = GetComponent<EnemyAI>();
        animator = GetComponent<Animator>();
        for(int i = 0; i < patrolPoints.Length; i++){
            patrolPoints[i] = patrolPointObj.transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float damage){
        health -= damage;
        if(health <=0 ){
            Destroy(bot);
        }
    }

    public void SetAnimatorState(string stateToSet, bool changeProne=false, bool changeCrouch=false, bool changeSprint=false)
    {
        switch(stateToSet){
            case "moveForward":
                StopMovementExceptFor(stateToSet);
                break;
            case "strafeLeft":
                animator.SetBool("run", false);
                StopMovementExceptFor(stateToSet);
                break;
            case "strafeRight":
                animator.SetBool("run", false);
                StopMovementExceptFor(stateToSet);
                break;
            case "moveBack":
                animator.SetBool("run", false);
                StopMovementExceptFor(stateToSet);
                break;
            case "idle":
                animator.SetBool("run", false);
                StopMovementExceptFor(stateToSet);
                break;
            case "shoot":
                animator.SetBool("run", false);
                animator.SetTrigger("stopReload");
                animator.SetBool("shoot",true);
                break;
            case "reload":
                animator.SetBool("shoot",false);
                animator.SetTrigger("reload");
                break;
            case "sprint":
                StopMovementExceptFor("moveForward");
                animator.SetBool("shoot", false);
                animator.SetTrigger("stopReload");
                animator.SetBool("run", true);
                break;
            default:
                Debug.LogWarning("New AI state not found");
                break;

            
        }
        if(changeProne) animator.SetBool("prone", !animator.GetBool("prone"));    
        if(changeCrouch) animator.SetBool("crouched", !animator.GetBool("crouched")); 
        if(changeSprint) animator.SetBool("run", !animator.GetBool("run")); 
           
    }

    public void StopMovementExceptFor(string exception)
    {
        animator.SetBool("strafeRight", false);
        animator.SetBool("moveForward", false);
        animator.SetBool("strafeLeft", false);
        animator.SetBool("moveBack", false);
        animator.SetBool("idle", false);
        animator.SetBool(exception, true);
    }
}
