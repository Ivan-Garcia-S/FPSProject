using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponManager2: MonoBehaviour
{
    public GameObject playerCam;
    public Hitmarker hitmark;
    public PlayerMotor motor;
    public GameObject crosshair;
    public Projectile2 projectile;
    public float maxRange = 100f;
    public Animator animator;
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

    [Header("Aiming in")]
    public bool isAimingIn;
    public Transform sightTarget;
    public float sightOffset;
    public float aimInTime;
    private Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;
    [SerializeField] private Transform weaponSwayObj;
    
    // Start is called before the first frame update
    void Awake()
    {
        //animator = gameObject.GetComponent<Animator>();
        //bullet = GetComponent<Projectile2>().bullet;
        
        //Set mag to full
        bulletsLeft = magazineSize;
        readyToShoot = true;
        isAutomatic = false;
        isAimingIn = false;
        motor = GetComponentInParent<PlayerMotor>();

    }

    // Update is called once per frame
    void Update()
    {
        shooting = animator.GetBool("tryToShoot");
        ///shooting = animator.GetBool("isShooting");

        //Auto reload when trying to shoot with no ammo
        if(readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Handle shooting
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0){
            bulletsShot = 0;
            Shoot();
        }
        //CalculateAimIn();

        //Remove crosshair if aiming down sights
        if(animator.GetBool("aimingDown")) crosshair.SetActive(false);
        else crosshair.SetActive(true);
        

        

        //Set ammo display
        if(ammoDisplay != null) ammoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    
    public void StartShoot()
    {
        //animator.SetBool("isShooting", true);
        animator.SetBool("tryToShoot", true);
    }
   
    public void EndShoot()
    {
        //animator.SetBool("isShooting", false);
        //animator.SetBool("isShooting", false);
        animator.SetBool("tryToShoot", false);
    }

    private void Shoot()
    {
        animator.SetBool("shooting", true);
        motor.StopSprint();
        Debug.Log("shooting: " + animator.GetBool("shooting"));

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
            if(!isAutomatic) EndShoot();
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
        animator.SetBool("shooting", false);

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

    public void AimingInPressed()
    {
        animator.SetBool("aimingDown",true);
        isAimingIn = true;
        motor.StopSprint();
    }

    public void AimingInReleased()
    {
        animator.SetBool("aimingDown",false);
        isAimingIn = false;
    }

    private void CalculateAimIn()
    {
        Vector3 targetPosition = transform.position;
        
        if(isAimingIn){
            Debug.Log("CALCUlATE NEW AIM IN");
            targetPosition = playerCam.transform.position;
        }
        weaponSwayPosition = weaponSwayObj.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition,targetPosition, ref weaponSwayPositionVelocity, aimInTime);
        weaponSwayObj.transform.position = weaponSwayPosition;

    }


}
