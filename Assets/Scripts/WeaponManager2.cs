using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponManager2: MonoBehaviour
{
    public GameObject playerCam;
    public Hitmarker hitmark;
    public Projectile2 projectile;
    public float maxRange = 100f;
    private Animator animator;
    //public float damage = 20f;
    //References
    public GameObject bullet;
    public Camera fpsCam;
    public Transform attackPoint;
    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;
    //Bullet force
    public float shootForce, upwardForce;
    //Gun stats
    public float timeBetweenShots, spread, reloadTime, fireRate;
    public int magazineSize, bulletsPerTap;
    public bool isAutomatic;

    int bulletsLeft, bulletsShot;

    bool shooting, readyToShoot, reloading;
    public bool allowInvoke = true;
    
    // Start is called before the first frame update
    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        //bullet = GetComponent<Projectile2>().bullet;
        
        //Set mag to full
        bulletsLeft = magazineSize;
        readyToShoot = true;

    }

    // Update is called once per frame
    void Update()
    {
        shooting = GetComponent<Animator>().GetBool("isShooting");

        //Auto reload when trying to shoot with no ammo
        if(readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Handle shooting
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0){
            bulletsShot = 0;
            Shoot();
        }

        //Set ammo display
        if(ammoDisplay != null) ammoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    /*public void onShoot(InputAction.CallbackContext context){
        //anim.Play();
        
        RaycastHit hit; 
        animator.SetBool("isShooting", true);
        if(context.performed && Physics.Raycast(playerCam.transform.position, transform.forward, out hit, maxRange)){
            Debug.Log("Hit");
            BotManager botManager = hit.transform.GetComponent<BotManager>();
            if(botManager != null){
                botManager.TakeDamage(damage);
                hitmark.botHit2();
            }
        }
        

    }
    */
    public void StartShoot()
    {
        animator.SetBool("isShooting", true);
    }
   
    public void EndShoot()
    {
        animator.SetBool("isShooting", false);
    }

    private void Shoot()
    {
        //Allow shot per frame
        readyToShoot = false;

        //Use Raycast to find bullet hit point
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit)) targetPoint = hit.point;
        else targetPoint = ray.GetPoint(75);

        //Calculate direction from attackPoint to the targetPoint
        Vector3 directionNoSpread = targetPoint - attackPoint.position;

        //Calculate spread
        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);

        //Direction with spread
        Vector3 directionWithSpread = directionNoSpread + new Vector3(spreadX,spreadY,0); 

        //Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity, gameObject.transform);
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add force to fire bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        //Upward force for grenades that bounce
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash
        if(muzzleFlash != null) Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        
        //Adjust bullet count
        bulletsLeft--;
        bulletsShot++;

        if(allowInvoke)
        {
            Invoke("ResetShot", fireRate);
            allowInvoke = false;
            if(!isAutomatic) GetComponent<WeaponManager>().EndShoot();
        }

        //If more than one bullet per tap call Shoot() again
        if(bulletsShot < bulletsPerTap && bulletsLeft > 0){
            Invoke("Shoot", timeBetweenShots);
        }
        
    }

    private void ResetShot()
    {
        //Allow shooting and Invoking again
        readyToShoot = true;
        allowInvoke = true;

    }

    public void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }


}
