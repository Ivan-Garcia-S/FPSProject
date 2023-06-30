using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public GameObject bot;
    public EnemyAI AI;
    public Animator animator;
    public GameObject destinationPointBox;
    public GameObject patrolPointObj;
    public Transform[] patrolPoints;
    public float health = 100f;
    
    // Start is called before the first frame update
    void Awake()
    {
        patrolPoints = new Transform[6];
        AI = GetComponent<EnemyAI>();
        animator = GetComponent<Animator>();
        for(int i = 0; i < patrolPoints.Length; i++){
            patrolPoints[i] = patrolPointObj.transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float damage){
        health -= damage;
        if(health <=0 ){
            Destroy(bot);
        }
    }
}
