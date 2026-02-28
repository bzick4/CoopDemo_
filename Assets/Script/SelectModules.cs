using UnityEngine;
using UnityEngine.UI;

public class SelectModules : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private CarSpawner carSpawner;

    [Header("Массивы модулей")]
    [SerializeField] private GameObject[] grabPrefabs;
    [SerializeField] private GameObject[] weaponPrefabs;

    [Header("Кнопки")]
    [SerializeField] private Button nextGrabButton;
    [SerializeField] private Button prevGrabButton;
    [SerializeField] private Button nextWeaponButton;
    [SerializeField] private Button prevWeaponButton;

    private GameObject currentGrabModule;
    private GameObject currentWeaponModule;

    private int currentGrabIndex = 0;
    private int currentWeaponIndex = 0;

    private const string KEY_GRAB   = "SelectedGrabIndex";
    private const string KEY_WEAPON = "SelectedWeaponIndex";

    void Start()
    {
        LoadSelections();

        if (nextGrabButton)   nextGrabButton.onClick.AddListener(NextGrab);
        if (prevGrabButton)   prevGrabButton.onClick.AddListener(PrevGrab);
        if (nextWeaponButton) nextWeaponButton.onClick.AddListener(NextWeapon);
        if (prevWeaponButton) prevWeaponButton.onClick.AddListener(PrevWeapon);

        // НЕ применяем модули автоматически при старте
        // Они применятся только когда CarSpawner вызовет ApplyModulesNow()
    }

    // Вызывается CarSpawner'ом после смены визуала машины
    public void OnCarVisualChanged()
    {
        // Если модули уже применены ранее — обновляем их на новой модели
        // (но без этой проверки — просто обновляем, если есть что обновлять)
        UpdateGrabModule();
        UpdateWeaponModule();
    }

    // Вызывается CarSpawner'ом после нажатия "Сохранить"
    public void ApplyModulesNow()
    {
        UpdateGrabModule();
        UpdateWeaponModule();
    }

    // Вызывается CarSpawner'ом при нажатии "Назад"
    public void HideModules()
    {
        if (currentGrabModule)
        {
            Destroy(currentGrabModule);
            currentGrabModule = null;
        }

        if (currentWeaponModule)
        {
            Destroy(currentWeaponModule);
            currentWeaponModule = null;
        }
    }

    // ────────────────────────────────────────────── Grab
    private void NextGrab()
    {
        if (grabPrefabs.Length == 0) return;

        currentGrabIndex = (currentGrabIndex + 1) % grabPrefabs.Length;
        UpdateGrabModule();
        SaveSelections();
    }

    private void PrevGrab()
    {
        if (grabPrefabs.Length == 0) return;

        currentGrabIndex = (currentGrabIndex - 1 + grabPrefabs.Length) % grabPrefabs.Length;
        UpdateGrabModule();
        SaveSelections();
    }

    private void UpdateGrabModule()
    {
        if (currentGrabModule)
        {
            Destroy(currentGrabModule);
            currentGrabModule = null;
        }

        var data = carSpawner.GetCurrentSOData();
        if (data == null) return;

        var visual = carSpawner.GetCurrentVisual();
        if (visual == null) return;

        var point = visual.Find(data.grabPointName);
        if (point == null) return;

        if (currentGrabIndex >= 0 && currentGrabIndex < grabPrefabs.Length && grabPrefabs[currentGrabIndex] != null)
        {
            currentGrabModule = Instantiate(grabPrefabs[currentGrabIndex], point);
            currentGrabModule.transform.localPosition = Vector3.zero;
            currentGrabModule.transform.localRotation = Quaternion.identity;
        }
    }

    // ────────────────────────────────────────────── Weapon
    private void NextWeapon()
    {
        if (weaponPrefabs.Length == 0) return;

        currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabs.Length;
        UpdateWeaponModule();
        SaveSelections();
    }

    private void PrevWeapon()
    {
        if (weaponPrefabs.Length == 0) return;

        currentWeaponIndex = (currentWeaponIndex - 1 + weaponPrefabs.Length) % weaponPrefabs.Length;
        UpdateWeaponModule();
        SaveSelections();
    }

    private void UpdateWeaponModule()
    {
        if (currentWeaponModule)
        {
            Destroy(currentWeaponModule);
            currentWeaponModule = null;
        }

        var data = carSpawner.GetCurrentSOData();
        if (data == null) return;

        var visual = carSpawner.GetCurrentVisual();
        if (visual == null) return;

        var point = visual.Find(data.weaponPointName);
        if (point == null) return;

        if (currentWeaponIndex >= 0 && currentWeaponIndex < weaponPrefabs.Length && weaponPrefabs[currentWeaponIndex] != null)
        {
            currentWeaponModule = Instantiate(weaponPrefabs[currentWeaponIndex], point);
            currentWeaponModule.transform.localPosition = Vector3.zero;
            currentWeaponModule.transform.localRotation = Quaternion.identity;
        }
    }

    // ────────────────────────────────────────────── Сохранение / загрузка
    private void LoadSelections()
    {
        currentGrabIndex   = PlayerPrefs.GetInt(KEY_GRAB,   0);
        currentWeaponIndex = PlayerPrefs.GetInt(KEY_WEAPON, 0);

        currentGrabIndex   = Mathf.Clamp(currentGrabIndex,   0, grabPrefabs.Length   - 1);
        currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, weaponPrefabs.Length - 1);
    }

    private void SaveSelections()
    {
        PlayerPrefs.SetInt(KEY_GRAB,   currentGrabIndex);
        PlayerPrefs.SetInt(KEY_WEAPON, currentWeaponIndex);
        PlayerPrefs.Save();
    }
}