using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;
    private float yRotation = 0f;
    public float xSensitivity = 90f;
    public float ySensitivity = 90f;
    private SkinnedMeshRenderer skinnedMesh;
    
    private Transform left;
    private Transform right;
    private GameObject leftArm;
    private GameObject rightArm;
    
    void Awake(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        skinnedMesh = transform.GetComponentInChildren<SkinnedMeshRenderer>();
        Transform[] bones = skinnedMesh.bones;
        //left = Array.FindIndex(bones, b => b.name == "Upper")
        leftArm = GameObject.Find("Bip01_L_UpperArm");
        rightArm = GameObject.Find("Bip01_R_UpperArm");
        Debug.Log("Right arm:" + rightArm);
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
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        leftArm.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        rightArm.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //Rotate player left and right
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime * xSensitivity);
    }
}
