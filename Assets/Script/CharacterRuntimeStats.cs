using Unity.Netcode;
using UnityEngine;

public class CharacterRuntimeStats : NetworkBehaviour
{
    // Сетевые переменные для синхронизации текущих статов
    public NetworkVariable<float> Health = new(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<float> Mana = new(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<float> Stamina = new(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Максимальные значения (для UI / расчётов)
    public float MaxHealth { get; private set; }
    public float MaxMana { get; private set; }
    public float MaxStamina { get; private set; }

    // Инициализация при спавне персонажа
    public void Init(SOData so)
    {
        if (IsServer)
            return;
        

        if (so == null)
        {
            Debug.LogError("Character SOData is null!", this);
            return;
        }

        MaxHealth = so.MaxHealth;
        MaxMana = so.MaxManna;
        MaxStamina = so.MaxStamina;

        Health.Value = MaxHealth;
        Mana.Value = MaxMana;
        Stamina.Value = MaxStamina;

        
    }

    #region Методы для изменения статов (только сервер)

    public void TakeDamage(float amount)
    {
        if (!IsServer) return;

        Health.Value = Mathf.Max(Health.Value - amount, 0f);
    }

    public void UseMana(float amount)
    {
        if (!IsServer) return;

        Mana.Value = Mathf.Max(Mana.Value - amount, 0f);
    }

    public void UseStamina(float amount)
    {
        if (!IsServer) return;

        Stamina.Value = Mathf.Max(Stamina.Value - amount, 0f);
    }

    public void Heal(float amount)
    {
        if (!IsServer) return;

        Health.Value = Mathf.Min(Health.Value + amount, MaxHealth);
    }

    public void RestoreMana(float amount)
    {
        if (!IsServer) return;

        Mana.Value = Mathf.Min(Mana.Value + amount, MaxMana);
    }

    public void RestoreStamina(float amount)
    {
        if (!IsServer) return;

        Stamina.Value = Mathf.Min(Stamina.Value + amount, MaxStamina);
    }

    #endregion
}