using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    Enemy[] enemies;
    public int groupCount;

    private void Start()
    {
        enemies = GetComponentsInChildren<Enemy>();
        groupCount = enemies.Length;

        InvokeRepeating("CheckChildren", 10, 10);
    }

    public void CheckChildren()
    {
        if (groupCount <= 0)
        {
            Reset();
        }
    }

    private void Reset()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].Reset();
        }

        groupCount = enemies.Length;
    }
}