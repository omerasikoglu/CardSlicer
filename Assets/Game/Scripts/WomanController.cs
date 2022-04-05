using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomanController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    void Update()
    {
        transform.position += movementSpeed * Time.deltaTime * transform.forward;
    }
}
