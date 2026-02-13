
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class CharController : NetworkBehaviour
{
    [Header("Скорости движения")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Камера (ссылка)")]
    private PlayerCameraController cameraController;

    private bool _isRunning;
    private Vector3 _moveInput;
    private Animator _animator;

    // NetworkVariable для позиции и Blend
    private NetworkVariable<Vector3> netPosition = new NetworkVariable<Vector3>(
        default(Vector3),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private NetworkVariable<float> netBlend = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Подписка на изменения blend для клиентов
        netBlend.OnValueChanged += (oldVal, newVal) =>
        {
            if (_animator != null)
                _animator.SetFloat("Blend", newVal, 0.1f, Time.deltaTime);
        };

        // Если ссылка на камеру не назначена в инспекторе — попробуем найти автоматически
        if (cameraController == null)
        {
            cameraController = GetComponent<PlayerCameraController>();
            if (cameraController == null)
                Debug.LogWarning("PlayerCameraController не найден на этом объекте!");
        }
    }

    private void OnEnable()
    {
        PlayerVisual visual = GetComponent<PlayerVisual>();
        if (visual != null)
            visual.OnVisualReady += InitAnimator;
    }

    private void InitAnimator(Animator animator)
    {
        _animator = animator;

        if (_animator == null)
        {
            Debug.LogError("Animator not found on root!");
            return;
        }

        var controller = _animator.runtimeAnimatorController;
        _animator.runtimeAnimatorController = null;
        _animator.runtimeAnimatorController = controller;
        _animator.Rebind();
        _animator.Update(0f);

        _animator.SetFloat("Blend", netBlend.Value, 0.1f, Time.deltaTime);
        Debug.Log("Animator initialized on root after visual spawn");

        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.SetAnimator(_animator);
        }
    }

    // private void Update()
    // {
    //     if (!IsOwner) return;

    //     // Ввод
    //     _isRunning = Input.GetKey(KeyCode.LeftShift);
    //     float h = Input.GetAxisRaw("Horizontal");
    //     float v = Input.GetAxisRaw("Vertical");
    //     _moveInput = new Vector3(h, 0f, v).normalized;  // normalized — диагональ не быстрее

    //     if (_moveInput.sqrMagnitude < 0.01f)
    //     {
    //         // Нет ввода
    //         netBlend.Value = 0f;
    //         return;
    //     }

    //     // Получаем направление камеры
    //     Vector3 camForward = Vector3.zero;
    //     if (cameraController != null && cameraController._camInstance != null)
    //     {
    //         camForward = cameraController._camInstance.transform.forward;
    //         camForward.y = 0f;
    //         camForward.Normalize();
    //     }
    //     else
    //     {
    //         // fallback — если камера ещё не готова
    //         camForward = transform.forward;
    //     }

    //     // Перпендикулярное направление (вправо)
    //     Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;

    //     // Желаемое направление движения относительно камеры
    //     Vector3 desiredDirection = (camForward * _moveInput.z) + (camRight * _moveInput.x);

    //     // Скорость
    //     float currentSpeed = _isRunning ? runSpeed : walkSpeed;

    //     // Движение
    //     Vector3 moveDelta = desiredDirection * currentSpeed * Time.deltaTime;
    //     transform.position += moveDelta;

    //     // Синхронизация позиции по сети
    //     netPosition.Value = transform.position;

    //     // Blend для анимации
    //     float blend = _moveInput.magnitude > 0.01f ? (_isRunning ? 2f : 1f) : 0f;
    //     netBlend.Value = blend;

    //     if (_animator != null)
    //         _animator.SetFloat("Blend", blend, 0.1f, Time.deltaTime);
    // }
    private void Update()
{
    if (!IsOwner) return;

    // Ввод
    _isRunning = Input.GetKey(KeyCode.LeftShift);
    float h = Input.GetAxisRaw("Horizontal");
    float v = Input.GetAxisRaw("Vertical");
    _moveInput = new Vector3(h, 0f, v).normalized;

    float targetBlend;

    if (_moveInput.sqrMagnitude < 0.01f)
    {
        targetBlend = 0f;
    }
    else
    {
        targetBlend = _isRunning ? 2f : 1f;
    }

    // Плавно интерполируем текущее значение к целевому
    float currentBlend = netBlend.Value;
    currentBlend = Mathf.Lerp(currentBlend, targetBlend, 10f * Time.deltaTime);  // 10 — скорость затухания

    netBlend.Value = currentBlend;

    // Локально тоже плавно
    if (_animator != null)
    {
        _animator.SetFloat("Blend", currentBlend, 0.1f, Time.deltaTime);
    }

    // Движение (твой текущий код с камерой)
    if (_moveInput.sqrMagnitude >= 0.01f)
    {
        Vector3 camForward = GetCameraForward();
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;
        Vector3 desiredDirection = (camForward * _moveInput.z) + (camRight * _moveInput.x);

        float currentSpeed = _isRunning ? runSpeed : walkSpeed;
        Vector3 moveDelta = desiredDirection * currentSpeed * Time.deltaTime;
        transform.position += moveDelta;

        netPosition.Value = transform.position;
    }
}

// Вспомогательный метод (чтобы не дублировать код)
private Vector3 GetCameraForward()
{
    if (cameraController != null && cameraController.CamInstance != null)
    {
        return cameraController.CamInstance.transform.forward;
    }
    return transform.forward; // fallback
}

    private void LateUpdate()
    {
        if (!IsOwner)
        {
            // Для чужих игроков — просто применяем сетевую позицию
            transform.position = netPosition.Value;
        }
    }

    public void InitStats(SOData data)
    {
        walkSpeed = data.WalkSpeed;
        runSpeed = data.RunSpeed;
    }
}