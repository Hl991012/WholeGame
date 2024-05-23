using System;
using System.Collections;
using System.Collections.Generic;
using GameFrame;
using UnityEngine;

public class Enemy : MonoBehaviour, IRecyclable
{
    
    public float moveSpeed;

    public void BeAttacked(int hp)
    {
        
    }
    private void Update()
    {
        transform.Translate(Vector3.down * (Time.deltaTime * moveSpeed));
    }

    public RecyclableType RecyclableType => RecyclableType.Enemy;
    
    public void OnGet()
    {
        gameObject.SetActive(true);
    }

    public void OnReturn()
    {
        gameObject.SetActive(false);
    }
}
