using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public AudioSource FootAudioSource;
    public AudioClip WalkSound;
    public AudioClip SprintSound;
    
    [Header("AI State")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    private float currentDistanceFromPoint = -1;

    [HideInInspector]
    public float criticalState;
    public float stoppingDistance;
    private float baseHealthRegenPerSecond = 5f;
    private float healthRegenPerSecond;
    private float healthRegenAcceleration = 15f;
    private float durationTimer;
    private float criticalStateDuration = 3.25f;
    private float criticalStatePercent = 0.26f;
    private bool dead = false;
    private float normalSpeed = 4f;
    private float crouchProneSpeedMult = 0.5f;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        Game = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        destinationPointBox = transform.Find("Destination Point").gameObject;
        destinationPointBox.SetActive(Game.showDestinationPointBox);
        //navDebugger = GetComponentInChildren<LineRenderer>().gameObject;
        healthRegenPerSecond = baseHealthRegenPerSecond;
        criticalState = criticalStatePercent * maxHealth;
        botCollider = GetComponent<CapsuleCollider>();
        patrolPointObj = GameObject.FindWithTag("PatrolPoints");
        patrolPoints = new Transform[patrolPointObj.transform.childCount];
        AI = GetComponent<EnemyAI>();
        animator = GetComponent<Animator>();
        stoppingDistance = GetComponent<NavMeshAgent>().stoppingDistance;
        for(int i = 0; i < patrolPoints.Length; i++){
            patrolPoints[i] = patrolPointObj.transform.GetChild(i);
        }

        //TESTING
        //navDebugger.SetActive(true);
    }

    void Start() 
    {
        InvokeRepeating("MoveIfStuck", 5.0f, 3f);
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

        //Play walk and sprint audio
        if(animator.GetBool("run")){
            if(FootAudioSource.isPlaying && FootAudioSource.clip != SprintSound){
                FootAudioSource.Stop();
                FootAudioSource.clip = SprintSound;
                FootAudioSource.Play();
            }
            else if(FootAudioSource.clip != SprintSound){
                FootAudioSource.clip = SprintSound;
                FootAudioSource.Play();
            }
        }
        else if(!animator.GetBool("idle")){
            if(FootAudioSource.isPlaying && FootAudioSource.clip != WalkSound){
                FootAudioSource.Stop();
                FootAudioSource.clip = WalkSound;
                FootAudioSource.Play();
            }
            else if(FootAudioSource.clip != WalkSound){
                FootAudioSource.clip = WalkSound;
                FootAudioSource.Play();
            }
        }
    }
    //Returns if bot was killed with this bullet
    public bool TakeDamage(float damage, PlayerState state= null){
        //Take damage
        currentHealth -= damage;

        //Update score if bot killed
        if(currentHealth <=0 && !dead){
            dead = true;
            //navDebugger.SetActive(false);
            Debug.Log("Bot has no health"); 
            if(state) state.AddKill();
            Game.HandleAIDeath(bot);
            return true;
        }

        //Reset health regen growth
        healthRegenPerSecond = baseHealthRegenPerSecond;

        //Reset timer for health regen
        durationTimer = 0;
        return false;
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

    void MoveIfStuck()
    {
        if(currentDistanceFromPoint == -1) currentDistanceFromPoint = AI.agent.remainingDistance;
        else if (AI.agent.remainingDistance == currentDistanceFromPoint) AI.agent.SetDestination(patrolPoints[6].position); 
        
    }
    public float GetCrouchProneSpeed()
    {
        return normalSpeed * crouchProneSpeedMult;
    }

    
}
