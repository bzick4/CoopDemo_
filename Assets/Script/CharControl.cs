using UnityEngine;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CharControl : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _InputReader;
   // [SerializeField] private NetworkAnimator _NetworkAnimator;

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

    private Vector3 _serverMoveDir;
    private float _serverSpeed;
    private float _lastSentY;


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

       // AssignDeepAnimator();
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
        
            SendMoveToServer();
        
        UpdateAnimation();
    }

    private void FixedUpdate()
{
    if (!IsServer) return;

    if (_serverMoveDir.sqrMagnitude < 0.001f) return;

    transform.position += _serverMoveDir * _serverSpeed * Time.fixedDeltaTime;
}

    private void UpdateAnimation()
{
    if (_animator != null)
    {
        float blend =
            Mathf.Abs(moveInput.x) > 0 || Mathf.Abs(moveInput.y) > 0
                ? (_isRunning ? 2f : 1f)
                : 0f;

        _animator.SetFloat("Blend", blend, 0.2f, Time.deltaTime);
    }
   
    // if (_animator != null)
    // {
    //     float blend =
    //         Mathf.Abs(moveInput.x) > 0 || Mathf.Abs(moveInput.y) > 0
    //             ? (_isRunning ? 2f : 1f)
    //             : 0f;

    //     _animator.SetFloat("Blend", blend, 0.2f, Time.deltaTime);
    // }
}

    private void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    private void OnSprint(bool running)
    {
        _isRunning = running;
    }



private void SendMoveToServer()
{
    Vector3 moveDir =
        transform.forward * moveInput.y +
        transform.right * moveInput.x;

    MoveServerRpc(moveDir, _isRunning);

    float currentY = _CameraTransform.eulerAngles.y;

    if (Mathf.Abs(currentY - _lastSentY) > 0.5f)
    {
        _lastSentY = currentY;
        RotateToCameraServerRpc(currentY);
    }
}


    [ServerRpc]
    private void MoveServerRpc(Vector3 moveDir, bool isRunning)
    {
        // moveDir.y = 0f;

        // if (moveDir.sqrMagnitude > 1f)
        //     moveDir.Normalize();

        // float speed = isRunning ? RunSpeed : WalkSpeed;

        // transform.position += moveDir * speed * Time.deltaTime;

    moveDir.y = 0f;

    if (moveDir.sqrMagnitude > 1f)
        moveDir.Normalize();

    float speed = isRunning ? RunSpeed : WalkSpeed;

    _serverMoveDir = moveDir;
    _serverSpeed = speed;

    }

    [ServerRpc]
    private void RotateToCameraServerRpc(float cameraY)
    {
        cameraY = Mathf.Repeat(cameraY, 360f);

        transform.rotation = Quaternion.Euler(0f, cameraY, 0f);
    }

}