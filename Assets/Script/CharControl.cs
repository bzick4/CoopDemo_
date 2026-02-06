// using UnityEngine;
// using Unity.Cinemachine;
// using Unity.Netcode;
// using Unity.VisualScripting;


// public class CharControl : NetworkBehaviour
// {
//     [Header("References")]
//     [SerializeField] private InputReader _InputReader;
//     private float _Walk => _SOData.WalkSpeed;
//     private float _Run => _SOData.RunSpeed;
//     [SerializeField] private Transform _CameraTransform;
//     //private SOData _SOData => GetComponent<PlayerVisual>().GetComponentInChildren<PlayerVisual>().visuals[0].GetComponent<CharacterEntry>().SODatao;

//     [Header("Private")]
//     private bool _isRunning;
//     private Vector2 moveInput;
//     private Animator _animator => GetComponentInChildren<Animator>();

//     [Header("Stats")]
//     private float _currentHealth;
//     private float _currentStamina;
//     private float _currentManna;


//    public override void OnNetworkSpawn()
// {
//     if (!IsOwner)
//         return;

//     if (_InputReader == null)
//     {
//         Debug.LogError("InputReader is NOT assigned!");
//         return;
//     }

//     StatsInit();

//     var brain = Camera.main.GetComponent<CinemachineBrain>();
//     _CameraTransform = brain.OutputCamera.transform;
    
//     _InputReader.EnableInput();
//     _InputReader.MoveEvent += OnMove;
//     _InputReader.RunEvent += OnSprint;
// }

// public override void OnNetworkDespawn()
// {
//     if (!IsOwner)
//         return;

//     _InputReader.MoveEvent -= OnMove;
//     _InputReader.RunEvent -= OnSprint;
//     _InputReader.DisableInput();
// }

// private void StatsInit()
//     {
//         _currentHealth = _SOData.MaxHealth;
//         _currentStamina = _SOData.MaxStamina;
//         _currentManna = _SOData.MaxManna;
//     }


//     // private void OnDestroy()
//     // {
//     //     if (!IsOwner)
//     //         return;

//     //     inputReader.MoveEvent -= OnMove;
//     // }

//     private void OnMove(Vector2 input)
// {
//     Debug.Log("MOVE INPUT: " + input);
//     moveInput = input;
// }

//     private void Update()
// {
//     if (!IsOwner)
//         return;

//     RotateToCamera();
//     SendMoveToServer();
// }

//     private void SendMoveToServer()
//     {
//         MoveServerRpc(moveInput, _isRunning);
//     }

//     private void RotateToCamera()
// {
//     float rotate  = 200;

//     if (_CameraTransform == null)
//         return;

//     Vector3 camForward = _CameraTransform.forward;
//     camForward.y = 0f;

//     if (camForward.sqrMagnitude < 0.001f)
//         return;

//     Quaternion targetRotation = Quaternion.LookRotation(camForward);
//     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotate * Time.deltaTime);

//     transform.forward = camForward;
// }

// private void OnSprint(bool running)
// {
//     _isRunning = running;
// }


 
//    [ServerRpc]
// private void MoveServerRpc(Vector2 input, bool isRunning)
// {
//     float speed = isRunning ? _Run : _Walk;

//     Vector3 forward = transform.forward;
//     Vector3 right = transform.right;

//     forward.y = 0;
//     right.y = 0;

//     Vector3 direction = forward * input.y + right * input.x;
//     transform.position += direction * speed * Time.deltaTime;

//      if (_animator != null)
//         {
//             float blend = Mathf.Abs(input.x) > 0 || Mathf.Abs(input.y) > 0 ? (isRunning ? 2f : 1f) : 0f;
//             _animator.SetFloat("Blend", blend, 0.2f, Time.deltaTime);
//         }
// }
// }

using UnityEngine;
using Unity.Cinemachine;
using Unity.Netcode;

public class CharControl : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _InputReader;

    [Header("Camera")]
    private Transform _CameraTransform;

    [Header("Private")]
    private Vector2 moveInput;
    private bool _isRunning;
    private Animator _animator => GetComponentInChildren<Animator>();

    // Runtime stats
    private CharacterRuntimeStats _runtimeStats => GetComponent<CharacterRuntimeStats>();
    private SOData _data;

    private float WalkSpeed;
    private float RunSpeed;

    private bool _statsInitialized = false;

    // -------------------------
    // Этот метод вызывается PlayerVisual сразу после выбора SOData
    public void InitStats(SOData data)
    {
        _data = data;
        WalkSpeed = data.WalkSpeed;
        RunSpeed = data.RunSpeed;

        // Инициализируем RuntimeStats, если есть
       
        if (_runtimeStats != null)
        {
            _runtimeStats.Init(_data);
        }

        _statsInitialized = true;
        Debug.Log($"CharControl: stats initialized for {_data.name}");
    }
    

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // Камера
        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null)
            _CameraTransform = brain.OutputCamera.transform;

        // Ввод
        if (_InputReader == null)
        {
            Debug.LogError("InputReader NOT assigned!");
            return;
        }

        _InputReader.EnableInput();
        _InputReader.MoveEvent += OnMove;
        _InputReader.RunEvent += OnSprint;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        _InputReader.MoveEvent -= OnMove;
        _InputReader.RunEvent -= OnSprint;
        _InputReader.DisableInput();
    }

    private void Update()
    {
        if (!IsOwner) return;

        RotateToCamera();
        //SendMoveToServer();

       MoveLocal();
    }

    private void MoveLocal()
{
    float speed = _isRunning ? RunSpeed : WalkSpeed;

    Vector3 forward = transform.forward;
    Vector3 right = transform.right;

    forward.y = 0;
    right.y = 0;

    Vector3 direction = forward * moveInput.y + right * moveInput.x;
    transform.position += direction * speed * Time.deltaTime;

    // анимация ЛОКАЛЬНО
    if (_animator != null)
    {
        float blend =
            Mathf.Abs(moveInput.x) > 0 || Mathf.Abs(moveInput.y) > 0
                ? (_isRunning ? 2f : 1f)
                : 0f;

        _animator.SetFloat("Blend", blend, 0.2f, Time.deltaTime);
    }
}

    private void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    private void OnSprint(bool running)
    {
        _isRunning = running;
    }

    // private void SendMoveToServer()
    // {
    //     MoveServerRpc(moveInput, _isRunning);
    // }

    // Поворот игрока по направлению камеры (GTA-стиль)
    private void RotateToCamera()
    {
        if (_CameraTransform == null) return;

        Vector3 camForward = _CameraTransform.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime);
        //transform.rotation = Quaternion.LookRotation(camForward);
        // if (_CameraTransform == null) return;

    // // Берём направление камеры
    // Vector3 camForward = _CameraTransform.forward;

    // // Сохраняем только горизонтальный поворот для трансформа персонажа
    // Vector3 flatForward = new Vector3(camForward.x, 0f, camForward.z);
    // if (flatForward.sqrMagnitude < 0.001f) return;

    // // Плавный поворот игрока по горизонтали
    // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatForward), 10f * Time.deltaTime);

    // // Для движения используем реальное направление камеры
    // Vector3 moveDir = _CameraTransform.forward * moveInput.y + _CameraTransform.right * moveInput.x;
    // moveDir.y = 0f; // чтобы не летел вверх
    // moveDir.Normalize();

    // // Локальное движение (для визуального отклика)
    // transform.position += moveDir * ( _isRunning ? RunSpeed : WalkSpeed) * Time.deltaTime;

    }

    // [ServerRpc]
    // private void MoveServerRpc(Vector2 input, bool isRunning)
    // {
    //     //if (!_statsInitialized) return;

    //     float speed = isRunning ? RunSpeed : WalkSpeed;

    //     Vector3 forward = transform.forward;
    //     Vector3 right = transform.right;

    //     forward.y = 0;
    //     right.y = 0;

    //     Vector3 direction = forward * input.y + right * input.x;
    //     transform.position += direction * speed * Time.deltaTime;

        
    // }
}