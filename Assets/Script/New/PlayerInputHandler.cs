using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerInputHandler : NetworkBehaviour
{
    private PlayerInputActions _input;

    public Vector2 MoveInput { get; private set; }
    public bool RunPressed { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _input = new PlayerInputActions();
        _input.Player.Enable();

        _input.Player.Move.performed += ctx =>
            MoveInput = ctx.ReadValue<Vector2>();

        _input.Player.Move.canceled += ctx =>
            MoveInput = Vector2.zero;

        _input.Player.Run.performed += ctx =>
            RunPressed = true;

        _input.Player.Run.canceled += ctx =>
            RunPressed = false;

        
    }

    private void LateUpdate()
    {
        // Сбрасываем одноразовые кнопки
        JumpPressed = false;
        AttackPressed = false;
    }

    private void OnDisable()
    {
        if (!IsOwner) return;
        _input?.Player.Disable();
    }

    public void ConsumeJump()
{
    JumpPressed = false;
}
}