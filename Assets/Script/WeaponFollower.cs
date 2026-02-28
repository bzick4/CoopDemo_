using UnityEngine;

public class WeaponFollower : MonoBehaviour
{
    private Transform weaponPoint;
    private Transform camTransform;

    [SerializeField] private bool debugLogs = true; // ← включи/выключи отладку

    void Awake()
    {
        // Находим камеру
        camTransform = Camera.main ? Camera.main.transform : null;
        if (camTransform == null)
        {
            Debug.LogError("[WeaponFollower] Camera.main НЕ НАЙДЕНА");
            enabled = false;
            return;
        }

        if (debugLogs)
            Debug.Log("[WeaponFollower] Камера найдена: " + camTransform.name);
    }

    void Start()
    {
        FindWeaponPoint();
    }

    // Вызывается из CarSpawner после смены визуала
    public void OnVisualChanged()
    {
        FindWeaponPoint();
    }

    private void FindWeaponPoint()
    {
        // Визуал — первый ребёнок корня машины (или адаптируй под свою иерархию)
        Transform visual = transform.childCount > 0 ? transform.GetChild(0) : null;

        if (visual == null)
        {
            if (debugLogs) Debug.LogWarning("[WeaponFollower] Визуал не найден (GetChild(0))");
            return;
        }

        // Ищем точку по имени (или по пути, если нужно)
        weaponPoint = visual.Find("WeaponPoint");

        if (weaponPoint == null)
        {
            if (debugLogs) Debug.LogWarning($"[WeaponFollower] WeaponPoint НЕ НАЙДЕН внутри {visual.name}");
        }
        else
        {
            if (debugLogs)
                Debug.Log($"[WeaponFollower] WeaponPoint найден: {weaponPoint.name} → позиция {weaponPoint.position}");
        }
    }

    void LateUpdate()
    {
        if (weaponPoint == null || camTransform == null) return;

        // Берём направление взгляда камеры
        Vector3 camForward = camTransform.forward;

        // Обнуляем вертикальную компоненту → поворот только по горизонтали (Y)
        camForward.y = 0f;

        // Если вектор почти нулевой — не трогаем
        if (camForward.sqrMagnitude < 0.001f) return;

        camForward.Normalize();

        // Поворачиваем точку строго по Y
        Quaternion targetRotation = Quaternion.LookRotation(camForward);

        // Если в префабе оружие смотрит не по Z-оси — добавь корректировку здесь:
        // targetRotation *= Quaternion.Euler(0, 90f, 0);   // если смотрит по X
        // targetRotation *= Quaternion.Euler(0, -90f, 0);  // если по -X
        // targetRotation *= Quaternion.Euler(0, 180f, 0);  // если смотрит назад

        weaponPoint.rotation = targetRotation;

        // Отладка каждые ~0.5 сек (чтобы видеть, работает ли)
        if (debugLogs && Time.frameCount % 30 == 0)
        {
            Debug.Log($"[WeaponFollower] Камера смотрит: forward = {camForward:F3} | Weapon Y = {weaponPoint.eulerAngles.y:F1}");
        }
    }
}