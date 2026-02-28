using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCarController : MonoBehaviour
{
    private CarInputActions _InputActions;
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float steerSpeed = 8f;
   
    private WheelCollider[] wheelColliders = new WheelCollider[4];
    private Transform[] wheelMeshes = new Transform[4];

    private Rigidbody rb;
    private bool initialized = false;

    // Ввод
    private Vector2 moveInput;
    private bool brakeInput;

    void Start()
    {
        _InputActions = new CarInputActions();
        _InputActions.Enable();

        rb = GetComponentInChildren<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Нет Rigidbody на машине!");
            enabled = false;
            return;
        }

    
        if (_InputActions == null)
        {
            Debug.LogError("CarInputActions не назначен в инспекторе!");
            enabled = false;
            return;
        }

        // Включаем ввод
        _InputActions.Enable();

        // // Подписываемся на события
        _InputActions.Car.Move.performed += OnMove;
        _InputActions.Car.Move.canceled += OnMoveCanceled;

        _InputActions.Car.Brake.performed += OnBrake;
        _InputActions.Car.Brake.canceled += OnBrakeCanceled;

        Debug.Log("Ввод активирован через CarInputActions");
    }

    void OnDestroy()
    {
        if (_InputActions != null)
        {
            _InputActions.Disable();
            _InputActions.Dispose();
        }
    }

    // Методы ввода
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnBrake(InputAction.CallbackContext context)
    {
        brakeInput = true;
    }

    private void OnBrakeCanceled(InputAction.CallbackContext context)
    {
        brakeInput = false;
    }

    public void Initialize()
    {
        wheelColliders = GetComponentsInChildren<WheelCollider>();

        Debug.Log($"[SimpleCarController] Найдено WheelCollider: {wheelColliders.Length}");

        if (wheelColliders.Length < 4)
        {
            Debug.LogError($"Найдено меньше 4 колёс ({wheelColliders.Length}) — машина не поедет!");
            initialized = false;
            return;
        }

        // Сортируем по имени
        System.Array.Sort(wheelColliders, (a, b) => string.Compare(a.name, b.name));

        // Выводим для проверки
        for (int i = 0; i < Mathf.Min(4, wheelColliders.Length); i++)
        {
            Debug.Log($"Колесо {i}: {wheelColliders[i].name} (родитель: {wheelColliders[i].transform.parent?.name})");
        }

        // Обрезаем до 4, если больше
        if (wheelColliders.Length > 4)
        {
            WheelCollider[] trimmed = new WheelCollider[4];
            System.Array.Copy(wheelColliders, trimmed, 4);
            wheelColliders = trimmed;
            Debug.Log("Обрезано до 4 колёс (лишние игнорируются)");
        }

        // Поиск визуальных мешей (адаптируй под свои имена)
        for (int i = 0; i < 4; i++)
        {
            string colliderName = wheelColliders[i].name;
            wheelMeshes[i] = wheelColliders[i].transform.Find("Mesh") ?? 
                             wheelColliders[i].transform.parent.Find(colliderName.Replace("Collider", "Mesh"));

            if (wheelMeshes[i] == null)
            {
                Debug.LogWarning($"Визуал колеса {colliderName} не найден");
            }
        }

        initialized = true;
        Debug.Log("[SimpleCarController] Инициализация завершена успешно");
    }

    private void FixedUpdate()
    {
        if (!initialized || wheelColliders == null || wheelColliders.Length < 4) return;

        if (wheelColliders[0] == null) return;

        float currentMotor = moveInput.y * motorForce;
        float currentBrake = brakeInput ? brakeForce : 0f;

        float currentSteer = moveInput.x * maxSteerAngle;
        currentSteer = Mathf.Lerp(0f, currentSteer, steerSpeed * Time.fixedDeltaTime);

        wheelColliders[0].steerAngle = currentSteer;
        wheelColliders[1].steerAngle = currentSteer;

        wheelColliders[2].motorTorque = currentMotor;
        wheelColliders[3].motorTorque = currentMotor;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].brakeTorque = currentBrake;
        }

        //AnimateWheels();

        // Отладка ввода (каждые ~0.5 сек)
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"Ввод: motor = {moveInput.y:F2}, steer = {moveInput.x:F2}, brake = {brakeInput}");
        }
    }

    private void AnimateWheels()
    {
        for (int i = 0; i < 4; i++)
        {
            if (wheelMeshes[i] == null) continue;

            Vector3 pos;
            Quaternion rot;
            wheelColliders[i].GetWorldPose(out pos, out rot);

            wheelMeshes[i].position = pos;
            wheelMeshes[i].rotation = rot;
        }
    }
}