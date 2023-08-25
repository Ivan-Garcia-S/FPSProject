using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyAI : MonoBehaviour
{
    //AI detection variables
    public NavMeshAgent agent;
    public ViewconeDetection detector;

    //Enemy transforms
    public GameObject[] enemies;
    public Transform currentEnemyTarget;

    public LayerMask whatIsGround, whatIsPlayer;
    public Transform walkPointMarker;

    //State variables
    public float health;
    public string enemyTag;
    public bool isProne;

    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    private Vector3 oldPosition;
    public Transform[] patrolPoints;

    //Chasing
    public bool chasePointSet;

    //Strafing
    public Vector3 strafeDestination;
    public bool strafingRight;


    //States
    public float sightRange, attackRange;
    public bool playerInSight, playerInAttackRange, playerAttackable;
    public PlayerState state;
    public AttackAction attackAction;

    //AI Properties
    public MultiAimConstraint headAim;
    public AIWeaponManager aiWM;
    public GameObject head;

    public enum AttackAction
    {
        DROPSHOT,
        STRAFE,
        NONE
    }
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
        attackAction = AttackAction.NONE;
        head = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head").gameObject;
        //headAim = transform.Find("Rig 1/HeadAim_E").GetComponent<MultiAimConstraint>();
        enemyTag = (tag == "Team1") ? "Team2" : "Team1";
        enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        agent = GetComponent<NavMeshAgent>();
        detector = GetComponentInChildren<ViewconeDetection>();
        patrolPoints = GameObject.Find("Patrol Points").GetComponentsInChildren<Transform>();
        oldPosition = agent.transform.position;
        currentEnemyTarget = null;
        aiWM = GetComponentInChildren<AIWeaponManager>();
    }

    private void Update()
    {
        if(attackAction == AttackAction.STRAFE)
        {
            transform.LookAt(currentEnemyTarget);
        }
        if(enemyTag != null)  enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        if(isProne){
             head.transform.localEulerAngles = new Vector3(-68, head.transform.localEulerAngles.y, head.transform.localEulerAngles.z);//Quaternion.Euler(head.transform.rotation.x, head.transform.rotation.y, -63f);
        }
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

    public void Prone()
    {
        //Either prone or stand up and set animator up accordingly
        isProne = !isProne;
        gameObject.GetComponent<Animator>().SetBool("prone", isProne);
        if(isProne) 
        {
           
            //Debug.Log("Prone = true");
            //GetComponent<BotManager>().botCollider.direction = 2;
        }
        else 
        {
            //Debug.Log("Prone = false");
            //GetComponent<BotManager>().botCollider.direction = 1;
        }
    }
}