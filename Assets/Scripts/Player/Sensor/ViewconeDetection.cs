using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewconeDetection : MonoBehaviour {

	public GameObject alertIcon;
	private string objectTag; 
	public List<GameObject> visibleEnemies;
	public EnemyAI AI;
	private AIWeaponManager gun;

	void Start () 
	{
		AI = gameObject.GetComponent<EnemyAI>();
		if(AI.enemyTag != null)  objectTag = AI.enemyTag;
		alertIcon.SetActive (false);
		visibleEnemies = new List<GameObject>();
		gun = GetComponentInChildren<AIWeaponManager>();
	}

	void Update() 
	{
		if(objectTag != AI.enemyTag){
			visibleEnemies.Clear();
			objectTag = AI.enemyTag;
			AI.currentEnemyTarget = null;
			alertIcon.SetActive(false);
		}

		//Remove enemies from list one by one if they are no longer visible
		if(visibleEnemies.Count > 0 && visibleEnemies[0] == null){
			visibleEnemies.RemoveAt(0);
		}
		
	}
	//Called every instant a GameObject is touching the Viewcone
	public void ObjectSpotted (Collider col) 
	{
		//OLD//If object spotted is an enemy soldier
		//if(objectTag != null && col.CompareTag(objectTag))
		
		//NEW//Check if layer == "WhatIsPlayer"
		try
		{
			GameObject enemySoldier = col.GetComponentInParent<CharacterController>().gameObject;
			if(objectTag != null && enemySoldier.CompareTag(objectTag))
			{
				RaycastHit newHit;
				Debug.DrawRay(gun.shootPoint.transform.position, 
				enemySoldier.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2").transform.position - gun.shootPoint.transform.position);

				//Find out if there's a clear line of sight from the AI's gun to the enemy soldier
				if(Physics.Raycast (new Ray(gun.shootPoint.transform.position, enemySoldier.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2").transform.position - gun.shootPoint.transform.position), out newHit))
				{
					if(newHit.collider.CompareTag(objectTag))
					{
						if(!visibleEnemies.Contains(enemySoldier))
						{
							visibleEnemies.Add(enemySoldier);
							if(AI.currentEnemyTarget == null) AI.currentEnemyTarget = enemySoldier.transform;
						}
						Debug.LogWarning (objectTag + " player spotted.");
						alertIcon.SetActive (true);
					}
					else
					{
						Debug.LogWarning (objectTag + " player within viewcone, but is obstructed.");
						if(enemySoldier.transform == AI.currentEnemyTarget)
						{
							Transform oldTarget = AI.currentEnemyTarget;
							visibleEnemies.Remove(enemySoldier);
							SetNewTargetEnemy();
							if(AI.currentEnemyTarget == oldTarget)  AI.currentEnemyTarget = null;
						}
					}
				}
			}
		}
		catch(NullReferenceException){}
		
	}
	//Called when a GameObject with a collider leaves the Viewcone
	public void ObjectLeft (Collider col)
	{
		try
		{
			GameObject enemySoldier = col.GetComponentInParent<CharacterController>().gameObject;
			//Check if collider is attached to an enemy soldier
			if(objectTag != null && enemySoldier.CompareTag(objectTag)){	
			alertIcon.SetActive (false);
			visibleEnemies.Remove(enemySoldier);	//Remove enemy from list of visible enemies if they leave the Viewcone
			if(AI.currentEnemyTarget == enemySoldier.transform)
			{
				Transform oldTarget = AI.currentEnemyTarget;
				SetNewTargetEnemy();
				if(AI.currentEnemyTarget == oldTarget)  AI.currentEnemyTarget = null;
			}
			if(visibleEnemies.Count == 0) alertIcon.SetActive (false);
		} 
		}
		catch(NullReferenceException){}

		
		
	}

	private void SetNewTargetEnemy()
	{
		foreach(GameObject visibleEnemy in visibleEnemies)
		{
			if(visibleEnemy != null)
			{
				AI.currentEnemyTarget = visibleEnemy.transform;
				return;
			} 
		}
		return;
	}
}