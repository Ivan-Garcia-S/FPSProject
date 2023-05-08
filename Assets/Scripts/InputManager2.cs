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
    private WeaponManager weaponManager;
    private WeaponManager2 wM2;
    
    // Start is called before the first frame update
    void Awake()
    {
        //Get Player Look and Movement components
        playerLook = GetComponent<PlayerLook>();
        motor = GetComponent<PlayerMotor>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        wM2 = GetComponentInChildren<WeaponManager2>();
        inputActions = new PlayerInputActions();
        playerActions = inputActions.Player;
        inputActions.Player.Enable();

        //Assign funciton calls for when player actions performed
        inputActions.Player.Jump.performed += ctx => motor.Jump();
        inputActions.Player.Sprint.performed += ctx => motor.Sprint();
        inputActions.Player.Crouch.performed += ctx => motor.Crouch();
        
        /*inputActions.Player.Shoot.started += ctx => weaponManager.StartShoot();
        inputActions.Player.Shoot.canceled += ctx => weaponManager.EndShoot();
        inputActions.Player.Reload.performed += ctx => weaponManager.Reload();
        */
        inputActions.Player.Shoot.started += ctx => wM2.StartShoot();
        inputActions.Player.Shoot.canceled += ctx => wM2.EndShoot();
        inputActions.Player.Reload.performed += ctx => wM2.Reload();

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