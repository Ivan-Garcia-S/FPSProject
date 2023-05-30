using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject bullet;
    public float damage = 20f;
    public float lifeSpan = 3f;

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
        if(collision.transform.tag != "Bot") Destroy(gameObject);
    }
}
