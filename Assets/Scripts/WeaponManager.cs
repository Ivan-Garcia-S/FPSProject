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
    public Hitmarker Hitmark;
    public PlayerMotor Motor;
    public GameObject Crosshair;
    public Projectile Projectile;
    public Animator Animator;
    public GameObject Sphere;
    public GameObject Bullet;
    public GameObject Gun;
    public Camera FpsCam;
    public Transform AttackPoint;
    public Transform CamGunPoint;
    //Graphics
    public GameObject MuzzleFlash;
    public TextMeshProUGUI AmmoDisplay;
    public AudioClip ReloadSound;
    public AudioClip BulletSound;
    public AudioSource GunAudioSource;

    [Header("Gun Stats")]
    public float shootForce, upwardForce;
    public float timeBetweenShots, spread, reloadTime, fireRate;
    public int magazineSize, bulletsPerTap, reserveAmmo, maxReserveAmmo;
    public float maxRange = 100f;
    public bool isAutomatic;


    public int bulletsLeft, bulletsShot;

    public bool shooting, readyToShoot, reloading;
    public bool allowInvoke = true;
    private float timeOfFirstShot = -1;


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
    private Transform leftHandTarget;
    private Transform leftHandTargetProne;
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
        Hitmark = GameObject.Find("Hitmarker").GetComponent<Hitmarker>();
        bulletsLeft = magazineSize;
        Crosshair = GameObject.FindWithTag("Crosshair");
        readyToShoot = true;
        isAutomatic = true;
        isAimingIn = false;
        Motor = GetComponentInParent<PlayerMotor>();
        Animator = GetComponentInParent<Animator>();
        Sphere = GameObject.Find("Sphere");
        ads = GameObject.Find("RightHandIK_AR").GetComponent<TwoBoneIKConstraint>();
        leftHand = GameObject.Find("LeftHandIK").GetComponent<TwoBoneIKConstraint>();
        leftHandTarget = transform.Find("LeftHandIKTarget");
        leftHandTargetProne = transform.Find("LeftHandIKTargetProne");
        GunAudioSource = GetComponent<AudioSource>();
        maxReserveAmmo = 250;
        reserveAmmo = maxReserveAmmo;
        //originalGunSpot = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        shooting = Animator.GetBool("tryToShoot");
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
        if(Animator.GetBool("aimingDown")) Crosshair.SetActive(false);
        else Crosshair.SetActive(true);
        
        //AimDownSights();

        //Set ammo display
        if(AmmoDisplay != null) AmmoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + reserveAmmo);
    }

    
    public void StartShoot()
    {
        //animator.SetBool("isShooting", true);
        Animator.SetBool("tryToShoot", true);
        timeOfFirstShot = Time.time;
    }
   
    public void EndShoot()
    {
        //animator.SetBool("isShooting", false);
        //animator.SetBool("isShooting", false);

        //Set animator and sfx correctly
        Animator.SetBool("tryToShoot", false);
        if(GunAudioSource.isPlaying && GunAudioSource.clip == BulletSound) 
        {
            float diffIntime = Time.time - timeOfFirstShot;
            if(diffIntime >= .128f){
                        GunAudioSource.Stop();
            }
            else{
                Invoke("StopGunShotSound", .128f - diffIntime);
            }
        }
    
        
    }
    //Manually stop sound so at least one bullet is fired in sound player
    private void StopGunShotSound()
    {
        if(GunAudioSource.clip == BulletSound){
            GunAudioSource.Stop();
        }
    }

    private void Shoot()
    {

        //TAKE OUT ///////
        //animator.SetBool("shoot", true);
        Motor.StopSprint();
        //Debug.Log("shooting: " + animator.GetBool("shooting"));

        //Allow shot per frame
        readyToShoot = false;

        //Use Raycast to find bullet hit point
        Ray ray = FpsCam.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));
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
        GameObject currentBullet = Instantiate(Bullet, AttackPoint.transform.position, AttackPoint.transform.rotation);
        currentBullet.GetComponent<Projectile>().SetBulletInfo(enemyTag, Hitmark);
        
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;
        currentBullet.transform.Rotate(90,0,0);

        //Change sound to shoot sound
        if(GunAudioSource.clip != BulletSound){
            GunAudioSource.Stop();
            GunAudioSource.clip = BulletSound;
        }
        if(!GunAudioSource.isPlaying){
            GunAudioSource.Play();
        }

        //Add force to fire bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        //Upward force for grenades that bounce
        currentBullet.GetComponent<Rigidbody>().AddForce(FpsCam.transform.up * upwardForce, ForceMode.Impulse);

        
        //Instantiate muzzle flash
        if(MuzzleFlash != null) Instantiate(MuzzleFlash, AttackPoint.position, Quaternion.identity);
        
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
    }

    public void Reload()
    {
        //Only reload if magazine not full
        if(bulletsLeft < magazineSize && !Motor.IsSprinting() && !reloading)
        {
            Animator.SetBool("reload",true);
            reloading = true;
            GunAudioSource.Stop();
            GunAudioSource.clip = ReloadSound;
            GunAudioSource.Play();
            StartCoroutine(ReleaseLeftHand());
            Invoke("ReloadFinished", reloadTime);
        }
        
    }

    private void ReloadFinished()
    {
        GunAudioSource.Stop();
        Animator.SetBool("reload",false);
        reserveAmmo -= (magazineSize - bulletsLeft);
        bulletsLeft = magazineSize;
        StartCoroutine(PlaceBackLeftHand());
        reloading = false;
    }

    public void CancelReload()
    {
        CancelInvoke("ReloadFinished");
        GunAudioSource.Stop();
        Animator.SetBool("reload", false);
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
            Animator.SetBool("aimingDown",true);
            
            Motor.StopSprint();
            startADS = StartCoroutine(ADS());
            Motor.ADSActive(true);
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
        isAimingIn = false;
        if(startADS != null) StopCoroutine(startADS);
        
        Animator.SetBool("aimingDown",false);
        stopADS = StartCoroutine(ExitADS());
        Motor.ADSActive(false);
    }

    IEnumerator ExitADS()
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
            targetPosition = FpsCam.transform.position;
        }
        weaponSwayPosition = weaponSwayObj.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition,targetPosition, ref weaponSwayPositionVelocity, aimInTime);
        weaponSwayObj.transform.position = weaponSwayPosition;

    }
    //Reset player attributes to their default state
    public void SetDefaultState()
    {
        bulletsLeft = magazineSize;
        reserveAmmo = maxReserveAmmo;
        Animator.SetBool("aimingDown",false);
        ads.weight = 0;
        isAimingIn = false;
        readyToShoot = true;
        shooting = false;
        reloading = false;
        allowInvoke = true;
    }
    public void SetProneSettings(bool isProne)
    {
        if(isProne)
        {
            leftHandTarget.localPosition = new Vector3(-0.92f,-0.95f,-2.97f);
            leftHandTarget.localEulerAngles = new Vector3(358.42f,272.68f,293.74f);
        }
        else 
        {
            leftHandTarget.localPosition = new Vector3(-0.13f,-0.027f,-0.57f);
            leftHandTarget.localEulerAngles = new Vector3(342.30f,86.89f,92.97f);
        }
    }

}
