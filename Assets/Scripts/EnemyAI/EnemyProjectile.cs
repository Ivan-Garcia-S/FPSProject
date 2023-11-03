using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject bullet;
    public float damage = 20f;
    public float lifeSpan = 3f;
    public string enemyTag;
    public string friendlyTag;
    public string sender;

    private void OnEnable() 
    { 
        transform.parent = null; 
        bullet = gameObject;
    }

    private void Update() 
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision) {

        try
        {
            Transform enemyHit = collision.transform.GetComponentInParent<CharacterController>().transform;
            
            if(enemyHit.tag == enemyTag)   
            {
                if(enemyHit.GetComponent<BotManager>() != null)
                    enemyHit.GetComponent<BotManager>().TakeDamage(damage);  //If bullet hits enemy bot do damage
                else if(enemyHit.GetComponent<PlayerState>() != null)
                    enemyHit.GetComponent<PlayerState>().TakeDamage(damage); // If bullet hits enemy player do damage
                else  Debug.LogWarning("Enemy object is unidentifiable");
            }
        
        }
        catch(NullReferenceException){
           // Debug.Log("Bullet collided with " + collision.gameObject.name);
        }
        //Destroy bullet unless it goes through a teammate with the same team tag
        
        if(collision.transform.tag != tag && collision.transform.tag != friendlyTag) Destroy(gameObject);  
    }

    public void SetBulletInfo(string[] senderInfo)
    {
        friendlyTag = senderInfo[0];
        enemyTag = (senderInfo[0] == "Team1") ? "Team2" : "Team1";
        sender = senderInfo[1];
    }
}
