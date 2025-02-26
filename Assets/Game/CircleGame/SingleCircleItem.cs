using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCircleItem : MonoBehaviour
{
    [SerializeField] private Collider2D[] colliders;
    [SerializeField] private int totalCount;

    private void Awake()
    {
        RemainCount = totalCount;
        foreach (var item in colliders)
        {
            item.enabled = true;
        }
    }

    public int RemainCount { get; private set; }

    public bool OnBeAttack()
    {
        RemainCount--;
        return RemainCount <= 0;
    }
}
