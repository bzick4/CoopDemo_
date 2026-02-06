// using UnityEngine;
// using UnityEngine.UI; // если используешь UI
// using TMPro; // если хочешь текстовые индикаторы

// public class VisualCharacter : MonoBehaviour
// {
//     private CharacterRuntimeStats runtimeStats;

//     // Пример: UI элементы над головой персонажа
//     [Header("UI Elements")]
//     [SerializeField] private Image healthBar;
//     [SerializeField] private Image manaBar;
//     [SerializeField] private Image staminaBar;
//     [SerializeField] private TextMeshProUGUI nameText;

//     // Метод для инициализации
//     public void Init(CharacterRuntimeStats stats)
//     {
//         runtimeStats = stats;

//         // Обновляем UI сразу
//         UpdateVisuals();

//         // Подписка на изменения NetworkVariable
//         if (runtimeStats != null)
//         {
//             runtimeStats.Health.OnValueChanged += (_, __) => UpdateVisuals();
//             runtimeStats.Mana.OnValueChanged += (_, __) => UpdateVisuals();
//             runtimeStats.Stamina.OnValueChanged += (_, __) => UpdateVisuals();
//         }
//     }

//     private void OnDestroy()
//     {
//         // Отписываемся от NetworkVariable, чтобы не было ошибок
//         if (runtimeStats != null)
//         {
//             runtimeStats.Health.OnValueChanged -= (_, __) => UpdateVisuals();
//             runtimeStats.Mana.OnValueChanged -= (_, __) => UpdateVisuals();
//             runtimeStats.Stamina.OnValueChanged -= (_, __) => UpdateVisuals();
//         }
//     }

//     // Основной апдейт UI / визуала
//     private void UpdateVisuals()
//     {
//         if (runtimeStats == null) return;

//         if (healthBar != null)
//             healthBar.fillAmount = runtimeStats.Health.Value / runtimeStats.Health.Value; // если у тебя есть MaxHealth, замени на max

//         if (manaBar != null)
//             manaBar.fillAmount = runtimeStats.Mana.Value / runtimeStats.Mana.Value; // аналогично

//         if (staminaBar != null)
//             staminaBar.fillAmount = runtimeStats.Stamina.Value / runtimeStats.Stamina.Value;
//     }
// }

using UnityEngine;

public class VisualCharacter : MonoBehaviour
{
    private CharacterRuntimeStats _stats;

    public void Init(CharacterRuntimeStats stats)
    {
        _stats = stats;

        if (_stats == null)
        {
            Debug.LogError("[VisualCharacter] RuntimeStats is NULL", this);
            return;
        }

        Debug.Log(
            $"[VisualCharacter INIT] " +
            $"HP: {_stats.Health.Value}/{_stats.MaxHealth}, " +
            $"Mana: {_stats.Mana.Value}/{_stats.MaxMana}, " +
            $"Stamina: {_stats.Stamina.Value}/{_stats.MaxStamina}, " +
            this
        );

        // Подписка на изменения (для дебага)
        _stats.Health.OnValueChanged += OnHealthChanged;
        _stats.Mana.OnValueChanged += OnManaChanged;
        _stats.Stamina.OnValueChanged += OnStaminaChanged;
    }

    private void OnDestroy()
    {
        if (_stats == null) return;

        _stats.Health.OnValueChanged -= OnHealthChanged;
        _stats.Mana.OnValueChanged -= OnManaChanged;
        _stats.Stamina.OnValueChanged -= OnStaminaChanged;
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        Debug.Log($"[VisualCharacter] Health: {oldValue} → {newValue}", this);
    }

    private void OnManaChanged(float oldValue, float newValue)
    {
        Debug.Log($"[VisualCharacter] Mana: {oldValue} → {newValue}", this);
    }

    private void OnStaminaChanged(float oldValue, float newValue)
    {
        Debug.Log($"[VisualCharacter] Stamina: {oldValue} → {newValue}", this);
    }
}