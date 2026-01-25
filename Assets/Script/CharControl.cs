using UnityEngine;
using Unity.Cinemachine;
using Unity.Netcode;


public class CharControl : NetworkBehaviour
{
    [SerializeField] private InputReader _InputReader;
    [SerializeField] private float _Speed;
    [SerializeField] private Transform _CameraTransform;



    private Vector2 moveInput;



   public override void OnNetworkSpawn()
{
    if (!IsOwner)
        return;

    if (_InputReader == null)
    {
        Debug.LogError("InputReader is NOT assigned!");
        return;
    }
    
    _InputReader.EnableInput();
    _InputReader.MoveEvent += OnMove;
}

public override void OnNetworkDespawn()
{
    if (!IsOwner)
        return;

    _InputReader.MoveEvent -= OnMove;
    _InputReader.DisableInput();
}


    // private void OnDestroy()
    // {
    //     if (!IsOwner)
    //         return;

    //     inputReader.MoveEvent -= OnMove;
    // }

    private void OnMove(Vector2 input)
{
    Debug.Log("MOVE INPUT: " + input);
    moveInput = input;
}

    private void Update()
{
    if (!IsOwner)
        return;

    RotateToCamera();
    SendMoveToServer();
}

    private void SendMoveToServer()
    {
        MoveServerRpc(moveInput);
    }

    private void RotateToCamera()
{
    Vector3 camForward = _CameraTransform.forward;
    camForward.y = 0f;

    if (camForward.sqrMagnitude < 0.001f)
        return;

    transform.forward = camForward;
}
//     private void RotateToCamera()
// {
//     Vector3 camForward = _CameraTransform.forward;
//     camForward.y = 0f;

//     if (camForward.sqrMagnitude < 0.001f)
//         return;

//     transform.forward = camForward;
// }

 
   [ServerRpc]
private void MoveServerRpc(Vector2 input)
{
    Vector3 forward = transform.forward;
    Vector3 right = transform.right;

    forward.y = 0;
    right.y = 0;

    Vector3 direction = forward * input.y + right * input.x;
    transform.position += direction * _Speed * Time.deltaTime;
}
}
