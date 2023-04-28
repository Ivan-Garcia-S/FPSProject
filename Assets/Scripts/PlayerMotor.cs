using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{

    private CharacterController controller;
    private Vector3 playerVelocity;
    public float speed = 6f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Recieve input for InputManager.cs and apply them to character controller
    public void ProcessMove(Vector2 input){
        //Debug.Log("In processmove");
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        Debug.Log("moveDirection = " + moveDirection);
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
    }


}
