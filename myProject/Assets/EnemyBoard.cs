using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoard : MonoBehaviour
{
    public Transform target;

    private void Start()
    {
        target = Camera.main.gameObject.transform;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(target.forward, target.up);
    }
}