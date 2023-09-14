using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("References")]
    private PlayerInputActions inputActions;
    private PlayerInputActions.PlayerActions playerActions;
    private PlayerLook playerLook;
    private PlayerMotor motor;
    private WeaponManager weaponManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        //Get Player Look and Movement components
        playerLook = GetComponent<PlayerLook>();
        motor = GetComponent<PlayerMotor>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        inputActions = new PlayerInputActions();
        playerActions = inputActions.Player;
        inputActions.Player.Enable();

        //Assign funciton calls for when player actions performed
        inputActions.Player.Jump.performed += ctx => motor.Jump();
        inputActions.Player.Sprint.performed += ctx => motor.Sprint();
        inputActions.Player.Crouch.performed += ctx => motor.Crouch();
        inputActions.Player.Prone.performed += ctx => motor.Prone();
        inputActions.Player.Shoot.started += ctx => weaponManager.StartShoot();
        inputActions.Player.Shoot.canceled += ctx => weaponManager.EndShoot();
        inputActions.Player.Reload.performed += ctx => weaponManager.Reload();
        inputActions.Player.ADSPress.performed += ctx => weaponManager.AimingInPressed();
        inputActions.Player.ADSRelease.performed += ctx => weaponManager.AimingInReleased();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        motor.ProcessMove(playerActions.Movement.ReadValue<Vector2>());
        playerLook.ProcessLook(playerActions.Look.ReadValue<Vector2>());
    }

    void LateUpdate() {
        //playerLook.ProcessLook(playerActions.Look.ReadValue<Vector2>());
    }
    
    private void onEnable(){
        playerActions.Enable();
    }

    private void onDisable(){
        playerActions.Disable();
    }
}