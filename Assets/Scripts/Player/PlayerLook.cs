using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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
    public float xSensitivity = 90f;
    public float ySensitivity = 90f;
    
    
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
    public void ProcessLook(Vector2 inputVector)
    {
        float mouseX = inputVector.x;
        float mouseY = inputVector.y;

        //Calculate camera rotation for looking up and down
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, minClamp, maxClamp);

        yRotation += (mouseX * Time.deltaTime) * xSensitivity;

        //Apply to camera transform
        cam.transform.localRotation = Quaternion.Euler(xRotation, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);

        //Rotate player left and right
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime * xSensitivity);
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
