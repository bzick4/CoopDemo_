// using UnityEngine;
// using Unity.Netcode;
// using Unity.Collections;
// using TMPro;


// public class PlayerNameDisplay : NetworkBehaviour
// {
    
//     [SerializeField] private TMP_Text nameText;

//     private NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>(
//         "Player",
//         NetworkVariableReadPermission.Everyone,
//         NetworkVariableWritePermission.Server
//     );

//     public override void OnNetworkSpawn()
//     {
//         base.OnNetworkSpawn();

//         playerName.OnValueChanged += OnNameChanged;

//         if (IsOwner)
//         {
//             // Пустышка появилась — показываем UI ввода имени
//             NameInputUI nameUI = FindObjectOfType<NameInputUI>();
//             if (nameUI != null)
//             {
//                 nameUI.ShowForPlayer(this);
//                 Debug.Log("[NameDisplay] Пустышка спавнулась → показываем UI ввода имени");
//             }
//             else
//             {
//                 Debug.LogWarning("[NameDisplay] NameInputUI не найден в сцене!");
//             }
//         }

//         OnNameChanged(default, playerName.Value);
//     }

//     private void OnNameChanged(FixedString128Bytes oldName, FixedString128Bytes newName)
//     {
//         if (nameText != null)
//         {
//             nameText.text = newName.ToString();
//             Debug.Log($"Имя обновлено: {newName}");
//         }
//     }

//     // Публичный метод — вызывается из UI после подтверждения
//     public void SetName(string name)
//     {
//         if (IsOwner)
//         {
//             SetPlayerNameServerRpc(name);
//         }
//     }

//     [ServerRpc(RequireOwnership = false)]
//     private void SetPlayerNameServerRpc(string name)
//     {
//         if (string.IsNullOrEmpty(name)) name = "Anonymous";
//         if (name.Length > 20) name = name.Substring(0, 20);

       
//         playerName.Value = name;
//         Debug.Log($"Сервер установил имя: {name}");
//     }
// }

using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class PlayerNameDisplay : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text nameText;

    private Transform cam;

    private NetworkVariable<FixedString128Bytes> playerName =
        new NetworkVariable<FixedString128Bytes>(
            "Player",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        cam = Camera.main != null ? Camera.main.transform : null;

        playerName.OnValueChanged += OnNameChanged;

        if (IsOwner)
        {
            NameInputUI nameUI = FindObjectOfType<NameInputUI>();
            if (nameUI != null)
            {
                nameUI.ShowForPlayer(this);
                Debug.Log("[NameDisplay] Показываем UI ввода имени");
            }
        }

        // Принудительно обновляем текст при спавне
        OnNameChanged(default, playerName.Value);
    }

    private void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main != null)
                cam = Camera.main.transform;
            else
                return;
        }

        // Поворачиваем только объект с текстом
        transform.LookAt(transform.position + cam.forward);
    }

    private void OnNameChanged(FixedString128Bytes oldName, FixedString128Bytes newName)
    {
        if (nameText != null)
            nameText.text = newName.ToString();
    }

    public void SetName(string name)
    {
        if (!IsOwner) return;
        SetPlayerNameServerRpc(name);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = "Anonymous";

        if (name.Length > 20)
            name = name.Substring(0, 20);

        playerName.Value = name;
    }
}