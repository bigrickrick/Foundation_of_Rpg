using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameInput : MonoBehaviour
{

    public static GameInput Instance { get; private set; }
    private PlayerInputAction playerInputAction;
    
    public event EventHandler OnShoot;

    
    public event EventHandler OnPause;
    public event EventHandler OnScroll;
    public event EventHandler OnRotateCamera;
    public event EventHandler OnRotateCamerastoped;
    public event EventHandler OnRotateCameraAlternate;
    public event EventHandler OnRotateCamerastopedAlternate;
    public void Awake()
    {
        Instance = this;
        playerInputAction = new PlayerInputAction();
        playerInputAction.Player.Enable();
        
        playerInputAction.Player.Shoot.performed += Shoot_performed;
      
        
        playerInputAction.Player.PauseMenu.performed += PauseMenu_performed;
        playerInputAction.Player.Scroll.performed += Scroll_performed;
        playerInputAction.Player.RotateCamera.performed += RotateCamera_performed;
        playerInputAction.Player.RotateCamera.canceled += RotateCamera_canceled;
        playerInputAction.Player.RotateCameraAlternate.performed += RotateCameraAlternate_performed;
        playerInputAction.Player.RotateCameraAlternate.canceled += RotateCameraAlternate_canceled;
    }

    private void RotateCameraAlternate_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRotateCamerastopedAlternate?.Invoke(this, EventArgs.Empty);
    }

    private void RotateCameraAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRotateCameraAlternate?.Invoke(this, EventArgs.Empty);
    }

    private void RotateCamera_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRotateCamerastoped?.Invoke(this, EventArgs.Empty);
    }

    private void RotateCamera_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRotateCamera?.Invoke(this, EventArgs.Empty);
    }

    private void Scroll_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnScroll?.Invoke(this, EventArgs.Empty);
    }

    private void PauseMenu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(InvokeShootNextFrame());
    }

    private IEnumerator InvokeShootNextFrame()
    {
        yield return null; 
        OnShoot?.Invoke(this, EventArgs.Empty);
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();


        inputVector = inputVector.normalized;


        return inputVector;
    }

    public float GetHorizontalRotationInput()
    {
        
        return Input.GetAxis("Horizontal"); 
    }
}
    

