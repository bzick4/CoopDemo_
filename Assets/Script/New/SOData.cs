using UnityEngine;

[CreateAssetMenu(fileName = "SOData", menuName = "ScriptableObjects/SOData")]
public class SOData : ScriptableObject
{

    [Header("Visual")]
    public GameObject VisualPrefab;
    public int AttackType;
   

    [Header("Player Stats")]
    public float MaxHealth;
    public float MaxStamina;
    public float MaxManna;


    [Header("Movement")]
    public float WalkSpeed;
    public float RunSpeed;

    // [Header("Атака")]
    // public PlayerAttackType PlayerAttack = PlayerAttackType.Melee;  // тип атаки
    // public float Damage = 25f;                        // урон
    // public float AttackRange = 2f;                    // дальность (для ближней)
    // public float ProjectileSpeed = 10f;               // скорость для дальнего
    // public GameObject ProjectilePrefab;               // префаб проектайла (для дальнего)

    // public enum PlayerAttackType
    // {
    // Melee,      // ближняя (hitbox)
    // Ranged      // дальняя (проектайл)
    // }
}
