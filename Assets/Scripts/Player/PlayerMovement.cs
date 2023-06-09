using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
   // private PlayerInput input;
    //private PlayerInputActions inputActions;
    private Vector2 moveVector = Vector2.zero;
    private Rigidbody playerRB;
    //private CharacterController controller;
    public float speed = 4f;
    public bool isGrounded;
    public float maxForce = 5f;
    public float jumpForce = 2.5f;
    public bool lerpCrouch = false;
    public bool crouching = false;
    public float crouchTimer = 1f;
    public bool sprinting = false;

    
    private void Awake() 
    {
        //Set rigidbody variable
        playerRB = GetComponent<Rigidbody>();

    }
    void OnCollisionEnter(Collision collision)
    {
        //Check if Player touching ground
        Debug.Log("Grounded");
        if(collision.transform.tag == "Ground"){
            isGrounded = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        //Check if Player not touching ground
        Debug.Log("Not Grounded");
        if(collision.transform.tag == "Ground"){
            isGrounded = false;
        }
    }

    public void processMove(Vector2 inputVector)
    {
        Vector3 currentVelocity = playerRB.velocity;
        Vector3 targetVelocity = new Vector3(inputVector.x, 0, inputVector.y); 
        targetVelocity *= speed;

        //Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);

        //Calculate correct force
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        Vector3.ClampMagnitude(velocityChange, maxForce);
        //playerMovement.GetComponent<Rigidbody>().AddForce(new Vector3(inputVector.x, 0, inputVector.y) * playerMovement.speed,ForceMode.Force);
        playerRB.AddForce(velocityChange,ForceMode.VelocityChange);
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

    public void onCrouch(InputAction.CallbackContext context)
    {
        //Signal to crouch
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }
    /*private void FixedUpdate() {
        Debug.Log(moveVector);
        Debug.Log("Grounded = " + isGrounded);
        
        Vector2 inputVector = inputActions.Player.Movement.ReadValue<Vector2>();
        playerRB.AddForce(new Vector3(inputVector.x, 0, inputVector.y) * speed,ForceMode.Force);
    }
    */

    public void onJump(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        if (context.performed && isGrounded){
            Vector3 jumpVector = Vector3.up * jumpForce;
            playerRB.AddForce(jumpVector, ForceMode.VelocityChange);
        }
    }
    public Vector2 onMove(InputAction.CallbackContext context)
    {
        return context.ReadValue<Vector2>();
    }
    public void onSprint(InputAction.CallbackContext context)
    {
        sprinting = !sprinting;
        if(sprinting){
            speed = 7;
        }
        else{
            speed = 4;
        }
    }

     
    /*private void OnEnable() {
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCancelled;
    }

    private void OnDisable() {
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCancelled;
    }

    
    private void OnMovementPerformed(InputAction.CallbackContext context){
        moveVector = context.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext context){
        moveVector = Vector2.zero;
    }
    */
   

}
