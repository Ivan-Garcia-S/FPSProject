using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWeaponManager : MonoBehaviour
{
    public GameObject shootPoint;
    public GameObject projectile;
    public GameObject gun;
    public Animator animator;
    public EnemyAI AI;

    //Graphics
    public GameObject muzzleFlash;
    //Bullet force
    public float shootForce, upwardForce;
    //Gun stats
    public float timeBetweenShots, spread, reloadTime, fireRate;
    public int magazineSize, bulletsPerTap;
    public bool isAutomatic;

    public int bulletsInMag, bulletsShot, bulletsInReserve;

    public bool shooting, reloading;
    public bool allowInvoke = true;
    public bool processedLastShootCall = true;
    public bool readyToShoot;

    
    // Start is called before the first frame update
    void Start()
    {
        AI = GetComponentInParent<EnemyAI>();
        animator = GetComponentInParent<Animator>();
        bulletsInMag = magazineSize;
        bulletsInReserve = 250;
        readyToShoot = true;
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Transform target)
    {

        if (readyToShoot && bulletsInMag > 0 && processedLastShootCall)
        {
            //Debug.Log("About to send shot");
            processedLastShootCall = false;
            animator.SetBool("shoot", true);
            Transform enemySpine = target.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2");

            //New look at
            Vector3 rotation = Quaternion.LookRotation(enemySpine.position - AI.transform.position).eulerAngles;
            rotation.x = rotation.z = 0f;
            AI.transform.rotation = Quaternion.Euler(rotation);
            Vector3 shootPoint = transform.Find("Shoot Point").transform.position;
            //Vector3 shootPoint = AI.transform.Find("pistol/Shoot Point").transform.position;
            //Calculate direction from attackPoint to the targetPoint
            Vector3 directionNoSpread = enemySpine.position - shootPoint;

             //Calculate spread
            float spreadX = Random.Range(spread, spread);
            float spreadY = Random.Range(spread, spread);

            //Direction with spread
            Vector3 directionWithSpread = directionNoSpread + new Vector3(spreadX,spreadY,0); 
            
            //animator.SetBool("shoot",true);
            ///Create bullet and add force to make it shoot forward
            GameObject bullet = GameObject.Instantiate(projectile, shootPoint, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().SendMessage("SetBulletInfo", AI.tag);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            rb.AddForce(directionWithSpread.normalized * 100f, ForceMode.Impulse);
            rb.AddForce(transform.up * upwardForce, ForceMode.Impulse);
            //rb.AddForce(AI.transform.Find("pistol").transform.up * AI.upwardForce, ForceMode.Impulse);
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
                    Invoke("ResetShot", fireRate);
                    allowInvoke = false;
                }
            }
            processedLastShootCall = true;
        }
        else if(processedLastShootCall){
            
            processedLastShootCall = false;
            animator.SetBool("shoot",false);
            animator.SetBool("idle", true);
            if(bulletsInMag == 0){
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
        animator.SetBool("shoot", false);

    }   
    public void Reload()
    {
        //Debug.Log("Reloading");
        animator.SetTrigger("reload");
        reloading = true;
        readyToShoot = false;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        animator.SetTrigger("stopReload");
        readyToShoot = true;
        bulletsInMag = magazineSize;
        bulletsInReserve -= bulletsInMag;
        reloading = false;
    }
}
