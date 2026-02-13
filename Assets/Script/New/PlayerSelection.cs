// using Unity.Netcode;
// using UnityEngine;

// public class PlayerSelection : NetworkBehaviour
// {
//     private int selectedIndex = 0;

//     public void ChooseCharacter(int index)
//     {
//         selectedIndex = index;
//         Debug.Log($"Выбран персонаж {index}");
        
//         // Отправляем серверу, если уже подключены
//         if (IsClient && IsOwner)
//             SendSelectionServerRpc(selectedIndex);
//     }

//     [ServerRpc]
//     private void SendSelectionServerRpc(int index, ServerRpcParams rpcParams = default)
//     {
//         // Сервер сам спавнит выбранный префаб для этого клиента
//         PlayerSpawner.Instance.SpawnPlayer(rpcParams.Receive.SenderClientId, index);
//     }
// }