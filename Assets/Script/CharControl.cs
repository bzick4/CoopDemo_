using System.Data.Common;
using Unity.Netcode;
using UnityEngine;

public class CharControl : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float speed;

    private Vector2 moveInput;



   public override void OnNetworkSpawn()
{
    if (!IsOwner)
        return;

    if (inputReader == null)
    {
        Debug.LogError("InputReader is NOT assigned!");
        return;
    }

    inputReader.EnableInput();
    inputReader.MoveEvent += OnMove;
}

public override void OnNetworkDespawn()
{
    if (!IsOwner)
        return;

    inputReader.MoveEvent -= OnMove;
    inputReader.DisableInput();
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

        SendMoveToServer();
    }

    private void SendMoveToServer()
    {
        MoveServerRpc(moveInput);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 input)
    {
    
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        Vector3 direction =
            forward * input.y +
            right * input.x;

        transform.position += direction * speed * Time.deltaTime;
    }
}
