using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public GameManager Game;
    public Camera MainCam;
    public WeaponManager WeaponMgr;
    public PlayerInputManager InputManager;
    
    [Header("Player Info")]
    private float maxHealth = 100f;
    public float currentHealth;
    public string myTag = "Team1";
    public string enemyTag = "Team2";
    private float criticalState;
    private float baseHealthRegenPerSecond = 5f;
    private float healthRegenPerSecond;
    private float healthRegenAcceleration = 15f;
    public int kills;
    public int deaths;

    [Header("Blood UI")]
    public Image overlay;
    public Sprite overlaySprite;
    private float criticalStateDuration = 3.25f;
    private float fadeSpeed;
    private float durationTimer;
    
    void Awake()
    {
        Game = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        MainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        WeaponMgr = GetComponentInChildren<WeaponManager>();
        InputManager = GetComponent<PlayerInputManager>();
        currentHealth = maxHealth;
        criticalState = 0.26f * maxHealth;
        overlay = GameObject.FindWithTag("Overlay").GetComponent<Image>();
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
        healthRegenPerSecond = baseHealthRegenPerSecond;
        gameObject.tag = myTag;
        GameObject damOverlay = GameObject.Find("DamageOverlay");
        overlaySprite = damOverlay.GetComponent<Sprite>();
        kills = deaths = 0;
    }

    void Update()
    {
        //If character has no health remaining
        if(currentHealth < 0){
            Debug.Log("You died");

            InputManager.DisableInputActions();
            MainCam.GetComponent<PlayerCamera>().PlayerActive = false;
            WeaponMgr.CancelInvoke("ReloadFinished");
            deaths += 1;
            //Destroy(gameObject);
            Game.HandlePlayerDeath(gameObject);
            //currentHealth = maxHealth;
        }

        //Health regen
        if(currentHealth > criticalState){
            healthRegenPerSecond += Time.deltaTime * healthRegenAcceleration;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1 - currentHealth / maxHealth);
            currentHealth = Mathf.Min(100, currentHealth + healthRegenPerSecond * Time.deltaTime);
        }
        
        else{
            durationTimer += Time.deltaTime;
            overlay.color = Color.Lerp(new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1f), new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.2f), Mathf.PingPong(Time.time * fadeSpeed, 1));
            if(durationTimer > criticalStateDuration){
                healthRegenPerSecond += Time.deltaTime * healthRegenAcceleration;
                currentHealth += Time.deltaTime * healthRegenPerSecond;
            }
            else{
                //Debug.Log("No health regen");
            }
        }
        
    }

    public void AddKill()
    {
        kills += 1;
    }

    public void TakeDamage(float damage)
    {
        //Take damage
        currentHealth -= damage;
        //Debug.Log("Damage taken: " + damage + ", health now: " + currentHealth);        //Reset health regen growth
        healthRegenPerSecond = baseHealthRegenPerSecond;

        //Handling UI for damage taken
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1 - currentHealth / maxHealth);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Bullet"){
            if(collision.gameObject.GetComponent<EnemyProjectile>() != null)
                TakeDamage(collision.gameObject.GetComponent<EnemyProjectile>().damage);
        }
    }

    public void SetDefaultState(bool resetKillDeath=false)
    {
        if(resetKillDeath){
             kills = 0;
            deaths = 0;
        }
        currentHealth = maxHealth;
        healthRegenPerSecond = baseHealthRegenPerSecond;
        gameObject.GetComponent<PlayerMotor>().StopMovementExceptFor("idle");
        WeaponMgr.SetDefaultState();
        MainCam.GetComponent<PlayerCamera>().PlayerActive = true;
        InputManager.EnableInputActions();
    }
}
