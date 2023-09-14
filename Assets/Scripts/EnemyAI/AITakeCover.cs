using UnityEngine;
using BehaviorTree;


public class AITakeCover : Node
{
    private EnemyAI AI;
    private Animator animator;
    private BotManager botManager;
    private float takeCoverRate = .05f;
    private Transform[] coverSpotTransforms;
    private Transform currentCoverPoint;
    private Vector3 soldierEyeHeight = new Vector3(0, 1.74f, 0);
    private float hideSpeed = 7f;
    private const float MAXLEVELLENGTH = 200f;

    public AITakeCover(BotManager bot)
    {
        AI = bot.AI;
        botManager = bot;
        animator = bot.animator;
        GameObject coverSpotParent = GameObject.Find("Cover Points");
        coverSpotTransforms = new Transform[6];
        int i = 0;
        foreach(Transform spot in coverSpotParent.transform){
            coverSpotTransforms[i++] = spot;
        }
    }
    
    public override NodeState Evaluate()
    {
        //If a cover spot has already been set
        if(AI.state == EnemyAI.PlayerState.HIDING)
        {
            //If the AI has reached the cover spot, stop hiding after a few seconds
            if(AI.canInvokeStopHiding && !AI.agent.hasPath)
            {
                botManager.SetAnimatorState("idle");
                AI.GoProne();
                AI.Invoke("StopHiding", 3f);
                AI.canInvokeStopHiding = false;
            }
            state = NodeState.SUCCESS;
        }
        else
        {
             //Hide only certain percent of the time when one shot
            if (Random.Range(0f, 1f) <= takeCoverRate)
            {
                AI.state = EnemyAI.PlayerState.HIDING;

                //AI.canInvokeStopHiding = true;
            
                //If there's a current target then run away from that target, else take cover at the nearest cover point
                if(AI.currentEnemyTarget != null)
                {
                    foreach(Transform coverSpot in coverSpotTransforms)
                    {
                        RaycastHit lineOfSightHit;
                        bool colliderHit = Physics.Raycast (new Ray(coverSpot.position + soldierEyeHeight, AI.currentEnemyTarget.position + soldierEyeHeight), out lineOfSightHit);

                        if(colliderHit && !lineOfSightHit.collider.CompareTag(AI.enemyTag))
                        {
                            currentCoverPoint = coverSpot;
                        }
                    }
                }
                else currentCoverPoint = FindNearestCoverPoint();
                
                //Set AI destination to the cover point
                botManager.SetAnimatorState("sprint");
                AI.agent.SetDestination(currentCoverPoint.position);
                AI.agent.speed = hideSpeed;
                state = NodeState.SUCCESS;
            }
            else state = NodeState.FAILURE;
        }
        return state;
    }

    //Find the closest cover point to the AI
    private Transform FindNearestCoverPoint()
    {
        Transform nearestPoint = null;
        float minDistanceToCover = MAXLEVELLENGTH;
        foreach(Transform coverSpot in coverSpotTransforms)
        {
            if(nearestPoint == null)
            {
                nearestPoint = coverSpot;
                minDistanceToCover = Vector3.Distance(coverSpot.position, AI.transform.position);
            }
            else
            {
                //Update closest cover point to AI
                float dist = Vector3.Distance(AI.transform.position, coverSpot.position);
                if(dist < minDistanceToCover)
                {
                    minDistanceToCover = dist;
                    nearestPoint = coverSpot;
                } 
            }
        }
        if(nearestPoint == null){
            Debug.LogError("Cover Point list is empty.");
        }
        return nearestPoint;
    }
}