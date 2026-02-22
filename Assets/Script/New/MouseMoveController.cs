using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class MouseMoveController : NetworkBehaviour
{
    public float WalkSpeed;
    public float RunSpeed;
    [SerializeField] private float _gravity = -9.81f;
    private Animator _animator;
    private CharacterController _characterController;
    private PlayerInputHandler _inputHandler;
    private Vector3 _targetPosition;
    private bool _hasTarget = false;
    private float _verticalVelocity = 0f;
    public bool _isMoving { private get; set; } = false;

    private NetworkVariable<Vector3> netPosition = new NetworkVariable<Vector3>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    private NetworkVariable<Quaternion> netRotation = new NetworkVariable<Quaternion>(
        Quaternion.identity,
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

        if (!IsOwner) return; Spawner();

        
        _inputHandler = GetComponent<PlayerInputHandler>();

        netBlend.OnValueChanged += (oldVal, newVal) =>
        {
            if (_animator != null)
                _animator.SetFloat("Blend", newVal, 0.1f, Time.deltaTime);
        };
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _targetPosition = transform.position;
    }

    public void InitStats(SOData data)
    {
        _isMoving = true;
        WalkSpeed = data.WalkSpeed;
        RunSpeed = data.RunSpeed;
    }

    void Update()
    {
        if (!IsOwner)
        {
            // Для не-владельцев — просто применяем сетевые значения
            transform.position = netPosition.Value;
            transform.rotation = netRotation.Value;
            HandleAnimation(netBlend.Value);
            return;
        }

        HandleMouseClick();
        HandleMovement();
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0) && _isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _targetPosition = hit.point;
                _hasTarget = true;
            }
        }
    }

    private void HandleMovement()
    {
       
        // Gravity
        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < 0f)
                _verticalVelocity = -1f;
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }

        float targetBlend = 0f;
        if (_hasTarget)
        {
            Vector3 direction = (_targetPosition - transform.position);
            direction.y = 0f;
            float distance = direction.magnitude;
           bool isRunning = _inputHandler.RunPressed;
            float speed = isRunning ? RunSpeed : WalkSpeed;
            if (distance > 0.1f)
            {
                Vector3 move = direction.normalized * speed;
                move.y = _verticalVelocity;
                _characterController.Move(move * Time.deltaTime);
                HandleRotation(direction);
                   targetBlend = isRunning ? 2f : 1f;
            }
            else
            {
                _hasTarget = false;
                targetBlend = 0f;
            }
        }
        else
        {
            Vector3 move = Vector3.zero;
            move.y = _verticalVelocity;
            _characterController.Move(move * Time.deltaTime);
            targetBlend = 0f;
        }

      
        //float currentBlend = netBlend.Value;
        float currentBlend = Mathf.Lerp(netBlend.Value, targetBlend, 10f * Time.deltaTime);

        // Синхронизация по сети
        netPosition.Value = transform.position;
        netRotation.Value = transform.rotation;
        netBlend.Value = currentBlend;
        //_animator.SetFloat("Blend", currentBlend, 0.1f, Time.deltaTime);
        //HandleAnimation(currentBlend);
    }

    private void HandleRotation(Vector3 direction)
    {
        Vector3 lookDir = direction;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }

    private void HandleAnimation(float blend)
    {
        if (_animator != null)
            _animator.SetFloat("Blend", blend, 0.1f, Time.deltaTime);
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
}

