// using UnityEngine;
// using Unity.Netcode;
// using UnityEngine.InputSystem;

// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(PlayerInput))]
// public class CarController : NetworkBehaviour
// {
//     public float motorForce = 1500f;
//     public float turnForce = 300f;

//     private Rigidbody rb;
//     private PlayerInput playerInput;
//     private InputAction moveAction;

//     private Vector2 clientInput;  // ввод клиента
//     private Vector2 serverInput;  // ввод сервера

//     private void Awake()
//     {
//         rb = GetComponent<Rigidbody>();
//         playerInput = GetComponent<PlayerInput>();
//         moveAction = playerInput.actions["Move"];
//     }

//     public override void OnNetworkSpawn()
//     {
//         if (IsOwner)
//         {
//             // Ждём один кадр, чтобы ownership точно установился
//             StartCoroutine(EnableInputNextFrame());
//         }
//     }

//     private System.Collections.IEnumerator EnableInputNextFrame()
//     {
//         yield return null; // один кадр
//         moveAction.Enable();
//         Debug.Log("Input enabled");
//     }

//     private void Update()
//     {
//         if (!IsOwner) return;

//         // Читаем ввод напрямую
//         clientInput = moveAction.ReadValue<Vector2>();
//         SubmitInputServerRpc(clientInput);
//         Debug.Log($"Client input: {clientInput}");
//     }

//     [ServerRpc]
//     private void SubmitInputServerRpc(Vector2 input)
//     {
//         serverInput = input;
//         Debug.Log($"Server received input: {serverInput}");
//     }

//     private void FixedUpdate()
//     {
//         if (!IsServer) return;

//         rb.AddForce(transform.forward * serverInput.y * motorForce); // убрали fixedDeltaTime
//         rb.AddTorque(Vector3.up * serverInput.x * turnForce);

//         Debug.Log($"Server moving | input: {serverInput} | velocity: {rb.linearVelocity}");
//     }
// }