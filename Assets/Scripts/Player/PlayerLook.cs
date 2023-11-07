using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    
    [Header("Sensitivity")]
    private float xRotation = 0f;
    private float yRotation = 0f;
    [SerializeField]
    private float minClamp = -80f;
    [SerializeField]
    private float maxClamp = 80f;
    public float xSensitivity = 15f;
    public float ySensitivity = 15f;
    private float controllerSenseX = 500f;
    private float controllerSenseY = 250f;
    private float controllerSenseXADS = 80f;
    private float controllerSenseYADS = 80f;

    [Header("Input")]
    private bool controllerUsed;
    
    
    void Awake()
    {
        //JUST DELETED//
        //skinnedMesh = transform.GetComponentInChildren<SkinnedMeshRenderer>();
        //Transform[] bones = skinnedMesh.bones;
        
        
        //left = Array.FindIndex(bones, b => b.name == "Upper")
        
        
        ////New Model/////DELETED
        /*leftArm = GameObject.Find("mixamorig:LeftArm");
        rightArm = GameObject.Find("mixamorig:RightArm");
        camSphere = GameObject.Find("CamSphere");
        head = GameObject.Find("mixamorig:Head");
        player = gameObject.GetComponentInParent<Animator>().gameObject;
        */
    }

    void Start() 
    {
        cam = GetComponent<PlayerState>().MainCam;
    }
    public void ProcessLook(Vector2 inputVector, InputControl typeInput, InputActionPhase ads)
    {
        controllerUsed = typeInput != null && !typeInput.device.description.deviceClass.Equals("Mouse");
        

        float mouseX = inputVector.x;
        float mouseY = inputVector.y;

        float extraXMult = 1;
        if(Mathf.Abs(mouseX) < 0.12){
            mouseX = 0;
        }
        else if(mouseX < 0.27f){
            extraXMult = 0.2f;
        }
        //Calculate camera rotation for looking up and down
        if(controllerUsed){  //For controller
            Debug.Log("Controller input = " + inputVector);
            if(ads.Equals(InputActionPhase.Performed))
            {
                xRotation -= (mouseY * Time.deltaTime) * controllerSenseYADS;
                yRotation += (mouseX * Time.deltaTime) * controllerSenseXADS * extraXMult;
            }
            else{
                xRotation -= (mouseY * Time.deltaTime) * controllerSenseY;
                yRotation += (mouseX * Time.deltaTime) * controllerSenseX * extraXMult;
            }
        }
        else{  //For all else
            xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
            yRotation += (mouseX * Time.deltaTime) * xSensitivity;
        }

        xRotation = Mathf.Clamp(xRotation, minClamp, maxClamp);

        //Apply to camera transform
        cam.transform.localRotation = Quaternion.Euler(xRotation, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);

        //Rotate player left and right
        if(controllerUsed)  transform.Rotate(Vector3.up * mouseX * Time.deltaTime * controllerSenseX);
        else transform.Rotate(Vector3.up * mouseX * Time.deltaTime * xSensitivity);
    }

    public void SetProneSettings(bool isProne)
    {
        if(isProne)
        {
            minClamp = -28.8f;
            maxClamp = 36.5f;
        }
        else{
            minClamp = -80f;
            maxClamp = 80f;
        }
    }
}
