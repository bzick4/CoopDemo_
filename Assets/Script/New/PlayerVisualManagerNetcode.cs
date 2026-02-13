// using UnityEngine;
// using Unity.Netcode;

// public class PlayerVisualManagerNetcode : NetworkBehaviour
// {
//     public static PlayerVisualManagerNetcode Instance;

//     private int pendingIndex = -1; // индекс выбранного визуала до спавна
//     private PlayerVisual playerVisual;

//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }

//     // Вызывается кнопками для выбора визуала
//     public void ChooseVisual(int index)
//     {
//         if (playerVisual != null && playerVisual.IsOwner)
//         {
//             playerVisual.SetVisualServerRpc(index); // отправляем на сервер
//         }
//         else
//         {
//             pendingIndex = index; // пока пустышка не создана
//         }
//     }

//     // Переименованный метод регистрации игрока
//     public void RegisterPlayerVisual(PlayerVisual visual)
//     {
//         if (!visual.IsOwner) return;

//         playerVisual = visual;

//         if (pendingIndex >= 0)
//         {
//             playerVisual.SetVisualServerRpc(pendingIndex);
//             pendingIndex = -1;
//         }
//     }
// }
using UnityEngine;
using Unity.Netcode;

public class PlayerVisualManagerNetcode : MonoBehaviour
{
    public static PlayerVisualManagerNetcode Instance { get; private set; }

    private PlayerVisual localPlayerVisual;

    // ←←←← ВАЖНО: это событие
    public event System.Action OnLocalPlayerVisualRegistered;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPlayerVisual(PlayerVisual visual)
    {
        if (visual.IsOwner)
        {
            localPlayerVisual = visual;
            Debug.Log("Локальный PlayerVisual зарегистрирован");
            OnLocalPlayerVisualRegistered?.Invoke();   // ← срабатывает → LoadingScreen исчезает
        }
    }

    public void ChooseVisual(int index)
    {
        localPlayerVisual?.RequestSetVisual(index);
    }

    public bool HasLocalPlayerVisual()
{
    return localPlayerVisual != null;
}

}