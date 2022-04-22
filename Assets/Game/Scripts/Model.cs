using System;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

public abstract class Model : MonoBehaviour
{
    [SerializeField, Foldout("[Movement]")] private float movementSpeed = 2f;
    protected const int defaultSpeed = 2;

    protected virtual void Update()
    {
        HandleMovement();
    }
    private void HandleMovement() {
        transform.position += movementSpeed * Time.deltaTime * Vector3.forward;
    }

    protected void SetMovementSpeed(int speed)
    {
        movementSpeed = speed;
    }
}
