using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public AISensor sensor;
    private ViewconeDetection detector;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;
    public Transform walkPointMarker;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    private Vector3 oldPosition;
    public Transform[] patrolPoints;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    private float spread;
    private float upwardForce;

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

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        sensor = GetComponent<AISensor>();
        detector = GetComponentInChildren<ViewconeDetection>();
        patrolPoints = GameObject.Find("Patrol Points").GetComponentsInChildren<Transform>();
        foreach(Transform point in patrolPoints) Debug.Log(point.position);
        oldPosition = agent.transform.position;
        spread = 0.1f;
        upwardForce = 0f;
    }

    void Start(){
        InvokeRepeating("MoveIfStuck", 3f,3f);
        //agent.destination = patrolPoints[2].position;
    }

    private void Update()
    {
        /*
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerAttackable = sensor.IsInSight(player.gameObject);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        else if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        else if (playerAttackable) AttackPlayer();
        else ChasePlayer();
        */
         //Check for sight and attack range
       
       
       
        //Remove for testing
        
        playerInSight = detector.alertIcon.activeInHierarchy;
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSight) Patroling();
        else if (playerInSight && !playerInAttackRange) ChasePlayer();
        else if (playerInSight && playerInAttackRange) AttackPlayer();
        
        

        if(walkPointSet) walkPointMarker.position = walkPoint;
        else walkPointMarker.position = new Vector3(-17f,23f,-9f);
        
        //GoToPoints();
    }
    private void GoToPoints()
    {
        foreach(Transform point in patrolPoints)
        {
            agent.destination = point.position;

            Vector3 distanceToWalkPoint = agent.transform.position - point.position;
            while(distanceToWalkPoint.magnitude > 1.5f){
                distanceToWalkPoint = agent.transform.position - point.position;
            }
        }
    }
    private void Patroling()
    {
        state = PlayerState.PATROLING;
        Debug.Log("PATROLING");
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && !agent.hasPath)

            agent.destination = walkPoint;//SetDestination(walkPoint);
            Debug.Log("Path is " + agent.pathStatus);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            Debug.Log("Within close proximity of point");
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Old walkpoint method
        Vector3 randDirection = Random.insideUnitSphere * 20f;

        randDirection += transform.position;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, 20f, -1);
        walkPoint = navHit.position;
        walkPointSet = true;

        //Debug.Log("New Walkpoint = " + walkPoint);
    
        //New walkpoint method
        /*int pointNum = Random.Range(0, patrolPoints.Length);
        Debug.Log("Point" + (pointNum + 1) + " chosen");
        walkPoint = patrolPoints[pointNum].position;
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
            Vector3 shootPoint = transform.Find("pistol3/Shoot Point").transform.position;
            //Calculate direction from attackPoint to the targetPoint
            Vector3 directionNoSpread = player.position - shootPoint;

             //Calculate spread
            float spreadX = Random.Range(-spread, spread);
            float spreadY = Random.Range(-spread, spread);

            //Direction with spread
            Vector3 directionWithSpread = directionNoSpread + new Vector3(spreadX,spreadY,0); 
            
            ///Attack code here
            Rigidbody rb = Instantiate(projectile, shootPoint, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(directionWithSpread.normalized * 100f, ForceMode.Impulse);
            rb.AddForce(transform.Find("pistol3").transform.up * upwardForce, ForceMode.Impulse);
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