using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewconeDetection : MonoBehaviour {

	public GameObject AlertIcon;
	private string objectTag; 
	public List<GameObject> VisibleEnemies;
	public EnemyAI AI;
	private AIWeaponManager Gun;
	private int layerMask;
	

	void Start () 
	{
		AI = gameObject.GetComponent<EnemyAI>();
		if(AI.enemyTag != null){
			objectTag = AI.enemyTag;
		}  
		else{
			Debug.Log("AI has no enemy TAG!!");
		}
		//alertIcon.SetActive (false);
		VisibleEnemies = new List<GameObject>();
		Gun = GetComponentInChildren<AIWeaponManager>();
		layerMask = LayerMask.GetMask("IgnoreAllButPlayer", "Ignore Raycast");
		layerMask = ~layerMask;
	}

	void Update() 
	{
		/*if(objectTag != AI.enemyTag){
			VisibleEnemies.Clear();
			objectTag = AI.enemyTag;
			AI.currentEnemyTarget = null;
			//alertIcon.SetActive(false);
		}
		*/

		//Remove enemies from list one by one if they are no longer visible
		/*if(VisibleEnemies.Count > 0 && VisibleEnemies[0] == null){
			VisibleEnemies.RemoveAt(0);
		}
		*/
		
	}
	//Called every instant a GameObject is touching the Viewcone
	//NO LONGER USED
	public void ObjectSpotted (Collider col) 
	{
		
		try
		{
			GameObject enemySoldier = col.GetComponentInParent<CharacterController>().gameObject;
			if(objectTag != null && enemySoldier.CompareTag(objectTag)) // Check if collider is attached to an enemy
			{
				RaycastHit newHit;
				//Debug.DrawRay(gun.shootPoint.transform.position, 
				//enemySoldier.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2").transform.position - gun.shootPoint.transform.position);

				//Find out if there's a clear line of sight from the AI's gun to the enemy soldier
				if(Physics.Raycast (new Ray(Gun.shootPoint.transform.position, enemySoldier.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2").transform.position - Gun.shootPoint.transform.position), out newHit, 200f,layerMask))
				{
					Debug.Log("Layer is " + newHit.collider.gameObject.layer);
					if(newHit.collider.CompareTag(objectTag)) 
					{
						if(!VisibleEnemies.Contains(enemySoldier)) //Add enemy to the list of enemies visible to the AI
						{
							VisibleEnemies.Add(enemySoldier);
							if(AI.currentEnemyTarget == null) AI.currentEnemyTarget = enemySoldier.transform;
						}
						//Debug.LogWarning (objectTag + " player spotted.");
						//alertIcon.SetActive (true);
					}
					else
					{
						//Debug.LogWarning (objectTag + " player within viewcone, but is obstructed.");
						if(enemySoldier.transform == AI.currentEnemyTarget) //Remove enemy from the list of visible enemies to the AI if the view is obstructed
						{
							Transform oldTarget = AI.currentEnemyTarget;
							VisibleEnemies.Remove(enemySoldier);
							SetNewTargetEnemy();
							if(AI.currentEnemyTarget == oldTarget)  AI.currentEnemyTarget = null;
						}
					}
				}
			}
		}
		catch(NullReferenceException){}
		
	}

	public void SoldierEntered(GameObject soldier)
	{
		if(objectTag != null && soldier.CompareTag(objectTag)) // Check if collider is attached to an enemy
		{
			if(!VisibleEnemies.Contains(soldier)) //Add enemy to the list of enemies visible to the AI
			{
				VisibleEnemies.Add(soldier);
			}
		}
	}

	public void SoldierLeft(GameObject soldier)
	{
		if(objectTag != null && soldier.CompareTag(objectTag)) // Check if collider is attached to an enemy
		{
			VisibleEnemies.Remove(soldier);	//Remove enemy from list of visible enemies if they leave the Viewcone
			if(AI.currentEnemyTarget == soldier.transform)
			{
				//Debug.Log("removed " + soldier.name + " as current enemy target bc left viewcone");
				AI.currentEnemyTarget = null;
			}
		}
	}
	
	//Called when a GameObject with a collider leaves the Viewcone
	public void ObjectLeft (Collider col)
	{
		try
		{
			GameObject enemySoldier = col.GetComponentInParent<CharacterController>().gameObject;
			//Check if collider is attached to an enemy soldier
			if(objectTag != null && enemySoldier.CompareTag(objectTag)){	
			//alertIcon.SetActive (false);
			VisibleEnemies.Remove(enemySoldier);	//Remove enemy from list of visible enemies if they leave the Viewcone
			if(AI.currentEnemyTarget == enemySoldier.transform)
			{
				Transform oldTarget = AI.currentEnemyTarget;
				SetNewTargetEnemy();
				if(AI.currentEnemyTarget == oldTarget)  AI.currentEnemyTarget = null;
			}
			if(VisibleEnemies.Count == 0) AlertIcon.SetActive (false);
		} 
		}
		catch(NullReferenceException){}

		
		
	}

	//Set a new enemy target if the old target leaves the viewcone
	private void SetNewTargetEnemy()
	{
		foreach(GameObject visibleEnemy in VisibleEnemies)
		{
			if(visibleEnemy != null)
			{
				AI.currentEnemyTarget = visibleEnemy.transform;
				return;
			} 
		}
		return;
	}

	public int GetLayerMask()
	{
		return layerMask;
	}

	private void OnDrawGizmos() {
		 Gizmos.color = Color.green;
		 Gizmos.DrawWireSphere(AI.transform.position, AI.attackRange);
		 Gizmos.color = Color.red;
		 Gizmos.DrawWireSphere(AI.transform.position, AI.attackRange + 2);
	}
}