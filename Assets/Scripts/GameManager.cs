using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject ai;
    public GameObject startSpawnPoints;
    public int teamSize = 5;
    public string team1 = "Team1";
    public static string team2 = "Team2";
    

    
    // Start is called before the first frame update
    void Awake()
    {
        startSpawnPoints = GameObject.Find("Spawn Points");
        if(teamSize < 1){
            Debug.LogError("Team size cannot be less than 1, change teamSize in GameManager");
        }
    }

    void Start()
    {
        ////////////Testing/////////////
        //SpawnTeams();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Spawns in team 1 and 2 at start of game
    private void SpawnTeams(){
        SpawnTeamOne();
        SpawnTeamTwo();
    }
    private void SpawnTeamOne(){
        Transform teamOne = startSpawnPoints.transform.Find("Team1");
        for(int i = 1; i < teamSize +1; i++)
        {
            if(i == 1){
                GameObject playerObject = GameObject.Find("Player");
                if(playerObject == null){
                    Debug.Log("Player null");
                }
                playerObject.transform.position = teamOne.Find("Spawn1").transform.position;
                //SetPositionAndRotation(teamOne.Find("Spawn1").transform.position, teamOne.Find("Spawn1").transform.rotation); 
                //Instantiate(player, teamOne.Find("Spawn" + i));
                Debug.Log("Moved player");
            } 
            else{
                GameObject bot = Instantiate(ai, teamOne.Find("Spawn" + i)) as GameObject;
                bot.GetComponent<EnemyAI>().SendMessage("TeamName", team1);
                bot.transform.SetParent(null);// = null;
            }

        }
    }
    private void SpawnTeamTwo(){
        Transform teamTwo = startSpawnPoints.transform.Find("Team2");
        for(int i = 1; i < teamSize + 1; i++)
        {
            GameObject bot = Instantiate(ai, teamTwo.Find("Spawn" + i)) as GameObject;
            bot.GetComponent<EnemyAI>().SendMessage("TeamName", team2);
            bot.transform.parent = null; 
        }
    }
}
