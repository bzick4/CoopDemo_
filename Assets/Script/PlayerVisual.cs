// using UnityEngine;
// using Unity.Netcode;


// [System.Serializable]
// public struct CharacterEntry
// {
//     public GameObject Prefab;
//     public SOData SODatao;
// }

// public class PlayerVisual : NetworkBehaviour
// {
//     [SerializeField] private GameObject[] visuals;
//     [SerializeField] private Transform visualRoot;

//     private NetworkVariable<int> visualIndex =
//         new NetworkVariable<int>(
//             -1,
//             NetworkVariableReadPermission.Everyone,
//             NetworkVariableWritePermission.Server
//         );

//     public override void OnNetworkSpawn()
// {
//     visualIndex.OnValueChanged += OnVisualChanged;

//     if (IsServer && visualIndex.Value == -1)
//     {
//         visualIndex.Value = Random.Range(0, visuals.Length);
//     }

//     if (visualIndex.Value != -1)
//     {
//         ApplyVisual(visualIndex.Value);
//     }
// }

//     private void OnVisualChanged(int oldValue, int newValue)
//     {
//         ApplyVisual(newValue);
//     }

//     private void ApplyVisual(int index)
//     {
//         foreach (Transform child in visualRoot)
//             Destroy(child.gameObject);

//         Instantiate(visuals[index], visualRoot);
//     }
// }

using UnityEngine;
using Unity.Netcode;

public class PlayerVisual : NetworkBehaviour
{
    [SerializeField] private SOData[] characters;
    [SerializeField] private Transform visualRoot;

    private NetworkVariable<int> characterIndex =
        new NetworkVariable<int>(
            -1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public SOData CurrentSOData { get; private set; }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"OnNetworkSpawn PlayerVisual | IsServer={IsServer}");

        characterIndex.OnValueChanged += OnCharacterChanged;

        if (IsServer && characterIndex.Value == -1)
        {
            characterIndex.Value = Random.Range(0, characters.Length);
        }

        if (characterIndex.Value != -1)
            ApplyCharacter(characterIndex.Value);
    }

    private void OnCharacterChanged(int oldIndex, int newIndex)
    {
        ApplyCharacter(newIndex);
    }

    private void ApplyCharacter(int index)
    {
        // Проверки
        if (characters == null)
        {
            Debug.LogError("characters array is NULL");
            return;
        }

        if (index < 0 || index >= characters.Length)
        {
            Debug.LogError($"Index out of range: {index}");
            return;
        }

        var data = characters[index];
        if (data == null)
        {
            Debug.LogError($"SOData at index {index} is NULL");
            return;
        }

        if (data.VisualPrefab == null)
        {
            Debug.LogError($"SOData {data.name} has NULL VisualPrefab");
            return;
        }

        // Сохраняем SOData
        CurrentSOData = data;

        Debug.Log($"[PlayerVisual] ApplyCharacter index = {index} | SOData name = {data.name}");

        // Очищаем старый визуал
        foreach (Transform child in visualRoot)
            Destroy(child.gameObject);

        // Создаем новый визуал
        var visual = Instantiate(data.VisualPrefab, visualRoot);

        // --- ВАЖНО: сразу инициализируем CharControl ---
        var control = GetComponent<CharControl>();
        if (control != null)
        {
            control.InitStats(CurrentSOData);
        }
    }
}