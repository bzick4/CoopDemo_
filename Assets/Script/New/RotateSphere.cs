using System;
using UnityEngine;

public class RotateSphere : MonoBehaviour
{
    public float rotationSpeed = 30f;
    [SerializeField] private Vector3 rotationAxis;

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}

