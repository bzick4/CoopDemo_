using UnityEngine;
using Unity.Netcode;

public class PlayerVisual : NetworkBehaviour
{
    [SerializeField] private GameObject[] visuals;
    [SerializeField] private Transform visualRoot;

    private NetworkVariable<int> visualIndex =
        new NetworkVariable<int>(
            -1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
{
    visualIndex.OnValueChanged += OnVisualChanged;

    if (IsServer && visualIndex.Value == -1)
    {
        visualIndex.Value = Random.Range(0, visuals.Length);
    }

    if (visualIndex.Value != -1)
    {
        ApplyVisual(visualIndex.Value);
    }
}

    private void OnVisualChanged(int oldValue, int newValue)
    {
        ApplyVisual(newValue);
    }

    private void ApplyVisual(int index)
    {
        foreach (Transform child in visualRoot)
            Destroy(child.gameObject);

        Instantiate(visuals[index], visualRoot);
    }
}