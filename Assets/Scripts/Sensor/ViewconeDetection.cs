using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewconeDetection : MonoBehaviour {

	public GameObject alertIcon;
	private const string ObjectTag = "Player"; 
	public Transform character;

	void Start () 
	{
		alertIcon.SetActive (false);
		if (character == null)
		{
			Debug.LogError (ObjectTag + " viewcone character property is not set!");
		}
	}

	public void ObjectSpotted (Collider col) 
	{
		if(col.CompareTag(ObjectTag))
		{
			RaycastHit newHit;
			Debug.DrawRay(transform.position, col.transform.position - transform.position);

			if(Physics.Raycast (new Ray(transform.position, col.transform.position - transform.position), out newHit))
			{
				if(newHit.collider.CompareTag(ObjectTag))
				{
					Debug.LogWarning (ObjectTag + " spotted by " + character.name + ".");
					alertIcon.SetActive (true);
				}
				else
				{
					Debug.Log (ObjectTag + " within viewcone of " + character.name + ", but is obstructed.");
					alertIcon.SetActive (false);
				}
			}
		}
	}

	public void ObjectLeft (Collider col)
	{
		if(col.CompareTag(ObjectTag)) alertIcon.SetActive (false);
	}
}