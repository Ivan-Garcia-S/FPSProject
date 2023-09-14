using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    public GameObject bullet;
    public GameObject hitmarker;
    private PlayerState playerState;
    
    [Header("Bullet Info")]
    public float damage = 26f;
    public float lifeSpan = 3f;
    
    private void OnEnable() 
    {
        //Debug.Log("Bullet created");
        //hitmarker = GetComponentInParent<WeaponManager>().hitmark.gameObject;
        if(hitmarker == null){
            Debug.Log("Hitmark null");
        }
        playerState = GameObject.Find("Soldier_M_AR").GetComponent<PlayerState>();
        gameObject.transform.parent = null;
        
    }

    private void Update() 
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Check if bulet hit enemy soldier
        //Debug.Log("Collision is " + collision.transform.name);

        if(collision.transform.tag == playerState.enemyTag){
            Debug.Log("Enemy soldier hit");
            //hitmarker.GetComponent<Hitmarker>().botHit2(); ////////NEED TO ADD BACK/////
            collision.transform.GetComponentInParent<BotManager>().TakeDamage(damage);
        }
        //else Debug.Log("Tag of obj hit = " + collision.transform.tag);
        Destroy(gameObject);
    }
}
