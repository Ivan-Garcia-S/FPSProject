using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{

    [Header("References")]
    private CharacterController controller;
    //private CharacterController controller;
    [SerializeField] private Animator animator;

    [Header("Player Info")]
    private Vector3 playerVelocity;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float gravity = -9.8f;
    private bool isGrounded;
    private float jumpHeight = 1.4f;
    private bool isCrouching = false;
    private bool isProne = false;
    bool isSprinting = false;
    private float crouchMultiplier = 0.5f;
  
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        //Constantly update if character is grounded
        isGrounded = controller.isGrounded;
        if(isGrounded && animator.GetCurrentAnimatorStateInfo(2).IsName("Jump")) animator.SetTrigger("stopJump");
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
        if(isCrouching){
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
        if(isGrounded || isProne)
        {
            if(isProne) Prone();
            //First stop sprinting if trying to crouch
            if(isSprinting) StopSprint();
        
            //Signal to crouch
            isCrouching = !isCrouching;  
            animator.SetBool("crouched", isCrouching);
        }
    }

    public void Prone()
    {
        //Toggle prone
        if(isGrounded || isProne)
        {
            if(isSprinting) StopSprint();
            else if(isCrouching) Crouch();
            isProne = !isProne;
            animator.SetBool("prone", isProne);
        }
        

    }
    public void Jump()
    {
        if(isGrounded || isProne){
            //If prone, don't jump, crouch
            if(isProne) Crouch();
            //If crouching or standing, jump
            else
            {
                if(isCrouching) Crouch();
                animator.SetTrigger("jump");
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
                //animator.SetTrigger("jump");
                if(isSprinting) StopSprint();
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
        if (isCrouching) Crouch();
        //If prone, stand
        else if(isProne) Prone();
        //Only change sprint if not ADS and not in air and not idle
        if(!animator.GetBool("aimingDown") && isGrounded && !animator.GetBool("idle"))
        {
            isSprinting = !isSprinting;
            if(isSprinting)
            {
                //Stop reload if necessary
                if(gameObject.GetComponentInChildren<WeaponManager>().reloading) gameObject.GetComponentInChildren<WeaponManager>().CancelReload();
                animator.SetBool("strafeLeft", false);
                animator.SetBool("strafeRight", false);
                animator.SetBool("moveForward",false);
            }
            
            animator.SetBool("run",isSprinting);
            
            
            speed = isSprinting ? 7 : 4;
        }
    }

    public void StopSprint()
    {
        animator.SetBool("run", false);
        isSprinting = false;
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

    public bool IsCrouched()
    {
        return isCrouching;
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }

    
}
