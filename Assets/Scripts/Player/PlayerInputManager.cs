using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("References")]
    private PlayerInputActions InputActions;
    private PlayerInputActions.PlayerActions PlayerActions;
    private PlayerLook PlayerLook;
    private PlayerMotor motor;
    private WeaponManager WeaponManager;
    private GameManager Game;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        //Get Player Look and Movement components
        PlayerLook = GetComponent<PlayerLook>();
        motor = GetComponent<PlayerMotor>();
        WeaponManager = GetComponentInChildren<WeaponManager>();
        Game = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

    }
    void Start()
    {
        
        InputActions = Game.PlayerInputActions;
        PlayerActions = InputActions.Player;
        InputActions.Player.Enable();

        //Assign funciton calls for when player actions performed
        InputActions.Player.Jump.performed += ctx => motor.Jump();
        InputActions.Player.Sprint.performed += ctx => motor.Sprint();
        InputActions.Player.Crouch.performed += ctx => motor.Crouch();
        InputActions.Player.Prone.performed += ctx => motor.Prone();
        InputActions.Player.Shoot.started += ctx => WeaponManager.StartShoot();
        InputActions.Player.Shoot.canceled += ctx => WeaponManager.EndShoot();
        InputActions.Player.Reload.performed += ctx => WeaponManager.Reload();
        InputActions.Player.ADSPress.performed += ctx => WeaponManager.AimingInPressed();
        InputActions.Player.ADSRelease.performed += ctx => WeaponManager.AimingInReleased();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        motor.ProcessMove(PlayerActions.Movement.ReadValue<Vector2>());
        PlayerLook.ProcessLook(PlayerActions.Look.ReadValue<Vector2>());
    }

    void LateUpdate() {
        //playerLook.ProcessLook(PlayerActions.Look.ReadValue<Vector2>());
    }
    
    private void onEnable(){
        PlayerActions.Enable();
    }

    private void onDisable(){
        PlayerActions.Disable();
    }

    public void DisableInputActions()
    {
        PlayerActions.Disable();
    }
    public void EnableInputActions()
    {
        PlayerActions.Enable();
    }
}