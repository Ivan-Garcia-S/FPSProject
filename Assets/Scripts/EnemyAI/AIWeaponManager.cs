using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWeaponManager : MonoBehaviour
{
    [Header("References")]
    public GameObject shootPoint;
    public GameObject projectile;
    public GameObject gun;
    public Animator animator;
    public EnemyAI AI;

    //Graphics
    public GameObject muzzleFlash;

    [Header("Gun Stats")]
    //Bullet force
    public float shootForce, upwardForce;
    //Gun stats
    public float timeBetweenShots, spread, reloadTime, fireRate;
    public int magazineSize, bulletsPerTap;
    public int bulletsInMag, bulletsShot, bulletsInReserve;
    public bool isAutomatic;
    public bool shooting, reloading;
    public bool allowInvoke;
    public bool processedLastShootCall = true;
    public bool readyToShoot;
    public bool aiNerfOn;
    private Coroutine firstBullet;
    private Coroutine callAINerf;
    private bool firstBulletRoutineRunning;
    private bool nerfRoutineRunning;
    public bool adsAnimComplete;

    [Header("Audio")]
    
    public AudioClip bulletSound;
    public AudioClip reloadSound;
    public AudioSource audioSource;

    
    // Start is called before the first frame update
    void Start()
    {
        AI = GetComponentInParent<EnemyAI>();
        animator = GetComponentInParent<Animator>();
        bulletsInMag = magazineSize;
        bulletsInReserve = 250;
        readyToShoot = true;
       aiNerfOn = true;
       allowInvoke = true;
       firstBulletRoutineRunning = false;
       nerfRoutineRunning = false;
        adsAnimComplete = false;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Transform target)
    {

        if (readyToShoot && !reloading && bulletsInMag > 0 && processedLastShootCall && target != null)
        {
            //Wait until bot ads animation complete before firing 
            if(!animator.GetBool("shoot") || !adsAnimComplete){
                animator.SetBool("shoot",true);
                StartCoroutine(WaitOnADS());
            }
            else{
                //Don't shoot again until this function runs through
                processedLastShootCall = false;
                //animator.SetBool("shoot", true);
                Transform enemySpine = target.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2");

                //Rotate AI to correct rotation
                Vector3 rotation = Quaternion.LookRotation(enemySpine.position - AI.transform.position).eulerAngles;
                rotation.x = rotation.z = 0f;
                AI.transform.rotation = Quaternion.Euler(rotation);
                Vector3 shootPoint = transform.Find("Shoot Point").transform.position;
                
                //Calculate direction from attackPoint to the targetPoint
                Vector3 directionNoSpread = enemySpine.position - shootPoint;

                float distanceToEnemy = Vector3.Distance(enemySpine.position, shootPoint);

                float currentSpread = spread;
                //Make spread smaller if enemy is far
                if(distanceToEnemy > 7.5f){
                    currentSpread = 0.7f;
                }

                //Calculate spread
                float spreadX = Random.Range(-currentSpread, currentSpread);
                float spreadY = Random.Range(-currentSpread, currentSpread);

                //Direction with spread
                Vector3 directionWithSpread = directionNoSpread + new Vector3(spreadX,spreadY,0); 
                
                string[] infoArray = new string[2];
                infoArray[0] = AI.tag;
                infoArray[1] = AI.name;

                ///Create bullet and add force to make it shoot forward
                GameObject bullet = GameObject.Instantiate(projectile, shootPoint, Quaternion.identity);
                
                //New Create bullet /////////NOT USING
                
                /*GameObject bullet = BulletPool.Pool.GetPooledBullet();
                if(bullet != null)
                {
                    bullet.transform.position = shootPoint;
                    bullet.transform.rotation = Quaternion.identity;
                    bullet.SetActive(true);
                }
                else{
                    bullet = BulletPool.Pool.AddToPool(projectile);
                }
                */

                bullet.GetComponent<EnemyProjectile>().SendMessage("SetBulletInfo", infoArray);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();

                //Change sound to shoot sound
                if(audioSource.clip != bulletSound){
                    audioSource.Stop();
                    audioSource.clip = bulletSound;
                }
                if(!audioSource.isPlaying){
                    audioSource.Play();
                }

                rb.AddForce(directionWithSpread.normalized * 100f, ForceMode.Impulse);
                rb.AddForce(transform.up * upwardForce, ForceMode.Impulse);
                bulletsInMag--;
                bulletsShot++;
                Debug.Log("AI Bullet shot");

                //If more than one bullet per tap call Shoot() again
                if(bulletsShot < bulletsPerTap && bulletsInMag > 0){
                    Invoke("Shoot", timeBetweenShots);
                }
                else
                {
                    //Set delay for the next time AI can shoot
                    readyToShoot = false;
                    if(allowInvoke)
                    {
                        //Debug.Log("Fire rate = " + fireRate);
                        Invoke("ResetShot", fireRate);
                        allowInvoke = false;
                    }
                }
                processedLastShootCall = true;
                
                /*if(nerfRoutineRunning){
                    StopCoroutine(TurnOnAINerf());
                }
                callAINerf = StartCoroutine(TurnOnAINerf());
                */
            }
            AI.lastEnemyShotAt = target;
            
        }
        else if(processedLastShootCall){
            // Make sure this is the only instance running the Shoot function 
            processedLastShootCall = false;
            
            //TAKE OUT FOR NEW ANIM
            //animator.SetBool("shoot",false);
            
            //Reload if can't shoot because no ammo
            if(bulletsInMag == 0 && !reloading){
                Reload();
            }
            processedLastShootCall = true;
        }
    }

    private void ResetShot()
    {
        //Allow shooting and Invoking again
        readyToShoot = true;
        allowInvoke = true;
        
        //TAKE OUT FOR NEW ANIM
        //animator.SetBool("shoot", false);
    }   
    public void Reload()
    {
        //Debug.Log("Reloading");
        animator.SetBool("shoot",false);
        animator.SetBool("reload",true);
        reloading = true;
        readyToShoot = false;
        audioSource.clip = reloadSound;
        audioSource.Play();
        Invoke("StopReloadAnimation", reloadTime - 0.5f);
        Invoke("ReloadFinished", reloadTime);
        
    }

    private void ReloadFinished()
    {
        ///////OLD SPOT FOR BELOW////
        readyToShoot = true;
        bulletsInMag = magazineSize;
        bulletsInReserve -= bulletsInMag;
        reloading = false;
    }

    private void StopReloadAnimation()
    {
        animator.SetBool("reload", false);
    }

    public void StartCountdown(float waitTime)
    {
        /*if(firstBullet == null || !firstBulletRoutineRunning){
            firstBullet = StartCoroutine(FirstBulletCountdown(waitTime));
        }
        */
        StartCoroutine(SetLastTargetToCurrent(waitTime));
    }

    
    IEnumerator FirstBulletCountdown(float seconds)
    {
        firstBulletRoutineRunning = true;
        yield return new WaitForSeconds(seconds);
        aiNerfOn = false;
        firstBulletRoutineRunning = false;
    }

    public IEnumerator SetLastTargetToCurrent(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //Debug.Log("calling shoot");
        Shoot(AI.currentEnemyTarget);
    }

    IEnumerator TurnOnAINerf()
    {
        nerfRoutineRunning = true;
        yield return new WaitForSeconds(0.5f);
        aiNerfOn = true;
        nerfRoutineRunning = false;
    }
    public IEnumerator WaitOnADS()
    {
        yield return new WaitForSeconds(0.6f);
        adsAnimComplete = true;
    }
}
