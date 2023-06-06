using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject bullet;
    public float damage = 20f;
    public float lifeSpan = 3f;
    public string enemyTag;

    private void OnEnable() 
    { 
        gameObject.transform.parent = null; 
        bullet = gameObject;
    }

    private void Update() 
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        
        //if(collision.transform.tag != "Bot") Destroy(gameObject);
    }

    void OnCollisionStay(Collision collision) {
        if(collision.transform.tag == enemyTag)   
        {
            if(collision.transform.GetComponent<BotManager>() != null)
                collision.transform.GetComponent<BotManager>().TakeDamage(damage);  //If bullet hits enemy bot do damage
            else if(collision.transform.GetComponent<PlayerState>() != null)
                collision.transform.GetComponent<PlayerState>().TakeDamage(damage); // If bullet hits enemy playyer do damage
            else  Debug.LogWarning("Enemy object is unidentifiable");
        }
            
        if(collision.transform.tag != tag) Destroy(gameObject);  
    }

    public void SetBulletInfo(string myTag)
    {
        tag = myTag;
        enemyTag = (myTag == "Team1") ? "Team2" : "Team1";
    }
}
