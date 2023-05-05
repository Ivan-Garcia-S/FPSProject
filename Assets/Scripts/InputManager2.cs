using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager2 : MonoBehaviour
{
    //private PlayerInput playerInput;
    private PlayerInputActions inputActions;
    private PlayerInputActions.PlayerActions playerActions;
    //private PlayerInput.PlayerActions onFoot;
    private PlayerLook playerLook;
    private PlayerMotor motor;
    
    // Start is called before the first frame update
    void Awake()
    {
        //Get Player Look and Movement components
        playerLook = GetComponent<PlayerLook>();
        motor = GetComponent<PlayerMotor>();
        inputActions = new PlayerInputActions();
        playerActions = inputActions.Player;
        inputActions.Player.Enable();

        //Assign funciton calls for when player actions performed
        inputActions.Player.Jump.performed += ctx => motor.Jump();
        inputActions.Player.Sprint.performed += ctx => motor.Sprint();
        inputActions.Player.Crouch.performed += ctx => motor.Crouch();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        motor.ProcessMove(playerActions.Movement.ReadValue<Vector2>());
        motor.processCrouch();
    }

    void LateUpdate() {
        playerLook.ProcessLook(playerActions.Look.ReadValue<Vector2>());
    }
    
    private void onEnable(){
        playerActions.Enable();
    }

    private void onDisable(){
        playerActions.Disable();
    }
}