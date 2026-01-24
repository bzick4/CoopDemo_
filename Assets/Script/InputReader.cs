// using System;
// using UnityEngine;
// using UnityEngine.InputSystem;

// [CreateAssetMenu(menuName = "Input/Input Reader")]
// public class InputReader : ScriptableObject, PlayerInputAction.IPlayerActions
// {
//     public event Action<Vector2> MoveEvent;

//     private PlayerInputAction inputActions;
//     private bool initialized;

//     public void Initialize()
//     {
//         if (initialized)
//             return;

//         inputActions = new PlayerInputAction();
//         inputActions.Player.SetCallbacks(this);
//         inputActions.Player.Enable();

//         initialized = true;
//     }

//     public void Shutdown()
//     {
//         if (!initialized)
//             return;

//         inputActions.Player.Disable();
//         inputActions.Dispose();

//         initialized = false;
//     }

//     // ===== MOVE =====
//     public void OnMove(InputAction.CallbackContext context)
//     {
//         if (context.performed)
//             MoveEvent?.Invoke(context.ReadValue<Vector2>());

//         if (context.canceled)
//             MoveEvent?.Invoke(Vector2.zero);
//     }

//     // ===== UNUSED ACTIONS (пока) =====
//     public void OnLook(InputAction.CallbackContext context) { }
//     public void OnAttack(InputAction.CallbackContext context) { }
//     public void OnInteract(InputAction.CallbackContext context) { }
//     public void OnCrouch(InputAction.CallbackContext context) { }
//     public void OnJump(InputAction.CallbackContext context) { }
//     public void OnPrevious(InputAction.CallbackContext context) { }
//     public void OnNext(InputAction.CallbackContext context) { }
//     public void OnSprint(InputAction.CallbackContext context) { }
// }

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, PlayerInputAction.IPlayerActions
{
    public event Action<Vector2> MoveEvent;

    private PlayerInputAction inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
        inputActions.Player.SetCallbacks(this);
    }

    public void EnableInput()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputActions is NULL");
            return;
        }

        inputActions.Player.Enable();
    }

    public void DisableInput()
    {
        if (inputActions == null)
            return;

        inputActions.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            MoveEvent?.Invoke(context.ReadValue<Vector2>());

        if (context.canceled)
            MoveEvent?.Invoke(Vector2.zero);
    }

    // Пустые методы — ОК
    public void OnLook(InputAction.CallbackContext context) { }
    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }
}