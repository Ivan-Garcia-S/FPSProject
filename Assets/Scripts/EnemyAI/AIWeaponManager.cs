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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Transform target)
    {

        if (readyToShoot && !reloading && bulletsInMag > 0 && processedLastShootCall)
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

                //Calculate spread
                float spreadX = Random.Range(-spread, spread);
                float spreadY = Random.Range(-spread, spread);

                //Direction with spread
                Vector3 directionWithSpread = directionNoSpread + new Vector3(spreadX,spreadY,0); 
                
                ///Create bullet and add force to make it shoot forward
                GameObject bullet = GameObject.Instantiate(projectile, shootPoint, Quaternion.identity);
                bullet.GetComponent<EnemyProjectile>().SendMessage("SetBulletInfo", AI.tag);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();

                rb.AddForce(directionWithSpread.normalized * 100f, ForceMode.Impulse);
                rb.AddForce(transform.up * upwardForce, ForceMode.Impulse);
                bulletsInMag--;
                bulletsShot++;
                
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
                        Debug.Log("Fire rate = " + fireRate);
                        Invoke("ResetShot", fireRate);
                        allowInvoke = false;
                    }
                }
                processedLastShootCall = true;
                
                if(nerfRoutineRunning){
                    StopCoroutine(TurnOnAINerf());
                }
                callAINerf = StartCoroutine(TurnOnAINerf());
            }
            
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
        if(firstBullet == null || !firstBulletRoutineRunning){
            firstBullet = StartCoroutine(FirstBulletCountdown(waitTime));
        }
    }

    
    IEnumerator FirstBulletCountdown(float seconds)
    {
        firstBulletRoutineRunning = true;
        yield return new WaitForSeconds(seconds);
        aiNerfOn = false;
        firstBulletRoutineRunning = false;
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
