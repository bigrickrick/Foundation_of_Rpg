using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum MovementMode
    {
        Normal,
        Grid
    }
    public MovementMode movementMode;
    public void Start()
    {
        movementMode = MovementMode.Normal;
    }

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SwitchMovementMode()
    {
        Debug.Log("switchMovementModeButtonClicked");
        if (movementMode == MovementMode.Normal)
        {

            movementMode = MovementMode.Grid;
        }
        else
        {
            movementMode = MovementMode.Normal;
        }

        Debug.Log("Movement mode switched to: " + movementMode);
    }

}
