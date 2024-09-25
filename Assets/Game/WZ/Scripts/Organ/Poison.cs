using System;
using UniRx;
using UnityEngine;

public class Poison : MonoBehaviour
{
    [SerializeField] private GameObject posionTrigger;
    [SerializeField] private ParticleSystem poisonEffect;

    public float interval;
    public float duration;

    private void OnEnable()
    {
        Observable.Interval(TimeSpan.FromSeconds(interval)).Subscribe(_ =>
        {
            posionTrigger.SetActive(true);
            poisonEffect.Play();
            Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe(_ =>
            {
                posionTrigger.SetActive(false);
            }).AddTo(this);
        }).AddTo(this);
    }
}
