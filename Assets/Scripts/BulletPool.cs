using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{

    public static BulletPool Pool;
    public List<GameObject> pooledObjects;
    public GameObject enemyProjectile;
    public int amountToPool;
    
    void Awake()
    {
        Pool = this;
    }

    void Start()
    {
        /*pooledObjects = new List<GameObject>();
        GameObject temp;
        for(int i = 0; i < amountToPool; i++)
        {
            temp = Instantiate(enemyProjectile);
            temp.SetActive(false);
            pooledObjects.Add(temp);
        }
        */
    }

    public GameObject GetPooledBullet()
    {
        //Create temp variable in case more bullets are added
        int amount = amountToPool;
        for(int i = 0; i < amount; i++)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    public GameObject AddToPool(GameObject projectile)
    {
        GameObject bullet = Instantiate(projectile);
        pooledObjects.Add(bullet);
        amountToPool++;
        return bullet;
    }
}
