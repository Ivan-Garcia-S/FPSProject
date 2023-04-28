using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;
    public float xSensitivity = 90f;
    public float ySensitivity = 90f;
    
    public void ProcessLook(Vector2 inputVector)
    {
        float mouseX = inputVector.x;
        float mouseY = inputVector.y;

        //Calculate camera rotation for looking up and down
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        //Apply to camera transform
        cam.transform.localRotation = Quaternion.Euler(xRotation,0,0);

        //Rotate player left and right
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime * xSensitivity);
    }
}
