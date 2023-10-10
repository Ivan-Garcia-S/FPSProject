using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    //AI Properties
    public MultiAimConstraint headAim;
    public AIWeaponManager aiWM;
    public GameObject head;
    public GameManager GameManager;

    [Header("AI Detection")]
    //AI detection variables
    public NavMeshAgent agent;
    public ViewconeDetection detector;

    [Header("Enemy Info")]
    //Enemy transforms
    public GameObject[] enemies;
    public Transform currentEnemyTarget;

    public LayerMask whatIsGround, whatIsPlayer;
    public Transform walkPointMarker;

    [Header("AI State")]
    //State variables
    public float health;
    public string enemyTag;
    public bool isProne;
    public float sightRange, attackRange;
    [HideInInspector]
    public bool playerInSight, playerInAttackRange, playerAttackable;
    public PlayerState state;
    public AttackAction attackAction;

    [Header("Patroling")]
    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    [HideInInspector]
    
    //public float walkPointRange;
    private Vector3 oldPosition;
    public Transform[] patrolPoints;

    [Header("Chasing")]
    //Chasing
    public bool chasePointSet;

    [Header("Strafing")]
    //Strafing
    public Vector3 strafeDestination;

    [HideInInspector]
    public bool strafingRight;

    [Header("Hiding")]
    //Hiding
    public bool canInvokeStopHiding;

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
        ATTACKING,
        HIDING
    }

    /*public EnemyAI(string teamTag = "Team1")
    {
        myTag = teamTag;
    }
    */
    private void Awake()
    {
        attackAction = AttackAction.NONE;
        canInvokeStopHiding = true;
        head = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head").gameObject;
        //headAim = transform.Find("Rig 1/HeadAim_E").GetComponent<MultiAimConstraint>();
        //enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        agent = GetComponent<NavMeshAgent>();
        detector = GetComponentInChildren<ViewconeDetection>();
        patrolPoints = GameObject.Find("Patrol Points").GetComponentsInChildren<Transform>();
        oldPosition = agent.transform.position;
        currentEnemyTarget = null;
        aiWM = GetComponentInChildren<AIWeaponManager>();
    }

    private void Update()
    {
        //Always look at target when strafing
        if(attackAction == AttackAction.STRAFE)
        {
            Debug.Log("Strafing");
            transform.LookAt(currentEnemyTarget);
        }
        //Get enemies currently on map (NOT NECCESARY RN)
        //if(enemyTag != null)  enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        //Adjust for when prone
        if(isProne){
             head.transform.localEulerAngles = new Vector3(-68, head.transform.localEulerAngles.y, head.transform.localEulerAngles.z);//Quaternion.Euler(head.transform.rotation.x, head.transform.rotation.y, -63f);
        }
    }

    public void GetEnemies(string enemyTag)
    {
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
        //Toggle prone and set animator up accordingly
        isProne = !isProne;
        gameObject.GetComponent<Animator>().SetBool("prone", isProne);
    }

    public void GoProne()
    {
        //Go prone and set up animator accordingly
        isProne = true;
        gameObject.GetComponent<Animator>().SetBool("prone", true);
    }
    public void StopHiding()
    {
        //Debug.Log("StopHiding called");
        //First stand back up
        Prone();
        //Search for enemies again
        state = EnemyAI.PlayerState.PATROLING;
        canInvokeStopHiding = true;
    }
}