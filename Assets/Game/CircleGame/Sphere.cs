using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NMNH.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sphere : MonoBehaviour
{
    [SerializeField] private CircleConctrol circleConctrol;
    
    private Vector3 moveDirection = new (0, -1, 0);

    public float moveSpeed = 0.02f;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other != null)
        {
            if (other.transform.CompareTag("Circle"))
            {
                moveDirection = Quaternion.Euler(0, 0, Random.Range(-35, 35)) * (new Vector3(0, 0.56f, 0) - transform.localPosition);
                moveDirection.z = 0;
                moveDirection = Vector3.Normalize(moveDirection);
                if (other.gameObject.TryGetComponent<SpriteRenderer>(out var temp))
                {
                    DOTween.ToAlpha(() => temp.color, val => temp.color = val, 0, 0.5f)
                        .SetUpdate(true)
                        .SetLink(gameObject)
                        .OnComplete(() =>
                    {
                        Destroy(other.gameObject);
                        circleConctrol.OnCollisionEnter1();
                    });
                }
                CircleGameManager.Instance.CurScore++;
                AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.CirclePz);
            }

            if (other.transform.CompareTag("DeadLine"))
            {
                circleConctrol.OnGameOver();
                transform.localPosition = new Vector3(0, 1.4f, 0);
            }
        }
    }

    private void Update()
    {
        if (CircleConctrol.Instance.IsGaming)
        {
            transform.Translate(moveDirection * moveSpeed * Time.unscaledDeltaTime);   
        }
    }
}
