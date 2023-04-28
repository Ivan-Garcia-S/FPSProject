using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerMovement playerMovement;
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Grounded2");
        if(collision.transform.tag == "Solid"){
            playerMovement.isGrounded = true;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Grounded2");
        if(collision.transform.tag == "Solid"){
            playerMovement.isGrounded = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Not Grounded2");
        if(collision.transform.tag == "Solid"){
            playerMovement.isGrounded = false;
        }
    }
}
