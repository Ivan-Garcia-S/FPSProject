using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    
    private float maxHealth = 100f;
    public float currentHealth;
    private float criticalState;
    private float baseHealthRegenPerSecond = 5f;
    private float healthRegenPerSecond;
    private float healthRegenAcceleration = 15f;

    public Image overlay;
    public Sprite overlaySprite;
    public float duration;
    public float criticalStateDuration = 3.25f;
    public float fadeSpeed;
    private float durationTimer;
    public string myTag = "Team1";
    public string enemyTag;

   
    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        criticalState = 0.2f * maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
        healthRegenPerSecond = baseHealthRegenPerSecond;
        gameObject.tag = myTag;
        GameObject damOverlay = GameObject.Find("DamageOverlay");
        //Debug.Log(damOverlay + " = damOverlay");
        overlaySprite = damOverlay.GetComponent<Sprite>();
        enemyTag = "Team2";
    }

    // Update is called once per frame
    void Update()
    {
        //If character has no health remaining
        if(currentHealth < 0){
            Debug.Log("You died");
            //currentHealth = maxHealth;
        }

        if(currentHealth > criticalState){
            healthRegenPerSecond += Time.deltaTime * healthRegenAcceleration;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1 - currentHealth / maxHealth);
            currentHealth = Mathf.Min(100, currentHealth + healthRegenPerSecond * Time.deltaTime);
        }
        
        //Old method
        /*else if(enteredCriticalState){
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
            enteredCriticalState = false;
            durationTimer += Time.deltaTime;
            overlay.color = Color.Lerp(overlay.color, new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.5f), Mathf.PingPong(Time.time * fadeSpeed, 1));
        }
        else{
            durationTimer += Time.deltaTime;
            overlay.color = Color.Lerp(overlay.color, new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.5f), Mathf.PingPong(Time.time * fadeSpeed, 1));
            if(durationTimer > criticalStateDuration){
                currentHealth += Time.deltaTime * healthRegenPerSecond;
            }
        }
        */
        else{
            durationTimer += Time.deltaTime;
            overlay.color = Color.Lerp(new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1f), new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.2f), Mathf.PingPong(Time.time * fadeSpeed, 1));
            if(durationTimer > criticalStateDuration){
                healthRegenPerSecond += Time.deltaTime * healthRegenAcceleration;
                currentHealth += Time.deltaTime * healthRegenPerSecond;
            }
            else{
                Debug.Log("No health regen");
            }
        }
        
    }

    public void TakeDamage(float damage)
    {
        //Take damage
        currentHealth -= damage;

        //Reset health regen growth
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
}
