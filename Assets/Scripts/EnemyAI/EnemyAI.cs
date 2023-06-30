using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //AI detection variables
    public NavMeshAgent agent;
    public AISensor sensor;
    public ViewconeDetection detector;

    //Enemy transforms
    public GameObject[] enemies;
    public Transform currentEnemyTarget;

    public LayerMask whatIsGround, whatIsPlayer;
    public Transform walkPointMarker;

    //State variables
    public float health;
    public string enemyTag;

    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    private Vector3 oldPosition;
    public Transform[] patrolPoints;

    //Chasing
    public bool chasePointSet;

    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public GameObject projectile;
    public float spread;
    public float upwardForce;

    //States
    public float sightRange, attackRange;
    public bool playerInSight, playerInAttackRange, playerAttackable;
    public PlayerState state;

    public enum PlayerState
    {
        PATROLING,
        CHASING,
        ATTACKING
    }

    /*public EnemyAI(string teamTag = "Team1")
    {
        myTag = teamTag;
    }
    */
    private void Awake()
    {
        enemyTag = (tag == "Team1") ? "Team2" : "Team1";
        enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        agent = GetComponent<NavMeshAgent>();
        sensor = GetComponent<AISensor>();
        detector = GetComponentInChildren<ViewconeDetection>();
        patrolPoints = GameObject.Find("Patrol Points").GetComponentsInChildren<Transform>();
        oldPosition = agent.transform.position;
        spread = 0.1f;
        upwardForce = 0f;
        currentEnemyTarget = null;
    }

    private void Update()
    {
        if(enemyTag != null)  enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        //Remove for testing
        /*
        //Check for sight and attack range  
        playerInSight = detector.alertIcon.activeInHierarchy;
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSight) Patroling();
        else if (playerInSight && !playerInAttackRange) ChasePlayer();
        else if (playerInSight && playerInAttackRange) AttackPlayer();
        
        

        if(walkPointSet) walkPointMarker.position = walkPoint;
        else walkPointMarker.position = new Vector3(-17f,23f,-9f);
        */
    }

    public void TeamName(string team)
    {
        gameObject.tag = team;
        enemyTag = (tag == "Team1") ? "Team2" : "Team1";
        enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        
        Material blue = Resources.Load("Materials/Blue", typeof(Material)) as Material;
        if(!blue)  Debug.LogWarning("Material not found");
        if(tag == "Team1")  gameObject.GetComponent<Renderer>().material = blue;

    }
    
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void InvokeResetAttack()
    {
        Invoke("ResetAttack", timeBetweenAttacks);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke("DestroyEnemy", 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}