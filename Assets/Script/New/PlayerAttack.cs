using UnityEngine;
using Unity.Netcode;

public class PlayerAttack : NetworkBehaviour
{
    [Header("Настройки")]
    [SerializeField] private KeyCode attackKey = KeyCode.F;
    [SerializeField] private float attackCooldown = 0.8f;

    private float lastAttackTime;
    private Animator animator;
    private PlayerVisual playerVisual;
    

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerVisual = GetComponent<PlayerVisual>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(attackKey))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        int attackType = playerVisual?.GetAttackType() ?? 0; // Получаем тип атаки из визуала, или 0 по умолчанию
        AttackServerRpc(attackType);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackServerRpc(int attackType)
    {
   
        PlayAttackClientRpc(attackType);
    }

    [ClientRpc]
    private void PlayAttackClientRpc(int attackType)
    {
        // Получаем из SOData, если нужно
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) return;

        animator.SetInteger("AttackType", attackType);
       animator.SetTrigger("Attack");

        //animator.ResetTrigger("Attack"); // сброс триггера для повторного срабатывания
    }

    // Если нужно передать аниматор извне
    public void SetAnimator(Animator anim) => animator = anim;
}