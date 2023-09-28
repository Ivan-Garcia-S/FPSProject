using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    public GameObject bullet;
    //public GameObject hitmarker;
    private PlayerState playerState;
    private Hitmarker hitmarker;
    
    [Header("Bullet Info")]
    public float damage = 26f;
    public float lifeSpan = 3f;
    public string enemyTeam;
    
    private void OnEnable() 
    {
        //Debug.Log("Bullet created");
        //hitmarker = GetComponentInParent<WeaponManager>().hitmark.gameObject;
        //if(hitmarker == null){
        //    Debug.Log("Hitmark null");
        //}
        ///playerState = GameObject.Find("Soldier_M_AR").GetComponent<PlayerState>();
        gameObject.transform.parent = null;
        
    }

    public void SetBulletInfo(string enemyTag, Hitmarker playerHitmarker)
    {
        enemyTeam = enemyTag;
        hitmarker = playerHitmarker;

    }
    private void Update() 
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Check if bulet hit enemy soldier
        if(collision.transform.tag == enemyTeam){
            Debug.Log("Enemy soldier hit");
            hitmarker.botHit3();
            collision.transform.GetComponentInParent<BotManager>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
