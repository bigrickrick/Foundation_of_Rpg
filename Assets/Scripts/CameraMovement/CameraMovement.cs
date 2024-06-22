using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera Playercamera;
    public float CameraSpeed;
    public float scrollspeed;
    public float rotationSpeed;
    [SerializeField] private GameInput gameInput;
    private Transform movementTransform;
    private Transform rotationPoint;
    public bool Rotate;
    public bool RotateAlternate;

    private void Start()
    {
        gameInput.OnScroll += GameInput_OnScroll;
        gameInput.OnRotateCamera += GameInput_OnRotateCamera;
        gameInput.OnRotateCamerastoped += GameInput_OnRotateCamerastoped;
        gameInput.OnRotateCameraAlternate += GameInput_OnRotateCameraAlternate;
        gameInput.OnRotateCamerastopedAlternate += GameInput_OnRotateCamerastopedAlternate;

        movementTransform = new GameObject("MovementTransform").transform;
        movementTransform.SetParent(transform); 
        movementTransform.localPosition = Vector3.zero; 
        rotationPoint = transform;
    }

    private void GameInput_OnRotateCamerastopedAlternate(object sender, System.EventArgs e)
    {
        RotateAlternate = false;
    }

    private void GameInput_OnRotateCameraAlternate(object sender, System.EventArgs e)
    {
        RotateAlternate = true;
    }

    private void Update()
    {
        HandleMovement();
        ScrollUpOrDown();
        if (Rotate)
        {
            RotateCamera(1);
        }
        if (RotateAlternate)
        {
            RotateCamera(-1);
        }

    }

    private void GameInput_OnRotateCamerastoped(object sender, System.EventArgs e)
    {
        Rotate = false;
    }

    private void GameInput_OnRotateCamera(object sender, System.EventArgs e)
    {
        Rotate = true;
    }

    private void RotateCamera(float direction)
    {
        float rotationDirection = direction;
        transform.RotateAround(rotationPoint.position, Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime);
    }

    private void GameInput_OnScroll(object sender, System.EventArgs e)
    {
        ScrollUpOrDown();
    }

    private void HandleMovement()
    {
        Vector3 inputVector = new Vector3(gameInput.GetMovementVectorNormalized().x, 0f, gameInput.GetMovementVectorNormalized().y);
        Vector3 moveDir = movementTransform.TransformDirection(inputVector);
        transform.position += moveDir * CameraSpeed * Time.deltaTime;
    }

    public float GetScrollAmount()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }

    private void ScrollUpOrDown()
    {
        float scrollAmount = GetScrollAmount();
        Vector3 newPosition = transform.position + Vector3.up * scrollAmount * scrollspeed * CameraSpeed * Time.deltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, 5f, 30f);
        transform.position = newPosition;
    }

    private void OnDestroy()
    {
        if (movementTransform != null)
        {
            Destroy(movementTransform.gameObject);
        }
    }
}
