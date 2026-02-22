using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class PlayerNameDisplay : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text _NameText;

    private Transform _cam;

    private NetworkVariable<FixedString128Bytes> _playerName =
        new NetworkVariable<FixedString128Bytes>(
            "Player",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _cam = Camera.main != null ? Camera.main.transform : null;

        _playerName.OnValueChanged += OnNameChanged;

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
        OnNameChanged(default, _playerName.Value);
    }

    private void LateUpdate()
    {
        if (_cam == null)
        {
            if (Camera.main != null)
                _cam = Camera.main.transform;
            else
                return;
        }

        // Поворачиваем только объект с текстом
        transform.LookAt(transform.position + _cam.forward);
    }

    private void OnNameChanged(FixedString128Bytes oldName, FixedString128Bytes newName)
    {
        if (_NameText != null)
            _NameText.text = newName.ToString();
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

        _playerName.Value = name;
    }
}