// using UnityEngine;
// using UnityEngine.UI;
// using Unity.Cinemachine;

// public class CarSpawner : MonoBehaviour
// {
//     [SerializeField] private Transform spawnPoint;
//     [SerializeField] private GameObject carBasePrefab;
//     [SerializeField] private SOData[] carDatas;

//     [Header("Кнопки выбора машины")]
//     [SerializeField] private Button nextCarButton;
//     [SerializeField] private Button prevCarButton;
//     [SerializeField] private Button saveButton;                  // "Подтвердить авто" → разблокировка модулей

//     [Header("Кнопки модулей и навигации")]
//     [SerializeField] private Button backToCarSelectionButton;    // "Назад" к выбору авто
//     [SerializeField] private Button finalizeButton;              // "Финальное сохранить" / "Готово" / "Начать"

//     [Header("Камера после финального выбора")]
//     [SerializeField] private GameObject cameraPrefab;            // ← префаб камеры (Cinemachine или обычная)

//     private GameObject currentCarInstance;
//     private GameObject currentVisual;

//     private int currentCarIndex = 0;
//     private bool modulesUnlocked = false;
//     private bool isFinalized = false;

//     private const string KEY_CAR = "SelectedCarIndex";
//     private const string KEY_MODULES_UNLOCKED = "ModulesUnlocked";

//     void Start()
//     {
//         LoadSelections();

//         currentCarInstance = Instantiate(carBasePrefab, spawnPoint.position, spawnPoint.rotation);

//         UpdateCarVisual(false);

//         // Подключение кнопок
//         if (nextCarButton != null) nextCarButton.onClick.AddListener(NextCar);
//         if (prevCarButton != null) prevCarButton.onClick.AddListener(PrevCar);
//         if (saveButton != null) saveButton.onClick.AddListener(SaveAndUnlockModules);
//         if (finalizeButton != null) finalizeButton.onClick.AddListener(FinalizeAndStart);
//         if (backToCarSelectionButton != null)
//         {
//             backToCarSelectionButton.onClick.AddListener(BackToCarSelection);
//             backToCarSelectionButton.gameObject.SetActive(modulesUnlocked);
//         }

//         // Финальная кнопка изначально скрыта
//         if (finalizeButton != null)
//             finalizeButton.gameObject.SetActive(false);
//     }

//     private void NextCar()
//     {
//         if (isFinalized) return;
//         currentCarIndex = (currentCarIndex + 1) % carDatas.Length;
//         UpdateCarVisual(false);
//     }

//     private void PrevCar()
//     {
//         if (isFinalized) return;
//         currentCarIndex = (currentCarIndex - 1 + carDatas.Length) % carDatas.Length;
//         UpdateCarVisual(false);
//     }

//     // private void UpdateCarVisual(bool notifyModules)
//     // {
//     //     if (currentVisual != null)
//     //     {
//     //         Destroy(currentVisual);
//     //         currentVisual = null;
//     //     }

//     //     if (carDatas == null || carDatas.Length == 0) return;

//     //     SOData data = carDatas[currentCarIndex];

//     //     if (data != null && data.VisualPrefab != null)
//     //     {
//     //         currentVisual = Instantiate(data.VisualPrefab, currentCarInstance.transform);
//     //         currentVisual.transform.localPosition = Vector3.zero;
//     //         currentVisual.transform.localRotation = Quaternion.identity;
//     //     }

//     //     if (notifyModules)
//     //     {
//     //         var selector = GetComponent<SelectModules>();
//     //         if (selector != null) selector.OnCarVisualChanged();
//     //     }
//     // }
//     private void UpdateCarVisual(bool notifyModules)
// {
//     if (currentVisual != null)
//     {
//         Destroy(currentVisual);
//         currentVisual = null;
//     }

//     if (carDatas == null || carDatas.Length == 0) return;

//       SOData data = carDatas[currentCarIndex];

//     if (data != null && data.VisualPrefab != null)
//     {
//         currentVisual = Instantiate(data.VisualPrefab, currentCarInstance.transform);
//         currentVisual.transform.localPosition = Vector3.zero;
//         currentVisual.transform.localRotation = Quaternion.identity;

//         // Инициализируем колёса в контроллере
//         var controller = currentCarInstance.GetComponent<SimpleCarController>();
//         if (controller != null)
//         {
//             controller.Initialize();
//         }
//     }

//     if (notifyModules)
//     {
//         var selector = GetComponent<SelectModules>();
//         if (selector != null) selector.OnCarVisualChanged();
//     }
// }

//     private void SaveAndUnlockModules()
//     {
//         if (isFinalized) return;

//         modulesUnlocked = true;
//         SaveSelections();

//         if (backToCarSelectionButton != null)
//             backToCarSelectionButton.gameObject.SetActive(true);

//         if (finalizeButton != null)
//             finalizeButton.gameObject.SetActive(true);

//         var selector = GetComponent<SelectModules>();
//         if (selector != null)
//             selector.ApplyModulesNow();

//         Debug.Log("Машина подтверждена → модули разблокированы");
//     }

//     private void FinalizeAndStart()
// {
//     if (isFinalized) return;

//     SaveSelections();
//     SetAllButtonsActive(false);

//     if (cameraPrefab == null)
//     {
//         Debug.LogError("cameraPrefab не назначен!");
//         return;
//     }

//     GameObject camObj = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity);

// var vcam = camObj.GetComponent<CinemachineCamera>();
// if (vcam != null && currentCarInstance != null)
// {
//     vcam.Follow = currentCarInstance.transform;
//     vcam.LookAt = currentCarInstance.transform;  // или currentVisual.transform.Find("CameraTarget")

//     // Принудительно заставляем Cinemachine обновиться
//     vcam.ForceCameraPosition(currentCarInstance.transform.position + new Vector3(0, 2, -6), Quaternion.identity);
// vcam.transform.position = currentCarInstance.transform.position + new Vector3(0, 2, -6);
// vcam.Priority = 30;

//     Debug.Log($"Камера привязана к: Follow = {vcam.Follow?.name}, LookAt = {vcam.LookAt?.name}");
// }

//     var follower = currentCarInstance.GetComponentInChildren<WeaponFollower>();
//     if (follower != null)
//     {
//         follower.enabled = true;
//     }

//     isFinalized = true;
// }

//     private void BackToCarSelection()
//     {
//         if (isFinalized) return;

//         var selector = GetComponent<SelectModules>();
//         if (selector != null)
//             selector.HideModules();

//         modulesUnlocked = false;
//         SaveSelections();

//         if (backToCarSelectionButton != null)
//             backToCarSelectionButton.gameObject.SetActive(false);

//         if (finalizeButton != null)
//             finalizeButton.gameObject.SetActive(false);

//         UpdateCarVisual(false);

//         Debug.Log("Вернулись к выбору авто");
//     }

//     private void SetAllButtonsActive(bool active)
//     {
//         if (nextCarButton != null)     nextCarButton.gameObject.SetActive(active);
//         if (prevCarButton != null)     prevCarButton.gameObject.SetActive(active);
//         if (saveButton != null)        saveButton.gameObject.SetActive(active);
//         if (backToCarSelectionButton != null) backToCarSelectionButton.gameObject.SetActive(active);
//         if (finalizeButton != null)    finalizeButton.gameObject.SetActive(active);
//     }

//     // Методы доступа
//     public SOData GetCurrentSOData()
//     {
//         if (carDatas == null || currentCarIndex < 0 || currentCarIndex >= carDatas.Length) return null;
//         return carDatas[currentCarIndex];
//     }

//     public Transform GetCurrentVisual()
//     {
//         return currentVisual != null ? currentVisual.transform : null;
//     }

//     public int GetCurrentCarIndex()
//     {
//         return currentCarIndex;
//     }

//     public bool AreModulesUnlocked()
//     {
//         return modulesUnlocked && !isFinalized;
//     }

//     // Сохранение / загрузка
//     private void LoadSelections()
//     {
//         currentCarIndex = PlayerPrefs.GetInt(KEY_CAR, 0);
//         currentCarIndex = Mathf.Clamp(currentCarIndex, 0, carDatas.Length - 1);

//         modulesUnlocked = PlayerPrefs.GetInt(KEY_MODULES_UNLOCKED, 0) == 1;
//     }

//     private void SaveSelections()
//     {
//         PlayerPrefs.SetInt(KEY_CAR, currentCarIndex);
//         PlayerPrefs.SetInt(KEY_MODULES_UNLOCKED, modulesUnlocked ? 1 : 0);
//         PlayerPrefs.Save();
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections; // ← правильный using для CinemachineVirtualCamera

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject carBasePrefab;
    [SerializeField] private SOData[] carDatas;


    [Header("Кнопки выбора машины")]
    [SerializeField] private Button nextCarButton;
    [SerializeField] private Button prevCarButton;
    [SerializeField] private Button saveButton;                  // "Подтвердить авто" → разблокировка модулей

    [Header("Кнопки модулей и навигации")]
    [SerializeField] private Button backToCarSelectionButton;    // "Назад" к выбору авто
    [SerializeField] private Button finalizeButton;              // "Финальное сохранить" / "Готово" / "Начать"

    [Header("Камера после финального выбора")]
    [SerializeField] private GameObject cameraPrefab;            // префаб с CinemachineVirtualCamera

    private GameObject currentCarInstance;
    private GameObject currentVisual;

    private int currentCarIndex = 0;
    private bool modulesUnlocked = false;
    private bool isFinalized = false;

    private const string KEY_CAR = "SelectedCarIndex";
    private const string KEY_MODULES_UNLOCKED = "ModulesUnlocked";

    private void Awake()
    {
        LoadSelections();

        currentCarInstance = Instantiate(carBasePrefab, spawnPoint.position, spawnPoint.rotation);

        UpdateCarVisual(false);

        // Подключение кнопок
        if (nextCarButton != null) nextCarButton.onClick.AddListener(NextCar);
        if (prevCarButton != null) prevCarButton.onClick.AddListener(PrevCar);
        if (saveButton != null) saveButton.onClick.AddListener(SaveAndUnlockModules);
        if (finalizeButton != null) finalizeButton.onClick.AddListener(FinalizeAndStart);
        if (backToCarSelectionButton != null)
        {
            backToCarSelectionButton.onClick.AddListener(BackToCarSelection);
            backToCarSelectionButton.gameObject.SetActive(modulesUnlocked);
        }

        if (finalizeButton != null)
            finalizeButton.gameObject.SetActive(false);
    }

    private void NextCar()
    {
        if (isFinalized) return;
        currentCarIndex = (currentCarIndex + 1) % carDatas.Length;
        UpdateCarVisual(false);
    }

    private void PrevCar()
    {
        if (isFinalized) return;
        currentCarIndex = (currentCarIndex - 1 + carDatas.Length) % carDatas.Length;
        UpdateCarVisual(false);
    }

    private void UpdateCarVisual(bool notifyModules)
{
    if (currentVisual != null)
    {
        Destroy(currentVisual);
        currentVisual = null;
    }

    if (carDatas == null || carDatas.Length == 0) return;

    SOData data = carDatas[currentCarIndex];

    if (data != null && data.VisualPrefab != null)
    {
        currentVisual = Instantiate(data.VisualPrefab, currentCarInstance.transform);
        currentVisual.transform.localPosition = Vector3.zero;
        currentVisual.transform.localRotation = Quaternion.identity;

        // ← Здесь запускаем отложенную инициализацию
        StartCoroutine(InitializeAfterFrame());
    }

    if (notifyModules)
    {
        var selector = GetComponent<SelectModules>();
        if (selector != null) selector.OnCarVisualChanged();
    }
}

// Новая корутина
private IEnumerator InitializeAfterFrame()
{
    // Ждём конец текущего кадра (или 1 FixedUpdate)
    yield return new WaitForFixedUpdate();  // или yield return null; для 1 кадра

    var controller = currentCarInstance.GetComponent<SimpleCarController>();
    if (controller != null)
    {
        controller.Initialize();
        Debug.Log("Колёса инициализированы с задержкой");
    }
}

    private void SaveAndUnlockModules()
    {
        if (isFinalized) return;

        modulesUnlocked = true;
        SaveSelections();

        if (backToCarSelectionButton != null)
            backToCarSelectionButton.gameObject.SetActive(true);

        if (finalizeButton != null)
            finalizeButton.gameObject.SetActive(true);

        var selector = GetComponent<SelectModules>();
        if (selector != null)
            selector.ApplyModulesNow();

        Debug.Log("Машина подтверждена → модули разблокированы");
    }
    private void FinalizeAndStart()
    {
        if (isFinalized) return;

        SaveSelections();
        SetAllButtonsActive(false);

        if (cameraPrefab == null)
        {
            Debug.LogError("cameraPrefab не назначен!");
            return;
        }

        // Спавним камеру НЕ в (0,0,0), а рядом с машиной
        Vector3 camSpawnPos = currentCarInstance.transform.position + new Vector3(0, 2, -6);
        GameObject camObj = Instantiate(cameraPrefab, camSpawnPos, Quaternion.identity);

        var vcam = camObj.GetComponent<CinemachineCamera>();
        if (vcam != null && currentCarInstance != null)
        {
            vcam.Follow = currentVisual.transform;
            vcam.LookAt = currentVisual.transform;

            // Принудительно телепортируем и обновляем
            vcam.ForceCameraPosition(camSpawnPos, currentCarInstance.transform.rotation);
            vcam.Priority = 30; // высокий приоритет

            // Отвязываем от родителя (если случайно стала ребёнком)
            camObj.transform.SetParent(null);

            Debug.Log($"Камера привязана к: Follow = {vcam.Follow?.name}, LookAt = {vcam.LookAt?.name}");
        }
        else
        {
            Debug.LogError("Не найден CinemachineVirtualCamera или currentCarInstance");
        }

        // Активация WeaponFollower (если нужно)
        var follower = currentCarInstance.GetComponentInChildren<WeaponFollower>();
        if (follower != null)
        {
            follower.enabled = true;
        }

        isFinalized = true;
    }

    private void BackToCarSelection()
    {
        if (isFinalized) return;

        var selector = GetComponent<SelectModules>();
        if (selector != null)
            selector.HideModules();

        modulesUnlocked = false;
        SaveSelections();

        if (backToCarSelectionButton != null)
            backToCarSelectionButton.gameObject.SetActive(false);

        if (finalizeButton != null)
            finalizeButton.gameObject.SetActive(false);

        UpdateCarVisual(false);

        Debug.Log("Вернулись к выбору авто");
    }

    private void SetAllButtonsActive(bool active)
    {
        if (nextCarButton != null)     nextCarButton.gameObject.SetActive(active);
        if (prevCarButton != null)     prevCarButton.gameObject.SetActive(active);
        if (saveButton != null)        saveButton.gameObject.SetActive(active);
        if (backToCarSelectionButton != null) backToCarSelectionButton.gameObject.SetActive(active);
        if (finalizeButton != null)    finalizeButton.gameObject.SetActive(active);
    }

    // Методы доступа
    public SOData GetCurrentSOData()
    {
        if (carDatas == null || currentCarIndex < 0 || currentCarIndex >= carDatas.Length) return null;
        return carDatas[currentCarIndex];
    }

    public Transform GetCurrentVisual()
    {
        return currentVisual != null ? currentVisual.transform : null;
    }

    public int GetCurrentCarIndex()
    {
        return currentCarIndex;
    }

    public bool AreModulesUnlocked()
    {
        return modulesUnlocked && !isFinalized;
    }

    // Сохранение / загрузка
    private void LoadSelections()
    {
        currentCarIndex = PlayerPrefs.GetInt(KEY_CAR, 0);
        currentCarIndex = Mathf.Clamp(currentCarIndex, 0, carDatas.Length - 1);

        modulesUnlocked = PlayerPrefs.GetInt(KEY_MODULES_UNLOCKED, 0) == 1;
    }

    private void SaveSelections()
    {
        PlayerPrefs.SetInt(KEY_CAR, currentCarIndex);
        PlayerPrefs.SetInt(KEY_MODULES_UNLOCKED, modulesUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }
}