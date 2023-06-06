using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{

    private CharacterController controller;
    [SerializeField ]private Animator animator;
    private Vector3 playerVelocity;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float gravity = -9.8f;
    private bool isGrounded;
    private float jumpHeight = 1.2f;
    private bool lerpCrouch = false;
    private bool crouching = false;
    private float crouchTimer = 1f;
    private bool sprinting;
    private float crouchMultiplier = 0.5f;
  

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        sprinting = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
    }
    //Recieve input for InputManager.cs and apply them to character controller
    public void ProcessMove(Vector2 input)
    {

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        
        //If not moving, turn off sprinting
        if(moveDirection == Vector3.zero) StopSprint();
        
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

     public void processCrouch()
    {
        //Perform crouch when button pressed
        if(lerpCrouch){
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if (crouching){
                ChangeParentScale(transform,new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, .6f, p), transform.localScale.z));
            }
            
            else{
                ChangeParentScale(transform, new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 1, p), transform.localScale.z));
            }
            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }

    public void Crouch()
    {
        //First stop sprinting if trying to crouch
        if(sprinting){
            Sprint();
        }
        //Signal to crouch
        crouching = !crouching;  
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void Jump()
    {
        if(isGrounded){
            if(crouching) Crouch();
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
            if(sprinting) StopSprint();
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
        if (crouching){
            Crouch();
        }
        //Only change sprint if not ADS and not in air
        if(!animator.GetBool("aimingDown") && isGrounded){
            sprinting = !sprinting;
            animator.SetBool("isSprinting",sprinting);
            //If crouching uncrouch
            
            if(sprinting){
                speed = 7;
            }
            else{
                speed = 4;
            }
        }
    }

    public void StopSprint()
    {
        animator.SetBool("isSprinting",false);
        if(sprinting) 
        {
            sprinting = false;
            speed = 4;
        }
    }
}
