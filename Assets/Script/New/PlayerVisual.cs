// using UnityEngine;
// using Unity.Netcode;
// using System;
// using System.Collections;

// public class PlayerVisual : NetworkBehaviour
// {
//     [Header("Визуалы")]
//     public SOData[] visualOptions;
//     private GameObject currentVisual;

//     private NetworkVariable<int> visualIndex = new NetworkVariable<int>(
//         -1,
//         NetworkVariableReadPermission.Everyone,
//         NetworkVariableWritePermission.Owner
//     );

//     public event Action<Animator> OnVisualReady;

//    public override void OnNetworkSpawn()
// {
//     base.OnNetworkSpawn();
//     Debug.LogWarning($"!!! OnNetworkSpawn CALLED on {gameObject.name} | IsSpawned: {IsSpawned} | IsOwner: {IsOwner} | OwnerClientId: {OwnerClientId} | LocalClientId: {NetworkManager.Singleton.LocalClientId} | IsHost: {IsHost} | IsClient: {IsClient}");

//     visualIndex.OnValueChanged += OnVisualChanged;

//     if (IsOwner)
//     {
//         var manager = PlayerVisualManagerNetcode.Instance;
//         if (manager == null)
//             Debug.LogError("Manager INSTANCE NULL!");
//         else
//         {
//             manager.RegisterPlayerVisual(this);
//             Debug.Log("!!! Registration SUCCESS");
//         }
//     }
//     else
//     {
//         Debug.Log("Not owner, skipping registration");
//     }
// }

//     // Кнопки вызывают этот метод
//     public void RequestSetVisual(int index)
//     {
//         if (!IsOwner) return;
//         SetVisualServerRpc(index);
//     }

//     [ServerRpc]
//     public void SetVisualServerRpc(int index)
//     {
//         if (index < 0 || index >= visualOptions.Length) return;
//         visualIndex.Value = index;
//     }

//     private void OnVisualChanged(int oldVal, int newVal)
//     {
//         StartCoroutine(DelayedVisualSpawn(newVal));
//     }

//     private IEnumerator DelayedVisualSpawn(int index)
// {
//     float timeout = 5f;
//     float timer = 0f;
//     while (!IsSpawned || !gameObject.activeInHierarchy)
//     {
//         yield return null;
//         timer += Time.deltaTime;
//         if (timer > timeout)
//         {
//             Debug.LogError("Timeout waiting for object to be ready for visual spawn");
//             yield break;
//         }
//     }

//     // Ждём ещё один кадр
//     yield return null;

//     Debug.Log($"Spawning visual index {index} on { (IsOwner ? "owner" : "client") }");
//     ApplyVisual(index);
// }

// private void ApplyVisual(int index)
// {
//     if (currentVisual != null)
//         Destroy(currentVisual);

//     if (index < 0 || index >= visualOptions.Length) return;

//     GameObject prefab = visualOptions[index].VisualPrefab;
//     if (prefab == null) return;

//     currentVisual = Instantiate(prefab, transform);
//     currentVisual.transform.localPosition = Vector3.zero;
//     currentVisual.transform.localRotation = Quaternion.identity;
//     currentVisual.transform.localScale = Vector3.one;

//     // Больше НЕ ищем Animator здесь — он на root (this.gameObject)
//     // Но можно принудительно "переподключить" модель к Animator (иногда нужно)
//     Animator animator = GetComponent<Animator>();  // на пустышке
//     if (animator != null && animator.avatar == null)
//     {
//         // Если Avatar потерялся — можно попробовать взять из модели, но обычно не нужно
//         Debug.LogWarning("Animator on root has no Avatar!");
//     }

//     // Вызываем событие (CharController получит Animator с root)
//     OnVisualReady?.Invoke(animator);

//     // Опционально: форсируем ребинд на root после смены меша
//     if (animator != null)
//     {
//         animator.Rebind();
//         animator.Update(0f);
//     }

//     // Init stats, если нужно
//     GetComponent<CharController>()?.InitStats(visualOptions[index]);
// }
// }

using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;

public class PlayerVisual : NetworkBehaviour
{
    [Header("Визуалы")]
    public SOData[] visualOptions;
    private GameObject currentVisual;

    // private NetworkVariable<int> visualIndex = new NetworkVariable<int>(
    //     -1,
    //     NetworkVariableReadPermission.Everyone,
    //     NetworkVariableWritePermission.Owner
    // );
    private NetworkVariable<int> visualIndex = new NetworkVariable<int>(
    -1,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server  // ← меняем на Server!
);

    public event Action<Animator> OnVisualReady;

//    public override void OnNetworkSpawn()
// {
//     base.OnNetworkSpawn();
//     Debug.LogWarning($"!!! OnNetworkSpawn CALLED on {gameObject.name} | IsSpawned: {IsSpawned} | IsOwner: {IsOwner} | OwnerClientId: {OwnerClientId} | LocalClientId: {NetworkManager.Singleton.LocalClientId} | IsHost: {IsHost} | IsClient: {IsClient}");

//     visualIndex.OnValueChanged += OnVisualChanged;

//     if (IsOwner)
//     {
//         var manager = PlayerVisualManagerNetcode.Instance;
//         if (manager == null)
//             Debug.LogError("Manager INSTANCE NULL!");
//         else
//         {
//             manager.RegisterPlayerVisual(this);
//             Debug.Log("!!! Registration SUCCESS");
//         }
//     }
//     else
//     {
//         Debug.Log("Not owner, skipping registration");
//     }
// }
public override void OnNetworkSpawn()
{
    base.OnNetworkSpawn();

    Debug.Log($"OnNetworkSpawn | visualIndex = {visualIndex.Value}");

    visualIndex.OnValueChanged += OnVisualChanged;

    if (IsOwner)
        PlayerVisualManagerNetcode.Instance?.RegisterPlayerVisual(this);

    // ← Ключевой фикс: применяем текущий визуал сразу при спавне
    if (visualIndex.Value >= 0)
    {
        Debug.Log($"Initial apply visual {visualIndex.Value} on spawn");
        ApplyVisual(visualIndex.Value);
    }
}

    // Кнопки вызывают этот метод
    public void RequestSetVisual(int index)
    {
        if (!IsOwner) return;
        SetVisualServerRpc(index);
    }

    [ServerRpc]
    public void SetVisualServerRpc(int index)
    {
        if (index < 0 || index >= visualOptions.Length) return;
        
        visualIndex.Value = index;
    }

    private void OnVisualChanged(int oldVal, int newVal)
    {
        StartCoroutine(DelayedVisualSpawn(newVal));
    }

    private IEnumerator DelayedVisualSpawn(int index)
{
    float timeout = 5f;
    float timer = 0f;
    while (!IsSpawned || !gameObject.activeInHierarchy)
    {
        yield return null;
        timer += Time.deltaTime;
        if (timer > timeout)
        {
            Debug.LogError("Timeout waiting for object to be ready for visual spawn");
            yield break;
        }
    }

    // Ждём ещё один кадр
    yield return null;

    Debug.Log($"Spawning visual index {index} on { (IsOwner ? "owner" : "client") }");
    ApplyVisual(index);
}

private void ApplyVisual(int index)
{
    Debug.Log($"ApplyVisual вызван на { (IsOwner ? "owner" : "remote") } | index = {index} | prefab = {visualOptions[index]?.name}");
    if (currentVisual != null)
        Destroy(currentVisual);

    if (index < 0 || index >= visualOptions.Length) return;

    GameObject prefab = visualOptions[index].VisualPrefab;
    if (prefab == null) return;

    currentVisual = Instantiate(prefab, transform);
    currentVisual.transform.localPosition = Vector3.zero;
    currentVisual.transform.localRotation = Quaternion.identity;
    currentVisual.transform.localScale = Vector3.one;

    // Больше НЕ ищем Animator здесь — он на root (this.gameObject)
    // Но можно принудительно "переподключить" модель к Animator (иногда нужно)
    Animator animator = GetComponent<Animator>();  // на пустышке
    if (animator != null && animator.avatar == null)
    {
        // Если Avatar потерялся — можно попробовать взять из модели, но обычно не нужно
        Debug.LogWarning("Animator on root has no Avatar!");
    }

    // Вызываем событие (CharController получит Animator с root)
    OnVisualReady?.Invoke(animator);

    // Опционально: форсируем ребинд на root после смены меша
    if (animator != null)
    {
        animator.Rebind();
        animator.Update(0f);
    }

    // Init stats, если нужно
    GetComponent<CharController>()?.InitStats(visualOptions[index]);
}
}