using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    NewControls inputActions;
    NewControls.PlayerControlsActions controlsActions;

    public bool GameIsRunning;

    [SerializeField] GameEngineManager manager;

    private void Awake()
    {
        inputActions = new();
        controlsActions = inputActions.PlayerControls;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        controlsActions.Enable();
    }

    private void OnDisable()
    {
        controlsActions.Disable();
    }

    private void Update()
    {
        if (!GameIsRunning) return;
        GetInputs();
    }

    private void GetInputs()
    {
        manager.HandlePlayerInput(0, 
            controlsActions.Move.ReadValue<Vector2>(), 
            Quaternion.Euler(Camera.main.transform.eulerAngles.x, 
            Camera.main.transform.eulerAngles.y, 0), 
            controlsActions.Fly.IsPressed(), 
            controlsActions.Boost.IsPressed(), 
            controlsActions.Attack.IsPressed());
    }
}
