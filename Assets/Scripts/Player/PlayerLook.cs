using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;

    private GameObject camSphere;
    private GameObject head;
    private float xRotation = 0f;
    private float yRotation = 0f;
    public float xSensitivity = 90f;
    public float ySensitivity = 90f;
    private SkinnedMeshRenderer skinnedMesh;
    
    private GameObject player;
    private Transform left;
    private Transform right;
    private GameObject leftArm;
    private GameObject rightArm;
    
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        skinnedMesh = transform.GetComponentInChildren<SkinnedMeshRenderer>();
        Transform[] bones = skinnedMesh.bones;
        //left = Array.FindIndex(bones, b => b.name == "Upper")
        
        ////Old Model////
        /*
        leftArm = GameObject.Find("Bip01_L_UpperArm");
        rightArm = GameObject.Find("Bip01_R_UpperArm");
        camSphere = GameObject.Find("CamSphere");
        head = GameObject.FindWithTag("PlayerHead");
        */
        
        ////New Model/////
        leftArm = GameObject.Find("mixamorig:LeftArm");
        rightArm = GameObject.Find("mixamorig:RightArm");
        camSphere = GameObject.Find("CamSphere");
        head = GameObject.Find("mixamorig:Head");
        player = gameObject.GetComponentInParent<Animator>().gameObject;
    }

    void Start()
    {
        Debug.Log("offset is " + (transform.position - cam.transform.position));
        //(.1, -1.7, -.1)
    }
    public void ProcessLook(Vector2 inputVector)
    {
        float mouseX = inputVector.x;
        float mouseY = inputVector.y;

        //Calculate camera rotation for looking up and down
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        yRotation += (mouseX * Time.deltaTime) * xSensitivity;

        //Apply to camera transform
        cam.transform.localRotation = Quaternion.Euler(xRotation, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
        //leftArm.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //rightArm.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //Rotate player left and right
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime * xSensitivity);

        //Move Camera Sphere if mouse Y moving
        if(inputVector.y != 0){
            //camSphere.transform.position = head.transform.position;
            //camSphere.transform.rotation = Quaternion.Euler(head.transform.eulerAngles.x, head.transform.eulerAngles.y, camSphere.transform.eulerAngles.z);
        }
    }
}
