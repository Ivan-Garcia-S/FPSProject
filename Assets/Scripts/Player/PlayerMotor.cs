using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{

    private CharacterController controller;
    //private CharacterController controller;
    [SerializeField ] private Animator animator;
    private Vector3 playerVelocity;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float gravity = -9.8f;
    private bool isGrounded;
    private float jumpHeight = 1.2f;
    private bool crouching = false;
    private bool prone = false;
    private bool sprinting = false;
    private float crouchMultiplier = 0.5f;
  

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        //controller = GameObject.Find("Player").GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Constantly update if character is grounded
        isGrounded = controller.isGrounded;
    }
    //Recieve input for InputManager.cs and apply them to character controller
    public void ProcessMove(Vector2 input)
    {

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        
        //If not moving, turn off sprinting
        if(moveDirection == Vector3.zero)
        {
            StopMovementExceptFor("idle");
        }
        else if(moveDirection.z == 0 && moveDirection.x > 0)
        {
            StopMovementExceptFor("strafeRight");
        }
        else if(moveDirection.z == 0 && moveDirection.x < 0)
        {
            StopMovementExceptFor("strafeLeft");
        }
        else if(moveDirection.z < 0){
            StopMovementExceptFor("moveBack");
        }
        else if (moveDirection.z > 0)
        {
            //Don't want to stop sprinting for this one
            StopMovementExceptFor("moveForward", false);
        }
        
        float trueSpeed = speed;
        if(crouching){
            trueSpeed *= crouchMultiplier;
        }
        controller.Move(transform.TransformDirection(moveDirection) * trueSpeed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if(isGrounded && playerVelocity.y < 0){
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
        // Debug.Log("Y Velocity = " + playerVelocity.y);

    }

    public void Crouch()
    {
        if(isGrounded)
        {
            if(prone) Prone();
            //First stop sprinting if trying to crouch
            if(sprinting) StopSprint();
        
            //Signal to crouch
            crouching = !crouching;  
            animator.SetBool("crouched", crouching);
        }
    }

    public void Prone()
    {
        if(isGrounded)
        {
            if(sprinting) StopSprint();
            else if(crouching) Crouch();
            prone = !prone;
            animator.SetBool("prone", prone);
        }
        

    }
    public void Jump()
    {
        if(isGrounded){
            //If prone, don't jump, crouch
            if(prone) Crouch();
            //If crouching or standing, jump
            else
            {
                if(crouching) Crouch();
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
                if(sprinting) StopSprint();
            }
        }
    }

    private void ChangeParentScale(Transform parent,Vector3 scale)
    {
         List<Transform> children= new List<Transform>();
         foreach(Transform child in parent) {
             child.parent = null;
             children.Add(child);
         }
         parent.localScale = scale;
         foreach(Transform child in children) child.parent = parent;    
        
    }

    public void Sprint()
    {
        //If crouching uncrouch
        if (crouching) Crouch();
        //If prone, stand
        else if(prone) Prone();
        //Only change sprint if not ADS and not in air and not idle
        if(!animator.GetBool("aimingDown") && isGrounded && !animator.GetBool("idle"))
        {
            sprinting = !sprinting;
            if(sprinting)
            {
                //Stop reload if necessary
                if(gameObject.GetComponentInChildren<WeaponManager2>().reloading) gameObject.GetComponentInChildren<WeaponManager2>().CancelReload();
                animator.SetBool("strafeLeft", false);
                animator.SetBool("strafeRight", false);
                animator.SetBool("moveForward",false);
            }
            
            animator.SetBool("run",sprinting);
            
            
            speed = sprinting ? 7 : 4;
        }
    }

    public void StopSprint()
    {
        animator.SetBool("run", false);
        sprinting = false;
        speed = 4;      
    }

    public void StopMovementExceptFor(string exception, bool stopSprint=true)
    {
        if(stopSprint) StopSprint();
        animator.SetBool("strafeRight", false);
        animator.SetBool("moveForward", false);
        animator.SetBool("strafeLeft", false);
        animator.SetBool("moveBack", false);
        animator.SetBool("idle", false);
        animator.SetBool(exception, true);
    }

    public bool isCrouched()
    {
        return crouching;
    }
}
