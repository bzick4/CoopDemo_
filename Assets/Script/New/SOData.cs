using UnityEngine;

[CreateAssetMenu(fileName = "SOData", menuName = "Cars/SOData")]
public class SOData : ScriptableObject
{

    [Header("Visual")]
    public GameObject VisualPrefab;
    public string grabPointName = "GrabPoint";      // ← это обязательно добавить
    public string weaponPointName = "WeaponPoint";

    [Header("Mass")]
    public float Mass;

    [Header("Player Stats")]
    public float MaxHealth;


    [Header("Movement")]
    public float Speed;

   
}
