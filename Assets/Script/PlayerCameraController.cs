using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private GameObject _CameraPrefab;
    [SerializeField] private Transform _CameraTarget;
    private CinemachineCamera _camInstance;

    public override void OnNetworkSpawn()
    {
        
        if (!IsLocalPlayer)
            return;

        // создаём локальную камеру
        GameObject camGO = Instantiate(_CameraPrefab);
        _camInstance = camGO.GetComponent<CinemachineCamera>();

        _camInstance.Follow = _CameraTarget;
        _camInstance.LookAt = _CameraTarget;
        _camInstance.Priority = 100;

        Debug.Log($"[CAMERA] Created for client {OwnerClientId}");
    }

    public override void OnNetworkDespawn()
    {
        if (!IsLocalPlayer)
            return;

        if (_camInstance != null)
            Destroy(_camInstance.gameObject);
    }

    private void RotateToCamera()
    {
        Vector3 camForward = _camInstance.transform.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude < 0.001f)
            return;

        transform.forward = camForward;
    }
}