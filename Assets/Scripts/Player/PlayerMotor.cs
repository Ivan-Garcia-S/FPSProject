using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{

    [Header("References")]
    private CharacterController Controller;
    private PlayerLook PlayerLook;
    private WeaponManager WeaponManager;
    public AudioSource FootAudioSource;
    public AudioClip WalkSound;
    public AudioClip SprintSound;

    [SerializeField] private Animator Animator;

    [Header("Player Info")]
    private Vector3 playerVelocity;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float gravity = -9.8f;
    public bool isGrounded;
    private float jumpHeight = 1.4f;
    private bool isCrouching = false;
    private bool isProne = false;
    bool isSprinting = false;
    private float crouchMultiplier = 0.5f;
    private float adsMultiplier = 0.7f;
  
    void Start()
    {

        PlayerLook = GetComponent<PlayerLook>();
        Controller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        WeaponManager = GetComponentInChildren<WeaponManager>();
    }

    void Update()
    {
        //Constantly update if character is grounded
        isGrounded = Controller.isGrounded;
        if(isGrounded && Animator.GetCurrentAnimatorStateInfo(2).IsName("Jump")) Animator.SetTrigger("stopJump");
        
        //Play footstep noise if walking
        if(Animator.GetBool("idle") || !isGrounded || isProne)
        {
            FootAudioSource.Stop();
        }
        else if(!Animator.GetBool("idle") && !isSprinting){
            if(FootAudioSource.isPlaying && FootAudioSource.clip != WalkSound){
                FootAudioSource.Stop();
                FootAudioSource.clip = WalkSound;
                FootAudioSource.Play();
            }
            else if(!FootAudioSource.isPlaying){
                FootAudioSource.clip = WalkSound;
                FootAudioSource.Play();
            }
        }
        else if(isSprinting){
             if(FootAudioSource.isPlaying && FootAudioSource.clip != SprintSound){
                FootAudioSource.Stop();
                FootAudioSource.clip = SprintSound;
                FootAudioSource.Play();
            }
            else if(!FootAudioSource.isPlaying){
                FootAudioSource.clip = SprintSound;
                FootAudioSource.Play();
            }
        }
    }
    //Recieve input for InputManager.cs and apply them to character controller
    public void ProcessMove(Vector2 input)
    {
        //if(typeInput.device.description.deviceClass ==)

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
        if(isCrouching || isProne){
            trueSpeed *= crouchMultiplier;
        }
        Controller.Move(transform.TransformDirection(moveDirection) * trueSpeed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if(isGrounded && playerVelocity.y < 0){
            playerVelocity.y = -2f;
        }
        Controller.Move(playerVelocity * Time.deltaTime);
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
            Animator.SetBool("crouched", isCrouching);
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
            Animator.SetBool("prone", isProne);
            PlayerLook.SetProneSettings(isProne);
            WeaponManager.SetProneSettings(isProne);
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
                Animator.SetTrigger("jump");
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
        if(!Animator.GetBool("aimingDown") && isGrounded && !Animator.GetBool("idle"))
        {
            isSprinting = !isSprinting;
            if(isSprinting)
            {
                //Stop reload if necessary
                if(gameObject.GetComponentInChildren<WeaponManager>().reloading) gameObject.GetComponentInChildren<WeaponManager>().CancelReload();
                Animator.SetBool("strafeLeft", false);
                Animator.SetBool("strafeRight", false);
                Animator.SetBool("moveForward",false);
            }
            
            Animator.SetBool("run",isSprinting);
            
            
            speed = isSprinting ? 7 : 4;
        }
    }

    public void StopSprint()
    {
        Animator.SetBool("run", false);
        isSprinting = false;
        speed = 4;      
    }

    public void StopMovementExceptFor(string exception, bool stopSprint=true)
    {
        if(stopSprint) StopSprint();
        Animator.SetBool("strafeRight", false);
        Animator.SetBool("moveForward", false);
        Animator.SetBool("strafeLeft", false);
        Animator.SetBool("moveBack", false);
        Animator.SetBool("idle", false);
        Animator.SetBool(exception, true);
    }

    public bool IsCrouched()
    {
        return isCrouching;
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }

    public void ADSActive(bool isADS)
    {
        speed = isADS ? speed * adsMultiplier : speed / adsMultiplier;
    }
    
}
