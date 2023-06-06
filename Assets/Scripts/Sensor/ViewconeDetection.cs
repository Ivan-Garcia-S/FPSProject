using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewconeDetection : MonoBehaviour {

	public GameObject alertIcon;
	private string objectTag; 
	public List<GameObject> visibleEnemies;
	public EnemyAI AI;

	void Start () 
	{
		AI = gameObject.GetComponent<EnemyAI>();
		if(AI.enemyTag != null)  objectTag = AI.enemyTag; 
		alertIcon.SetActive (false);
		visibleEnemies = new List<GameObject>();
	}

	void Update() 
	{
		if(objectTag != AI.enemyTag){
			visibleEnemies.Clear();
			objectTag = AI.enemyTag;
			AI.currentEnemyTarget = null;
			alertIcon.SetActive(false);
		}

		if(visibleEnemies.Count > 0 && visibleEnemies[0] == null){
			visibleEnemies.RemoveAt(0);
		}
		
	}
	public void ObjectSpotted (Collider col) 
	{
		if(objectTag != null && col.CompareTag(objectTag))
		{
			RaycastHit newHit;
			Debug.DrawRay(transform.position, col.transform.position - transform.position);

			if(Physics.Raycast (new Ray(transform.position, col.transform.position - transform.position), out newHit))
			{
				if(newHit.collider.CompareTag(objectTag))
				{
					if(!visibleEnemies.Contains(col.gameObject))
					{
						visibleEnemies.Add(col.gameObject);
						if(AI.currentEnemyTarget == null){
							AI.currentEnemyTarget = col.gameObject.transform;
						}
					}

					Debug.LogWarning (objectTag + " player spotted.");
					alertIcon.SetActive (true);
				}
				else
				{
					Debug.LogWarning (objectTag + " player within viewcone, but is obstructed.");
					
				}
			}
		}
	}

	public void ObjectLeft (Collider col)
	{
		
		if(objectTag != null && col.CompareTag(objectTag)){	//If object left is an enemy
			alertIcon.SetActive (false);
			visibleEnemies.Remove(col.gameObject);	//Remove from list of visible enemies
			if(AI.currentEnemyTarget == col.gameObject.transform)
			{
				AI.currentEnemyTarget = visibleEnemies.Count > 0 ? visibleEnemies[0].transform : null;
			}
			if(visibleEnemies.Count == 0) alertIcon.SetActive (false);
		} 
		
	}
}