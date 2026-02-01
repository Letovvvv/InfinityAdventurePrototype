using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions _playerInputActions;

    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerDash;

    private void Awake()
    {
        Instance = this;

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _playerInputActions.Combat.Attack.started += PlayerAttack_started;
        _playerInputActions.Player.Dash.started += PlayerDash_started;

    }

    private void PlayerDash_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerDash.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector;
    }

    public void DisabledMovement()
    {
        _playerInputActions.Disable();
    }

    private void PlayerAttack_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }
}
