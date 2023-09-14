using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    GameObject head;
    GameObject gun;
    Camera cam;
    PlayerMotor motor;
    Vector3 standOffset = new Vector3(0.1f, -1.7f, -0.1f);          //- Vector3(18.8500004,2.16999984,39.8600006)
    Vector3 crouchOffset = new Vector3(0.004f, 1.306f, 0.325f);    //// Cam position --- Vector3(0.0511999987,0.611333311,0.0680999979)
    Ray lookRay;
    RaycastHit lookRayHit;
    public Transform lookPoint;
    public Transform defaultCamPosition;
    public float forwardMultiplierHead = 0.26f;
    public float upMultiplierHead = 0.2f;
    public float horizontalMultiplierhead = 0f;
    public float forwardMultiplierGun = -0.14f;
    public float upMultiplierGun = 0.17f;
    public float horizontalMultiplierGun = -0.1f;
   //Vector3 lookPoint;
    void Start()
    {
        player = GameObject.Find("Soldier_M_AR");
        motor = player.GetComponent<PlayerMotor>();
        head = player.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head").gameObject;
        gun = GameObject.FindWithTag("PlayerWeapon");
        cam = gameObject.GetComponent<Camera>();
        cam.transform.position = defaultCamPosition.position;
    }

    // Update is called once per frwame
    //Changed from FixedUpdate to Update
    void FixedUpdate()
    {
        //Make camera look point in center of player view
        lookRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));  
        lookPoint.position = lookRay.GetPoint(10);
        
        Vector3 addZ = Vector3.zero;
        if(motor.IsSprinting()){
            addZ = head.transform.forward * .18f;
        }
        //Camera settings for when placed by head
        transform.position = head.transform.position + head.transform.forward * forwardMultiplierHead + head.transform.up * upMultiplierHead + addZ;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, player.transform.eulerAngles.y, transform.eulerAngles.z);
       

       //Camera settings for when placed behind gun
       //transform.position = gun.transform.position + gun.transform.forward * forwardMultiplierGun + gun.transform.up * upMultiplierGun + gun.transform.right * horizontalMultiplierGun;
       //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, player.transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
