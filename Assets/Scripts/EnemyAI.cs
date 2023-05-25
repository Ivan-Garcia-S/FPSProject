using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;
    public Transform walkPointMarker;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    private Vector3 oldPosition;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public PlayerState state;

    public enum PlayerState
    {
        PATROLING,
        CHASING,
        ATTACKING
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        oldPosition = agent.transform.position;
    }

    void Start(){
        InvokeRepeating("MoveIfStuck", 3f,3f);
        
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
        
        if(walkPointSet) walkPointMarker.position = walkPoint;
        else walkPointMarker.position = new Vector3(-17f,23f,-9f);
    }

    private void Patroling()
    {
        state = PlayerState.PATROLING;
        Debug.Log("PATROLING");
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
       
        Vector3 randDirection = Random.insideUnitSphere * 8f;
 
        randDirection += transform.position;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, 8f, -1);
        walkPoint = navHit.position;
        walkPointSet = true;

        //Debug.Log("New Walkpoint = " + walkPoint);
        /*
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        Debug.Log("New Walkpoint = " + walkPoint);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
        */
    }

    private void ChasePlayer()
    {
        state = PlayerState.CHASING;
        walkPointSet = false;
        Debug.Log("CHASE");
        agent.SetDestination(player.position);
       
    }

    private void AttackPlayer()
    {
        state = PlayerState.ATTACKING;
        walkPointSet = false;
        Debug.Log("ATTACKING");
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            ///End of attack code

            alreadyAttacked = true;
            Invoke("ResetAttack", timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
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

    private void MoveIfStuck()
    {
        //Debug.Log("In moveifstuck current pos = " + GetPosition());
        //If stuck find new walkpoint to move to
        Vector3 currentPosition = GetPosition();
        if(currentPosition == oldPosition && state == PlayerState.PATROLING )
        {
            Debug.Log("Stuck Patroling");
            Debug.Log("Status is " + agent.pathStatus);
            SearchWalkPoint();
        }
        else if(currentPosition == oldPosition && state == PlayerState.CHASING){
            Debug.Log("Stuck Chasing");
            Debug.Log("Status is " + agent.pathStatus);
            Patroling();
        }
        oldPosition = currentPosition;
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