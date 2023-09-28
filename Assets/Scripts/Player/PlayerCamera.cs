using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject Player;
    GameObject Head;
    Camera Cam;
    PlayerMotor Motor;
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

    public bool PlayerActive { get; set;}
   //Vector3 lookPoint;
    void Start()
    {
        Player = GameObject.Find("Soldier_M_AR");
        PlayerActive = true;
        Motor = Player.GetComponent<PlayerMotor>();
        Head = Player.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head").gameObject;
        Cam = gameObject.GetComponent<Camera>();
        Cam.transform.position = defaultCamPosition.position;
    }

    // Update is called once per frwame
    //Changed from FixedUpdate to Update
    void FixedUpdate()
    {
        //Make camera look point in center of player view
        lookRay = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));  
        lookPoint.position = lookRay.GetPoint(10);
        
        Vector3 addZ = Vector3.zero;
        if(Motor.IsSprinting()){
            addZ = Head.transform.forward * .18f;
        }
        //Camera settings for when placed by head
        if(PlayerActive){
            transform.position = Head.transform.position + Head.transform.forward * forwardMultiplierHead + Head.transform.up * upMultiplierHead + addZ;
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Player.transform.eulerAngles.y, transform.eulerAngles.z);
        }
        
       

       //Camera settings for when placed behind gun
       //transform.position = gun.transform.position + gun.transform.forward * forwardMultiplierGun + gun.transform.up * upMultiplierGun + gun.transform.right * horizontalMultiplierGun;
       //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, player.transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public void FindPlayer()
    {
        Player = GameObject.Find("Soldier_M_AR");
        Head = Player.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head").gameObject;
        Motor = Player.GetComponent<PlayerMotor>();
        PlayerActive = true;
    }

    
}
