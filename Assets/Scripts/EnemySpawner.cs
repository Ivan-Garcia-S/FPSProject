using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject bot; 
    
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = new GameObject[6];

        for(int i = 0; i < spawnPoints.Length; i++){
            spawnPoints[i] = transform.GetChild(i).gameObject;
            spawnAt(i);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Spawn bot at specific spawn point
    private void spawnAt(int i)
    {

        Instantiate(bot, spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
        //bot.SendMessage("TeamSelect", teamName);
    }


}
