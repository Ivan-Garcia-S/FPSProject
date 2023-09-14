using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AICheckPlayerInSight : Node
{
    public EnemyAI AI;
    public AICheckPlayerInSight(BotManager bot)
    {
        AI = bot.AI;
    }

    //Return if the AI currently spots an enemy
    public override NodeState Evaluate()
    {
        Transform oldTarget = null;
        if(AI.currentEnemyTarget != null) //If AI has target check to see if they're attackable first
        {
            oldTarget = AI.currentEnemyTarget;
            RaycastHit newHit;
            //Debug.DrawRay(gun.shootPoint.transform.position, 
            //enemySoldier.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2").transform.position - gun.shootPoint.transform.position);

            //Find out if there's a clear line of sight from the AI's gun to the enemy soldier
            if(Physics.Raycast (new Ray(AI.aiWM.shootPoint.transform.position, AI.currentEnemyTarget.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2").transform.position - AI.aiWM.shootPoint.transform.position), out newHit))
            {
                if(newHit.collider.CompareTag(AI.enemyTag)) 
                {
                    state = NodeState.SUCCESS;
                    return state;
                }

                else AI.currentEnemyTarget = null;
            }
            else AI.currentEnemyTarget = null;
        }

        if(AI.currentEnemyTarget == null)
        {
            foreach(GameObject soldier in AI.detector.visibleEnemies)
            {
                if(soldier != null && (oldTarget == null || soldier.transform != oldTarget))
                {
                    RaycastHit newHit;

                    //Find out if there's a clear line of sight from the AI's gun to the enemy soldier
                    if(Physics.Raycast (new Ray(AI.aiWM.shootPoint.transform.position, soldier.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2").transform.position - AI.aiWM.shootPoint.transform.position), out newHit))
                    {
                        if(newHit.collider.CompareTag(AI.enemyTag)) 
                        {
                            AI.currentEnemyTarget = soldier.transform;
                            state = NodeState.SUCCESS;
                            return state;
                        }
                    }
                }
            }
        }
        
        //if(AI.currentEnemyTarget != null) state = NodeState.SUCCESS;
        //else state = NodeState.FAILURE;
        
        state = NodeState.FAILURE;
        return state;
    }
}
