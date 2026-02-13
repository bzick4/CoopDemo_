using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, InputPlayerAction.IPlayerActions
{
    private InputPlayerAction _actions;

    // События для движения и камеры
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    public event Action<bool> RunEvent;

    private void OnEnable()
    {
        if (_actions == null)
        {
            _actions = new InputPlayerAction();
            _actions.Player.SetCallbacks(this);
        }
    }

    public void EnableInput() => _actions.Player.Enable();
    public void DisableInput() => _actions.Player.Disable();

    public void OnActionMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnActionLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            RunEvent?.Invoke(true);
        else if (context.phase == InputActionPhase.Canceled)
            RunEvent?.Invoke(false);
    }

    // Если есть другие Actions, оставляем пустыми
    public void OnJump(InputAction.CallbackContext context) { }
    public void OnFire(InputAction.CallbackContext context) { }

   
}