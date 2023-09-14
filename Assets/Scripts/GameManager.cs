using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;
    public GameObject AISoldierT1;
    public GameObject AISoldierT2;
    private GameObject StartSpawnPoints;
    public Transform Scoreboard;

    public TextMeshProUGUI ScoreT1;
    public TextMeshProUGUI ScoreT2;
    
    [HideInInspector]
    private int teamSize = 5;
    private static string team1 = "Team1";
    private static string team2 = "Team2";
    private int teamOneScore = 0;
    private int teamTwoScore = 0;
    

    
    // Start is called before the first frame update
    void Awake()
    {
        Scoreboard = GameObject.FindWithTag("Scoreboard").transform;
        StartSpawnPoints = GameObject.Find("Spawn Points");
        if(teamSize < 1){
            Debug.LogError("Team size cannot be less than 1, change teamSize in GameManager");
        }
    }

    void Start()
    {
        //T1Score = Scoreboard.Find("Team1/Score").GetComponent<TMP_Text>();//Find("Team1/Score").GetComponent<TextMeshPro>();
        //T2Score = Scoreboard.Find("Team2/Score").GetComponent<TMP_Text>();
        ////////////Testing/////////////
        SpawnTeams();
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
        Transform teamOne = StartSpawnPoints.transform.Find("Team1");
        for(int i = 1; i < teamSize +1; i++)
        {
            if(i == 1){
                GameObject playerObject = GameObject.Find("Soldier_M_AR");
                if(playerObject == null){
                    Debug.Log("Player null");
                }
                CharacterController playerController = playerObject.GetComponent<CharacterController>();
                playerController.enabled = false;
                playerObject.transform.position = teamOne.Find("Spawn1").transform.position;
                playerController.enabled = true;
                //SetPositionAndRotation(teamOne.Find("Spawn1").transform.position, teamOne.Find("Spawn1").transform.rotation); 
                //Instantiate(Player, teamOne.Find("Spawn" + i));
                Debug.Log("Moved player");
            } 
            else{
                Transform spawnPt = teamOne.Find("Spawn" + i).transform;
                GameObject bot = Instantiate(AISoldierT1, spawnPt.position, spawnPt.rotation) as GameObject;
                bot.GetComponent<EnemyAI>().SendMessage("GetEnemies", team2);
                bot.transform.SetParent(null);// = null;
            }

        }
    }
    private void SpawnTeamTwo(){
        Transform teamTwo = StartSpawnPoints.transform.Find("Team2");
        for(int i = 1; i < teamSize + 1; i++)
        {
            Transform spawnPt = teamTwo.Find("Spawn" + i).transform;
            GameObject bot = Instantiate(AISoldierT2, spawnPt.position, spawnPt.rotation) as GameObject;
            bot.GetComponent<EnemyAI>().SendMessage("GetEnemies", team1);
            bot.transform.parent = null; 
        }
    }

    public void UpdateScore(string teamKilled)
    {
        if(teamKilled == team1)
        {
            teamTwoScore += 1;
            ScoreT2.SetText("- " + teamTwoScore.ToString());
        }
        else if(teamKilled == team2)
        {
            teamOneScore += 1;
            ScoreT1.SetText("- " + teamOneScore.ToString());
        }
        else{
            Debug.LogWarning("Invalid Team name entered");
        }
    }
}
