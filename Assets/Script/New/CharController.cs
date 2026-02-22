
// using UnityEngine;
// using Unity.Netcode;
// using System.Collections;

// public class CharController : NetworkBehaviour
// {
    


//     [Header("Скорости движения")]
//     public float walkSpeed = 5f;
//     public float runSpeed = 10f;

//     [Header("Камера (ссылка)")]
//     private PlayerCameraController cameraController;

//     private bool _isRunning;
//     private Vector3 _moveInput;
//     private Animator _animator;

//     // NetworkVariable для позиции и Blend
//     private NetworkVariable<Vector3> netPosition = new NetworkVariable<Vector3>(
//         default(Vector3),
//         NetworkVariableReadPermission.Everyone,
//         NetworkVariableWritePermission.Owner
//     );

//     private NetworkVariable<float> netBlend = new NetworkVariable<float>(
//         0f,
//         NetworkVariableReadPermission.Everyone,
//         NetworkVariableWritePermission.Owner
//     );

//     public override void OnNetworkSpawn()
//     {
//         base.OnNetworkSpawn();

//         // Подписка на изменения blend для клиентов
//         netBlend.OnValueChanged += (oldVal, newVal) =>
//         {
//             if (_animator != null)
//                 _animator.SetFloat("Blend", newVal, 0.1f, Time.deltaTime);
//         };

//         // Если ссылка на камеру не назначена в инспекторе — попробуем найти автоматически
//         if (cameraController == null)
//         {
//             cameraController = GetComponent<PlayerCameraController>();
//             if (cameraController == null)
//                 Debug.LogWarning("PlayerCameraController не найден на этом объекте!");
//         }
//     }

//     private void OnEnable()
//     {
//         PlayerVisual visual = GetComponent<PlayerVisual>();
//         if (visual != null)
//             visual.OnVisualReady += InitAnimator;
//     }

//     private void InitAnimator(Animator animator)
//     {
//         _animator = animator;

//         if (_animator == null)
//         {
//             Debug.LogError("Animator not found on root!");
//             return;
//         }

//         var controller = _animator.runtimeAnimatorController;
//         _animator.runtimeAnimatorController = null;
//         _animator.runtimeAnimatorController = controller;
//         _animator.Rebind();
//         _animator.Update(0f);

//         _animator.SetFloat("Blend", netBlend.Value, 0.1f, Time.deltaTime);
//         Debug.Log("Animator initialized on root after visual spawn");

//         PlayerAttack attack = GetComponent<PlayerAttack>();
//         if (attack != null)
//         {
//             attack.SetAnimator(_animator);
//         }
//     }

//     // private void Update()
//     // {
//     //     if (!IsOwner) return;

//     //     // Ввод
//     //     _isRunning = Input.GetKey(KeyCode.LeftShift);
//     //     float h = Input.GetAxisRaw("Horizontal");
//     //     float v = Input.GetAxisRaw("Vertical");
//     //     _moveInput = new Vector3(h, 0f, v).normalized;  // normalized — диагональ не быстрее

//     //     if (_moveInput.sqrMagnitude < 0.01f)
//     //     {
//     //         // Нет ввода
//     //         netBlend.Value = 0f;
//     //         return;
//     //     }

//     //     // Получаем направление камеры
//     //     Vector3 camForward = Vector3.zero;
//     //     if (cameraController != null && cameraController._camInstance != null)
//     //     {
//     //         camForward = cameraController._camInstance.transform.forward;
//     //         camForward.y = 0f;
//     //         camForward.Normalize();
//     //     }
//     //     else
//     //     {
//     //         // fallback — если камера ещё не готова
//     //         camForward = transform.forward;
//     //     }

//     //     // Перпендикулярное направление (вправо)
//     //     Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;

//     //     // Желаемое направление движения относительно камеры
//     //     Vector3 desiredDirection = (camForward * _moveInput.z) + (camRight * _moveInput.x);

//     //     // Скорость
//     //     float currentSpeed = _isRunning ? runSpeed : walkSpeed;

//     //     // Движение
//     //     Vector3 moveDelta = desiredDirection * currentSpeed * Time.deltaTime;
//     //     transform.position += moveDelta;

//     //     // Синхронизация позиции по сети
//     //     netPosition.Value = transform.position;

//     //     // Blend для анимации
//     //     float blend = _moveInput.magnitude > 0.01f ? (_isRunning ? 2f : 1f) : 0f;
//     //     netBlend.Value = blend;

//     //     if (_animator != null)
//     //         _animator.SetFloat("Blend", blend, 0.1f, Time.deltaTime);
//     // }
//     private void Update()
// {
//     if (!IsOwner) return;

//     // Ввод
//     _isRunning = Input.GetKey(KeyCode.LeftShift);
//     float h = Input.GetAxisRaw("Horizontal");
//     float v = Input.GetAxisRaw("Vertical");
//     _moveInput = new Vector3(h, 0f, v).normalized;

//     float targetBlend;

//     if (_moveInput.sqrMagnitude < 0.01f)
//     {
//         targetBlend = 0f;
//     }
//     else
//     {
//         targetBlend = _isRunning ? 2f : 1f;
//     }

//     // Плавно интерполируем текущее значение к целевому
//     float currentBlend = netBlend.Value;
//     currentBlend = Mathf.Lerp(currentBlend, targetBlend, 10f * Time.deltaTime);  // 10 — скорость затухания

//     netBlend.Value = currentBlend;

//     // Локально тоже плавно
//     if (_animator != null)
//     {
//         _animator.SetFloat("Blend", currentBlend, 0.1f, Time.deltaTime);
//     }

    
//     if (_moveInput.sqrMagnitude >= 0.01f)
//     {
//         Vector3 camForward = GetCameraForward();
//         camForward.y = 0f;
//         camForward.Normalize();

//         Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;
//         Vector3 desiredDirection = (camForward * _moveInput.z) + (camRight * _moveInput.x);

//         float currentSpeed = _isRunning ? runSpeed : walkSpeed;
//         Vector3 moveDelta = desiredDirection * currentSpeed * Time.deltaTime;
//         transform.position += moveDelta;

//         netPosition.Value = transform.position;
//     }
// }

// // Вспомогательный метод (чтобы не дублировать код)
// private Vector3 GetCameraForward()
// {
//     if (cameraController != null && cameraController.CamInstance != null)
//     {
//         return cameraController.CamInstance.transform.forward;
//     }
//     return transform.forward; // fallback
// }

//     private void LateUpdate()
//     {
//         if (!IsOwner)
//         {
//             // Для чужих игроков — просто применяем сетевую позицию
//             transform.position = netPosition.Value;
//         }
//     }

//     public void InitStats(SOData data)
//     {
//         walkSpeed = data.WalkSpeed;
//         runSpeed = data.RunSpeed;
//     }
// }

using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class CharController : NetworkBehaviour
{
    [Header("Скорости движения")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float gravity = -9.81f;
    private int jumpLayerIndex;

    private CharacterController _characterController;
    private PlayerCameraController cameraController;
    private Animator _animator;
    private PlayerInputHandler _inputHandler;

    // Вертикальная скорость
    private float _verticalVelocity;

    // Netcode
    private NetworkVariable<Vector3> netPosition = new NetworkVariable<Vector3>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private NetworkVariable<float> netBlend = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private void Start()
    {
        _verticalVelocity = 0f; // обязательно обнуляем при старте
        enabled = false; // отключаем скрипт до прогрузки визуала
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponents();

        if (IsOwner)
            {
                netPosition.Value = transform.position;
                Spawner();
            }

        if (_inputHandler == null)
            Debug.LogError("PlayerInputHandler не найден!");

        if (cameraController == null)
            Debug.LogWarning("PlayerCameraController не найден!");

        netBlend.OnValueChanged += (oldVal, newVal) =>
        {
            if (_animator != null)
                _animator.SetFloat("Blend", newVal, 0.1f, Time.deltaTime);
        };
    }

    private void GetComponents()
    {
         _characterController = GetComponent<CharacterController>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        cameraController = GetComponent<PlayerCameraController>(); 
    }

    private void Spawner()
    {
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();

        if (spawnManager != null)
        {
        Vector3 spawnPos = spawnManager.GetRandomSpawnPoint();

        CharacterController controller = GetComponent<CharacterController>();

        controller.enabled = false;
        transform.position = spawnPos;
        controller.enabled = true;
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
        jumpLayerIndex = _animator.GetLayerIndex("Jump");
        if (_animator == null)
        {
            Debug.LogError("Animator не найден!");
            return;
        }
        var controller = _animator.runtimeAnimatorController;
        _animator.runtimeAnimatorController = null;
        _animator.runtimeAnimatorController = controller;
        _animator.Rebind();
        _animator.Update(0f);
        _animator.SetFloat("Blend", netBlend.Value, 0.1f, Time.deltaTime);
        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
            attack.SetAnimator(_animator);
        enabled = true; // активируем скрипт ходьбы после прогрузки визуала
    }

    private void Jump()
    {
        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < 0f)
                _verticalVelocity = -1f;

            if (_animator != null)
                _animator.SetBool("isGrounded", true);

            if (_inputHandler.JumpPressed)
            {
                _verticalVelocity = jumpForce;

                if (_animator != null)
                    _animator.SetTrigger("Jump");

                _inputHandler.ConsumeJump();
            }
        }
        else
        {
            if (_animator != null)
                _animator.SetBool("isGrounded", false);
        }
    }

    private void HorizontalMovement()
    {
        Vector3 moveInput = new Vector3(_inputHandler.MoveInput.x, 0f, _inputHandler.MoveInput.y).normalized;
    bool isRunning = _inputHandler.RunPressed;
    Vector3 move = Vector3.zero;

    if (moveInput.sqrMagnitude > 0.01f)
    {
        Vector3 camForward = GetCameraForward();
        camForward.y = 0f;
        camForward.Normalize();
        Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;

        Vector3 desiredDir = camForward * moveInput.z + camRight * moveInput.x;
        float speed = isRunning ? runSpeed : walkSpeed;
        move = desiredDir * speed;
    }

    
    if (_characterController.isGrounded)
    {
        if (_verticalVelocity < 0f)
            _verticalVelocity = -1f; // лёгкое прижатие к земле

        if (_inputHandler.JumpPressed)
        {
            _verticalVelocity = jumpForce;
            _inputHandler.ConsumeJump();
        }
    }

    // ===== Gravity =====
    _verticalVelocity += gravity * Time.deltaTime;

    // ===== Apply Movement =====
    Vector3 finalMove = move + Vector3.up * _verticalVelocity;
    _characterController.Move(finalMove * Time.deltaTime);

    // ===== Blend Animation =====
    float targetBlend = moveInput.sqrMagnitude < 0.01f ? 0f : (isRunning ? 2f : 1f);
    float currentBlend = Mathf.Lerp(netBlend.Value, targetBlend, 10f * Time.deltaTime);
    netBlend.Value = currentBlend;
    if (_animator != null)
        _animator.SetFloat("Blend", currentBlend, 0.1f, Time.deltaTime);

    // ===== Netcode Sync =====
    netPosition.Value = transform.position;
    }
  

    private void Update()
{
    if (!IsOwner || _inputHandler == null || _characterController == null)
        return;

        HorizontalMovement();  
        Debug.Log(_inputHandler.MoveInput);
}

   private void LateUpdate()
{
    if (!IsOwner)
    {
        transform.position = netPosition.Value;
    }
}

    private Vector3 GetCameraForward()
    {
        if (cameraController != null && cameraController.CamInstance != null)
            return cameraController.CamInstance.transform.forward;

        return transform.forward;
    }

    public void InitStats(SOData data)
    {
        walkSpeed = data.WalkSpeed;
        runSpeed = data.RunSpeed;
    }
}