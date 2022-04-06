using NaughtyAttributes;
using UnityEngine;

public abstract class Model : MonoBehaviour
{
    [SerializeField, Foldout("[Movement]")] private float movementSpeed = 2f;
    protected virtual void Update()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {
        transform.position += movementSpeed * Time.deltaTime * transform.forward;
    }
}
