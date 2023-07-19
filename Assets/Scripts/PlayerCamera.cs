using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    Camera cam;
    PlayerMotor motor;
    Vector3 standOffset = new Vector3(-0.275f, 1.834f, 0.1f);          //- Vector3(18.8500004,2.16999984,39.8600006)
    Vector3 crouchOffset = new Vector3(0.004f, 1.306f, 0.325f);    //// Cam position --- Vector3(0.0511999987,0.611333311,0.0680999979)
    Ray lookRay;
    RaycastHit lookRayHit;
    public Transform lookPoint;
    public Transform defaultCamPosition;
   //Vector3 lookPoint;
    void Start()
    {
        motor = GameObject.Find("Player").GetComponentInChildren<PlayerMotor>();
        cam = gameObject.GetComponent<Camera>();
        //cam.transform.position = defaultCamPosition.position;
        player = motor.gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        lookRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        /*if(Physics.Raycast(lookRay, out lookRayHit)){
           // Debug.Log("Hit at " + lookRayHit.point);
            lookPoint.position = lookRayHit.point;
        }
        */
         
        //else{
           //Debug.Log("Look point at " + lookRay.GetPoint(10));
        lookPoint.position = lookRay.GetPoint(10);
        //} 
       
        //transform.position = new Vector3(0.0399f,0.639f,0.103f);
        //if(!motor.isCrouched()) transform.position = player.transform.position + standOffset;
        
        //else transform.position = player.transform.position + crouchOffset;
    }
}
