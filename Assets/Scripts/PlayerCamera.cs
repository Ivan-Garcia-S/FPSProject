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
    void Start()
    {
        motor = GameObject.Find("Player").GetComponentInChildren<PlayerMotor>();
       // cam = 
        player = motor.gameObject;
        //transform.position = new Vector3(0.0399f,0.639f,0.103f); //new Vector3(0.0512f,0.611f,0.0681f);//player.transform.position + standOffset;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(0.0399f,0.639f,0.103f);
        //if(!motor.isCrouched()) transform.position = player.transform.position + standOffset;
        
        //else transform.position = player.transform.position + crouchOffset;
    }
}
