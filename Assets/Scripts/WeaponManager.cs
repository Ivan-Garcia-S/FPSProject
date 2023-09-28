using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Animations.Rigging;

public class WeaponManager: MonoBehaviour
{
    [Header("References")]
    //References
    public Hitmarker hitmark;
    public PlayerMotor motor;
    public GameObject crosshair;
    public Projectile projectile;
    public Animator animator;
    public GameObject sphere;
    public GameObject bullet;
    public GameObject gun;
    public Camera fpsCam;
    public Transform attackPoint;
    public Transform camGunPoint;
    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;

    [Header("Gun Stats")]
    public float shootForce, upwardForce;
    public float timeBetweenShots, spread, reloadTime, fireRate;
    public int magazineSize, bulletsPerTap;
    public float maxRange = 100f;
    public bool isAutomatic;

    public int bulletsLeft, bulletsShot;

    public bool shooting, readyToShoot, reloading;
    public bool allowInvoke = true;


    private Vector3 shootPointOffset = new Vector3(0.0022f,0.0237f,0.1061f);
    // = new Vector3(-0.050999999f,0.151999995f,0.0189999994f);

    private Coroutine startADS;
    private Coroutine stopADS;
    private string enemyTag = "Team2";

    [Header("Aiming In")]
    public bool isAimingIn;
    public Transform sightTarget;
    public Transform originalGunSpot;
    private TwoBoneIKConstraint ads;
    private TwoBoneIKConstraint leftHand;
    public float sightOffset;
    public float aimInTime;
    public bool adsComplete = false;
    private float timeElapsed;
    private Vector3 initialADSPosition;
    private Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;
    [SerializeField] private Transform weaponSwayObj;
    
    // Start is called before the first frame update
    void Awake()
    {
        //animator = gameObject.GetComponent<Animator>();
        //bullet = GetComponent<Projectile2>().bullet;
        
        //Set mag to full
        hitmark = GameObject.Find("Hitmarker").GetComponent<Hitmarker>();
        bulletsLeft = magazineSize;
        crosshair = GameObject.FindWithTag("Crosshair");
        readyToShoot = true;
        isAutomatic = true;
        isAimingIn = false;
        motor = GetComponentInParent<PlayerMotor>();
        animator = GetComponentInParent<Animator>();
        sphere = GameObject.Find("Sphere");
        ads = GameObject.Find("RightHandIK_AR").GetComponent<TwoBoneIKConstraint>();
        leftHand = GameObject.Find("LeftHandIK").GetComponent<TwoBoneIKConstraint>();
        //originalGunSpot = transform.localPosition;
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

        /*if(isAimingIn)
        {
            if (timeElapsed < aimInTime)
            {
                transform.position = Vector3.Lerp(transform.position, camGunPoint.transform.position, timeElapsed / aimInTime);
                timeElapsed += Time.deltaTime;
            }
            else
            {
                transform.position = camGunPoint.transform.position;
            }

            //transform.position = camGunPoint.transform.position;//fpsCam.transform.position + new Vector3(0,0,0.5f);
            transform.rotation = fpsCam.transform.rotation;
        }
        */

        //Remove crosshair if aiming down sights
        if(animator.GetBool("aimingDown")) crosshair.SetActive(false);
        else crosshair.SetActive(true);
        
        //AimDownSights();

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

        //TAKE OUT ///////
        //animator.SetBool("shoot", true);
        motor.StopSprint();
        //Debug.Log("shooting: " + animator.GetBool("shooting"));

        //Allow shot per frame
        readyToShoot = false;

        //Use Raycast to find bullet hit point
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit)) targetPoint = hit.point;
        else targetPoint = ray.GetPoint(75);

        //Calculate direction from attackPoint to the targetPoint
        Vector3 directionNoSpread = targetPoint - (transform.position + shootPointOffset);

        //Calculate spread
        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);

        //Direction with spread
        Vector3 directionWithSpread = directionNoSpread + new Vector3(spreadX,spreadY,0); 

        //Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.transform.position, attackPoint.transform.rotation);
        currentBullet.GetComponent<Projectile>().SetBulletInfo(enemyTag, hitmark);
        //Debug.Log("New bullet shot");

        //GameObject currentBullet = Instantiate(bullet, attackPoint.transform, false);

        
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
        
        //TAKE OUT// 8/28/23
        //animator.SetBool("shoot", false);

    }

    public void Reload()
    {
        //Only reload if magazine not full
        if(bulletsLeft < magazineSize)
        {
            animator.SetBool("reload",true);
            reloading = true;
            StartCoroutine(ReleaseLeftHand());
            Invoke("ReloadFinished", reloadTime);
        }
        
    }

    private void ReloadFinished()
    {
        animator.SetBool("reload",false);
        bulletsLeft = magazineSize;
        StartCoroutine(PlaceBackLeftHand());
        reloading = false;
    }

    public void CancelReload()
    {
        CancelInvoke("ReloadFinished");
        animator.SetBool("reload", false);
        StartCoroutine(PlaceBackLeftHand());
        reloading = false;
    }

    IEnumerator ReleaseLeftHand()
    {
        float timeElapsedHand = 0;
        while (timeElapsedHand < 1.1)
        {
            leftHand.weight = Mathf.Lerp(leftHand.weight, 0, timeElapsedHand / 1.1f);
            timeElapsedHand += Time.deltaTime;
            yield return null;
        }
        leftHand.weight = 0;
    }
    IEnumerator PlaceBackLeftHand()
    {
        float timeElapsedHand = 0;
        while (timeElapsedHand < 1.1)
        {
            leftHand.weight = Mathf.Lerp(leftHand.weight, 1, timeElapsedHand / 1.1f);
            timeElapsedHand += Time.deltaTime;
            yield return null;
        }
        leftHand.weight = 1;
    }
    private void AimDownSights()
    {
        if(isAimingIn)  transform.localPosition = Vector3.Lerp(initialADSPosition, sightTarget.position, timeElapsed / aimInTime);//Time.deltaTime * adsSpeed);
        
        else transform.localPosition = Vector3.Lerp(initialADSPosition, originalGunSpot.position, timeElapsed / aimInTime);//Time.deltaTime * adsSpeed);
    }
    public void AimingInPressed()
    {
        //Debug.Log("In ADS PRESS");
        if(!reloading)
        {
            isAimingIn = true;
            if(stopADS != null) StopCoroutine(stopADS);
            animator.SetBool("aimingDown",true);
            
            motor.StopSprint();
            startADS = StartCoroutine(ADS());
        }
    }

    IEnumerator ADS()
    {
        timeElapsed = 0;
        while (timeElapsed < aimInTime)
        {
            ads.weight = Mathf.Lerp(ads.weight, 1, timeElapsed / aimInTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        ads.weight = 1;
    }

    public void AimingInReleased()
    {
        //Debug.Log("In ADS released");
        isAimingIn = false;
        
        if(startADS != null) StopCoroutine(startADS);
        
        animator.SetBool("aimingDown",false);
        stopADS = StartCoroutine(exitADS());
       
    }

    IEnumerator exitADS()
    {
        timeElapsed = 0;
        while (timeElapsed < aimInTime)
        {
            ads.weight = Mathf.Lerp(ads.weight, 0, timeElapsed / aimInTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        ads.weight = 0;
    }
        

    private void CalculateAimIn()
    {
        Vector3 targetPosition = transform.position;
        
        if(isAimingIn){
            //Debug.Log("CALCUlATE NEW AIM IN");
            targetPosition = fpsCam.transform.position;
        }
        weaponSwayPosition = weaponSwayObj.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition,targetPosition, ref weaponSwayPositionVelocity, aimInTime);
        weaponSwayObj.transform.position = weaponSwayPosition;

    }
    public void SetDefaultState()
    {
        bulletsLeft = magazineSize;
        animator.SetBool("aimingDown",false);
        ads.weight = 0;
        isAimingIn = false;
        readyToShoot = true;
        shooting = false;
        reloading = false;
        allowInvoke = true;
    }

}
