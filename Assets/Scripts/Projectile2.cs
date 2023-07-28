using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2 : MonoBehaviour
{
    public GameObject bullet;
    public GameObject hitmark;
    public float damage = 20f;
    public float lifeSpan = 999f;
    private PlayerState playerState;

    private void OnEnable() 
    {
        //Debug.Log("Bullet created");
        //hitmark = GetComponentInParent<WeaponManager2>().hitmark;
        if(hitmark == null){
            Debug.Log("Hitmark null");
        }
        playerState = GameObject.Find("Player").GetComponentInChildren<PlayerState>();
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
        if(collision.transform.tag == playerState.enemyTag){
            hitmark.GetComponent<Hitmarker>().botHit2();
            collision.transform.GetComponentInParent<BotManager>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
