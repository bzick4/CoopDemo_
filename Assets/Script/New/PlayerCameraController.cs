// using Unity.Netcode;
// using UnityEngine;
// using Unity.Cinemachine;

// public class PlayerCameraController : NetworkBehaviour
// {
//     [Header("Camera")]
//     [SerializeField] private GameObject _CameraPrefab;
//     [SerializeField] private Transform _CameraTarget;
//     private CinemachineCamera _camInstance;

//     public override void OnNetworkSpawn()
//     {
        
//         if (!IsLocalPlayer)
//             return;

//         // создаём локальную камеру
//         GameObject camGO = Instantiate(_CameraPrefab);
//         _camInstance = camGO.GetComponent<CinemachineCamera>();

//         _camInstance.Follow = _CameraTarget;
//         _camInstance.LookAt = _CameraTarget;
//         _camInstance.Priority = 100;

//         Debug.Log($"[CAMERA] Created for client {OwnerClientId}");
//     }

//     public override void OnNetworkDespawn()
//     {
//         if (!IsLocalPlayer)
//             return;

//         if (_camInstance != null)
//             Destroy(_camInstance.gameObject);
//     }

//     private void RotateToCamera()
//     {
//         Vector3 camForward = _camInstance.transform.forward;
//         camForward.y = 0f;

//         if (camForward.sqrMagnitude < 0.001f)
//             return;

//         transform.forward = camForward;
//     }
// }
using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine; // ← правильно: Cinemachine, а не Unity.Cinemachine

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private GameObject _cameraPrefab;
    [SerializeField] private Transform _cameraTarget;

    [Header("TypeCamera")]
    private CinemachineCamera _TPS;
    private CinemachineCamera _RTS;

    [Header("Rotation")]
    [SerializeField] private bool rotateCharacterToCamera = true;
    [SerializeField] private float rotationSpeed = 720f;

    public CinemachineCamera CamInstance { get; private set; }  // публичное свойство для доступа из других классов (например, CharController);

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
            return;

        GameObject camGO = Instantiate(_cameraPrefab);
        CamInstance = camGO.GetComponent<CinemachineCamera>();
       CamInstance.Priority = 50;

        if (CamInstance == null)
        {
            Debug.LogError("[PlayerCameraController] В префабе камеры нет CinemachineVirtualCamera!");
            return;
        }

        CamInstance.Follow = _cameraTarget;
        CamInstance.LookAt = _cameraTarget;
        CamInstance.Priority= 50;

        Debug.Log($"[CAMERA] Локальная камера создана для игрока {OwnerClientId}");
    }

    public override void OnNetworkDespawn()
    {
        if (!IsLocalPlayer)
            return;

        if (CamInstance != null && CamInstance.gameObject != null)
        {
            Destroy(CamInstance.gameObject);
            CamInstance = null;
        }
    }

    private void LateUpdate()
    {
        if (!IsLocalPlayer || CamInstance == null || !rotateCharacterToCamera)
            return;

        //RotateToCamera();
    }

    public void RotateToCamera()
    {
        // Берём направление "вперёд" от камеры
        Vector3 camForward = CamInstance.transform.forward;
        camForward.y = 0f; // убираем вертикальную компоненту

        // Если вектор слишком маленький (камера смотрит почти вертикально вниз/вверх)
        if (camForward.sqrMagnitude < 0.001f)
            return;

        camForward.Normalize();

        // Плавный поворот (Quaternion.Slerp или RotateTowards)
        Quaternion targetRotation = Quaternion.LookRotation(camForward);

        // Можно просто мгновенно:
        // transform.rotation = targetRotation;

        // Или плавно (рекомендую):
        float maxDegreesDelta = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            maxDegreesDelta
        );
    }

    // Для дебага (опционально)
    private void OnDrawGizmosSelected()
    {
        if (CamInstance != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, CamInstance.transform.forward * 5f);
        }
    }
}