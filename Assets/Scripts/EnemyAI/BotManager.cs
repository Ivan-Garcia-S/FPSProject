using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    [Header("References")]
    public GameObject bot;
    public EnemyAI AI;
    public CapsuleCollider botCollider;
    public Animator animator;
    public GameObject destinationPointBox;
    public GameObject patrolPointObj;
    public Transform[] patrolPoints;
    public GameObject navDebugger;
    public GameManager Game;
    
    [Header("AI State")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    [HideInInspector]
    public float criticalState;
    private float baseHealthRegenPerSecond = 5f;
    private float healthRegenPerSecond;
    private float healthRegenAcceleration = 15f;
    private float durationTimer;
    private float criticalStateDuration = 3.25f;
    private float criticalStatePercent = 0.26f;
    private bool dead = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        Game = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        destinationPointBox = transform.Find("Destination Point").gameObject;
        destinationPointBox.SetActive(Game.showDestinationPointBox);
        navDebugger = GetComponentInChildren<LineRenderer>().gameObject;
        healthRegenPerSecond = baseHealthRegenPerSecond;
        criticalState = criticalStatePercent * maxHealth;
        botCollider = GetComponent<CapsuleCollider>();
        patrolPoints = new Transform[6];
        AI = GetComponent<EnemyAI>();
        animator = GetComponent<Animator>();
        for(int i = 0; i < patrolPoints.Length; i++){
            patrolPoints[i] = patrolPointObj.transform.GetChild(i);
        }

        //TESTING
        navDebugger.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Health regen
        if(currentHealth > criticalState){
            healthRegenPerSecond += Time.deltaTime * healthRegenAcceleration;
            currentHealth = Mathf.Min(100, currentHealth + healthRegenPerSecond * Time.deltaTime);
        }
        
        else{
            durationTimer += Time.deltaTime;
            if(durationTimer > criticalStateDuration){
                healthRegenPerSecond += Time.deltaTime * healthRegenAcceleration;
                currentHealth = Mathf.Min(100, currentHealth + Time.deltaTime * healthRegenPerSecond);
            }
        }
    }
    public void TakeDamage(float damage){
        //Take damage
        currentHealth -= damage;

        //Update score if bot killed
        if(currentHealth <=0 && !dead){
            navDebugger.SetActive(false);
            dead = true;
            Game.HandleAIDeath(bot);
        }

        //Reset health regen growth
        healthRegenPerSecond = baseHealthRegenPerSecond;

        //Reset timer for health regen
        durationTimer = 0;
    }
    //Correctly set state for animator to transition to
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
                animator.SetBool("reload",false);
                animator.SetBool("shoot",true);
                break;
            case "reload":
                animator.SetBool("shoot",false);
                animator.SetTrigger("reload");
                break;
            case "sprint":
                StopMovementExceptFor("moveForward");
                animator.SetBool("shoot", false);
                animator.SetBool("reload",false);
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
    //Set animator movement state
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
