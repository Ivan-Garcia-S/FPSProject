using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2 : MonoBehaviour
{
    public GameObject bullet;
    public GameObject hitmark;
    public float damage = 20f;
    public float lifeSpan = 3f;

    private void OnEnable() 
    {
        //Debug.Log("Bullet created");
        //hitmark = GetComponentInParent<WeaponManager2>().hitmark;
        if(hitmark == null){
            Debug.Log("Hitmark null");
        }
        
        gameObject.transform.parent = null;
        
    }

    private void Update() 
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Check if Player touching ground
        Debug.Log("Bullet hit");
        if(collision.transform.tag == "Bot"){
            hitmark.GetComponent<Hitmarker>().botHit2();
            collision.transform.GetComponent<BotManager>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
