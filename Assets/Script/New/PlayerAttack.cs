using UnityEngine;
using Unity.Netcode;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private KeyCode attackKey = KeyCode.F;
    [SerializeField] private float attackCooldown = 0.8f;

    private float lastAttackTime;
    private Animator animator;
    private PlayerVisual playerVisual;
    //private SOData currentSOData;

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
            TryAttack(playerVisual != null ? playerVisual.GetAttackType() : 0);
        }
    }

//     private void LateUpdate()
// {
//     if (animator != null)
//     {
//         animator.ResetTrigger("Attack");
//     }
// }

    private void TryAttack(int attackType)
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            Debug.Log("[Attack] Кулдаун ещё не прошёл");
            return;
        }

        lastAttackTime = Time.time;

        //int attackType = playerVisual?.GetAttackType() ?? 4;
        Debug.Log($"[Attack] Тип атаки для визуала: {attackType}");

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
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[Attack] Animator не найден!");
                return;
            }
        }

        Debug.Log($"[Attack] Запуск на {(IsOwner ? "локальном" : "удалённом")} игроке | AttackType = {attackType}");

        animator.SetInteger("AttackType", attackType);
        animator.SetTrigger("Attack");

        //animator.SetInteger("AttackType", -1);
        animator.ResetTrigger("Attack");
    }
    public void OnAttackFinished()
{
    if (animator == null) return;

    animator.SetInteger("AttackType", -1);  // или 0, если -1 не подходит
    Debug.Log("[Attack] Анимация закончилась → AttackType сброшен в -1");
}

    public void SetAnimator(Animator anim) => animator = anim;
}