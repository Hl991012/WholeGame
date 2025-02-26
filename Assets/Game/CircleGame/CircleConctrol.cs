using System;
using System.Collections;
using System.Collections.Generic;
using GameFrame;
using UnityEngine;
using Random = UnityEngine.Random;

public class CircleConctrol : MonoSingleton<CircleConctrol>
{
    [SerializeField] private List<SingleCircleItem> circleItems;
    [SerializeField] private Transform circleTrans;
    [SerializeField] private SingleCircleItem initCircle;
    [SerializeField] private Transform sphereTrans;
    [SerializeField] private CircleGameUIPresenter circleGameUIPresenter;

    public bool IsGaming { get; private set; }

    public bool HasRevived { get; private set; }

    private Quaternion originalRotation;

    private float x;

    public float speed = 20;

    private SingleCircleItem curCircle;

    protected override void OnEnable()
    {
        base.OnEnable();
        Reset();
    }

    private void Update()
    {
        if (IsGaming && Input.GetMouseButtonDown(0))
        {
            originalRotation = circleTrans.rotation;
            x = Input.mousePosition.x;
        }

        if (IsGaming && Input.GetMouseButton(0))
        {
            circleTrans.rotation = originalRotation * Quaternion.Euler(0, 0, speed * (Input.mousePosition.x - x));
        }
    }

    public void OnCollisionEnter1()
    {
        if(curCircle == null) return;

        if (curCircle.OnBeAttack())
        {
            Destroy(curCircle.gameObject);
            curCircle = Instantiate(circleItems[0], transform).GetComponent<SingleCircleItem>();
            curCircle.transform.localPosition = Vector3.zero;
            curCircle.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            curCircle.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        IsGaming = true;
    }

    public void Reset()
    {
        circleTrans.position = new Vector3(0, 1.3f, 0);
        IsGaming = false;
        for (var i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        curCircle = Instantiate(initCircle, transform).GetComponent<SingleCircleItem>();
        curCircle.transform.localPosition = Vector3.zero;
        curCircle.transform.rotation = Quaternion.Euler(0, 0, -3.24f);
        curCircle.gameObject.SetActive(true);
    }

    public void Revive()
    {
        sphereTrans.localPosition = new Vector3(0, 1.4f, 0);
        HasRevived = true;
        IsGaming = true;
    }

    public void OnGameOver()
    {
        IsGaming = false;
        if (HasRevived)
        {
            Reset();
            circleGameUIPresenter.ShowGameOverPanel();
        }
        else
        {
            circleGameUIPresenter.ShowRevivePanel();
        }
    }
}
