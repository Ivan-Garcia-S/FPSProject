using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    private PlayerInputActions.PlayerActions playerActions;
    private PlayerMovement playerMovement;
    //private PlayerInput.PlayerActions onFoot;
    private PlayerLook playerLook;
    
    // Start is called before the first frame update
    void Awake()
    {
        //Get Player Look and Movement components
        playerMovement = GetComponent<PlayerMovement>();
        playerLook = GetComponent<PlayerLook>();
        inputActions = new PlayerInputActions();
        playerActions = inputActions.Player;
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += playerMovement.onJump;
        inputActions.Player.Crouch.performed += playerMovement.onCrouch;
        inputActions.Player.Sprint.performed += playerMovement.onSprint;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerMovement.processMove(playerActions.Movement.ReadValue<Vector2>());
        playerMovement.processCrouch();
    }

    void LateUpdate() {
        Debug.Log("In late update");
        playerLook.ProcessLook(playerActions.Look.ReadValue<Vector2>());
    }
    
    private void onEnable(){
        playerActions.Enable();
    }

    private void onDisable(){
        playerActions.Disable();
    }
}
