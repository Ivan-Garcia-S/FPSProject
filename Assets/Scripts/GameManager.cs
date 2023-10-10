using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;
    public GameObject AISoldierT1;
    public GameObject AISoldierT2;
    private GameObject StartSpawnPoints;
    public Transform Scoreboard;
    public PlayerCamera MainCam;
    public PlayerInputActions PlayerInputActions;

    public TextMeshProUGUI ScoreT1;
    public TextMeshProUGUI ScoreT2;
    public bool showDestinationPointBox = false;
    
    [HideInInspector]
    private int teamSize = 1;
    private string playerName = "Soldier_M_AR";
    private static string team1 = "Team1";
    private static string team2 = "Team2";
    private int teamOneScore = 0;
    private int teamTwoScore = 0;
    private float respawnDelay = 2f;
    private float torsoHeight = 1.15f;
    public List<GameObject> activeTeam1Soldiers = new List<GameObject>();
    public List<GameObject> activeTeam2Soldiers = new List<GameObject>();
    private System.Random rand = new System.Random();
    public List<Transform> respawnPoints = new List<Transform>();
    

    
    // Start is called before the first frame update
    void Awake()
    {
        Scoreboard = GameObject.FindWithTag("Scoreboard").transform;
        StartSpawnPoints = GameObject.Find("Spawn Points");
        PlayerInputActions = new PlayerInputActions();
        MainCam = GameObject.FindWithTag("MainCamera").GetComponent<PlayerCamera>();
        Transform respawnParent = StartSpawnPoints.transform.Find("Respawn Points");
        for(int i = 0; i < respawnParent.childCount; i++)
        {
            respawnPoints.Add(respawnParent.GetChild(i));
        }
        //Debug.Log("Repawn count is" + respawnPoints.Count);
        if(teamSize < 1){
            Debug.LogError("Team size cannot be less than 1, change teamSize in GameManager");
        }
        NavMesh.avoidancePredictionTime = 4;
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
       // Transform spawn = respawnPoints[0];
        //Debug.DrawRay(spawn.GetChild(0).transform.position, activeTeam1Soldiers[1].transform.position + Vector3.up * 1.15f - spawn.GetChild(0).transform.position);
    }

    //Spawns in team 1 and 2 at start of game
    private void SpawnTeams(){
        SpawnTeamOne();
        SpawnTeamTwo();
    }

    private void SpawnBot(Transform spawn, string teamToSpawn)
    {
        if(teamToSpawn == team1)
        {
            GameObject bot = Instantiate(AISoldierT1, spawn.position, spawn.rotation) as GameObject;
            activeTeam1Soldiers.Add(bot);
            bot.GetComponent<EnemyAI>().SendMessage("GetEnemies", team2);
            bot.transform.SetParent(null);// = null;
        }
        else if(teamToSpawn == team2)
        {
            GameObject bot = Instantiate(AISoldierT2, spawn.position, spawn.rotation) as GameObject;
            activeTeam2Soldiers.Add(bot);
            bot.GetComponent<EnemyAI>().SendMessage("GetEnemies", team1);
            bot.transform.SetParent(null);// = null;
        }
        else{
            Debug.LogError("invalid team to spawn");
        }
        
    }
    private void SpawnTeamOne(){
        Transform teamOne = StartSpawnPoints.transform.Find(team1);
        for(int i = 1; i < teamSize +1; i++)
        {
            if(i == 1){
                GameObject playerObject = GameObject.Find(playerName);
                activeTeam1Soldiers.Add(playerObject);
                if(playerObject == null){
                    Debug.Log("Player null");
                }
                CharacterController playerController = playerObject.GetComponent<CharacterController>();
                playerController.enabled = false;
                playerObject.transform.position = teamOne.Find("Spawn1").transform.position;
                playerController.enabled = true;
                //SetPositionAndRotation(teamOne.Find("Spawn1").transform.position, teamOne.Find("Spawn1").transform.rotation); 
                //Instantiate(Player, teamOne.Find("Spawn" + i));
                //Debug.Log("Moved player");
            } 
            else{
                Transform spawnAt = teamOne.Find("Spawn" + i).transform;
                SpawnBot(spawnAt, team1);
            }
        }
    }
    private void SpawnTeamTwo(){
        Transform teamTwo = StartSpawnPoints.transform.Find(team2);
        for(int i = 1; i < teamSize + 1; i++)
        {
            Transform spawnAt = teamTwo.Find("Spawn" + i).transform;
            SpawnBot(spawnAt, team2);
        }
    }


    public void HandleAIDeath(GameObject aiSoldier)
    {
        //Get tag for use after soldier object destroyed
        string deadSoldierTeam = aiSoldier.tag;
        //Debug.Log("dead soldier team =" + deadSoldierTeam);

        //Remove bot from current active bot list
        if(deadSoldierTeam == team1) activeTeam1Soldiers.Remove(aiSoldier);
        else if(deadSoldierTeam == team2) activeTeam2Soldiers.Remove(aiSoldier);
        
        //Destroy game object and make score and respawn updates
        Destroy(aiSoldier);
        UpdateScore(deadSoldierTeam);
        StartCoroutine(SpawnNewAI(deadSoldierTeam));
    }
    public void HandlePlayerDeath(GameObject player)
    {
        activeTeam1Soldiers.Remove(player);
        //Destroy(player);
        player.SetActive(false);
        UpdateScore(team1);
        StartCoroutine(SpawnPlayerBack(player));
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

    IEnumerator SpawnNewAI(string teamKilled)
    {
        //Delay a bit before spawning back in
        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("Spawning one AI");
        Transform spawnAt = ChooseSpawnPoint(teamKilled);
        //Debug.Log("Spawn is " + spawnAt.name);

        if(teamKilled == team1)
        {
            GameObject bot = Instantiate(AISoldierT1, spawnAt.position, spawnAt.rotation) as GameObject;
            activeTeam1Soldiers.Add(bot);
        }
        else if(teamKilled == team2)
        {
            GameObject bot = Instantiate(AISoldierT2, spawnAt.position, spawnAt.rotation) as GameObject;
            activeTeam2Soldiers.Add(bot);
        }
        

    }
    IEnumerator SpawnPlayerBack(GameObject player)
    {
        yield return new WaitForSeconds(respawnDelay);

        Transform spawnAt = ChooseSpawnPoint(team1);
        //GameObject player = Instantiate(Player, spawnAt.position, spawnAt.rotation) as GameObject;
        
        //Allow player to be moved
        CharacterController playerController = player.GetComponent<CharacterController>();
        playerController.enabled = false;
        player.transform.position = spawnAt.position;
        playerController.enabled = true;

        player.GetComponent<PlayerState>().SetDefaultState();
        player.SetActive(true);
        activeTeam1Soldiers.Add(player);
    }
    
    private Transform ChooseSpawnPoint(string teamKilled)
    {
        List<Transform> shuffledList;
        var index = -1;
        string enemyTag = teamKilled == team1 ? team2 : team1;
        if(teamKilled == team1)
        {
            //Debug.Log("Team = 1");
            shuffledList = ShuffledRespawnList();
            for(int i = 0; i < shuffledList.Count; i++)//Transform spawn in shuffledList)
            {
                Transform spawn = shuffledList[i];
                bool enemyWithinSight = false;
                for(int j = 0; j < activeTeam2Soldiers.Count && !enemyWithinSight; j++)
                {
                    RaycastHit newHit;
                    //Debug.DrawRay(spawn.GetChild(0).transform.position, activeTeam2Soldiers[j].transform.position + Vector3.up * torsoHeight);
                    bool hit = Physics.Raycast (new Ray(spawn.GetChild(0).transform.position, activeTeam2Soldiers[j].transform.position + Vector3.up * torsoHeight - spawn.GetChild(0).transform.position), out newHit);
                    if(hit && newHit.collider.gameObject.tag == team2)
                    {
                        //Debug.Log("Enemy can spot spawn");
                        enemyWithinSight = true;
                    }
                }
                if(!enemyWithinSight){
                    //Debug.Log("Spawn at " + spawn.name);
                    return spawn;
                }
                index = rand.Next(respawnPoints.Count -1);
            }
        }
        else if(teamKilled == team2)
        {
            shuffledList = ShuffledRespawnList();
            //Debug.Log("Team = 2");
            for(int i = 0; i < shuffledList.Count; i++)//Transform spawn in shuffledList)
            {
                //Debug.Log("looping");
                Transform spawn = shuffledList[i];
                bool enemyWithinSight = false;
                for(int j = 0; j < activeTeam1Soldiers.Count && !enemyWithinSight; j++)
                {
                    RaycastHit newHit;
                    bool hit = Physics.Raycast (new Ray(spawn.GetChild(0).transform.position, activeTeam1Soldiers[j].transform.position + Vector3.up * torsoHeight - spawn.GetChild(0).transform.position), out newHit);
                    if(hit && newHit.collider.gameObject.tag == enemyTag)
                    {
                        //Debug.Log("Enemy can spot spawn");
                        enemyWithinSight = true;
                    }
                }
                if(!enemyWithinSight)
                {
                    //Debug.Log("Spawn at " + spawn.name);
                    return spawn;
                }
                
                
            }   
            index = rand.Next(shuffledList.Count -1);
        }
        else{
            Debug.Log("Invalid team name");
            return null;
        }

        return shuffledList[index];
    }
    private List<Transform> ShuffledRespawnList()
    {
        List<Transform> newList = respawnPoints.ToList();
        /*for (int i = respawnPoints.Count - 1; i > 0; i--)
        {
            var k = rand.Next(i + 1);
            var value = respawnPoints[k];
            respawnPoints[k] = respawnPoints[i];
            respawnPoints[i] = value;
        }
        */
        for (int i = respawnPoints.Count - 1; i > 0; i--)
        {
            var k = rand.Next(i + 1);
            var value = respawnPoints[k];
            newList[k] = respawnPoints[i];
            newList[i] = value;
        }
        return newList;
    }
}
