using UnityEngine;

[CreateAssetMenu(fileName = "SOData", menuName = "ScriptableObjects/SOData")]
public class SOData : ScriptableObject
{

    [Header("Visual")]
    public GameObject VisualPrefab;
   

    [Header("Player Stats")]
    public float MaxHealth;
    public float MaxStamina;
    public float MaxManna;


    [Header("Movement")]
    public float WalkSpeed;
    public float RunSpeed;

    public string CharacterName;
}
