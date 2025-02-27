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

    private SingleCircleItem nextCircle;

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
            curCircle = nextCircle;
            curCircle.SetNormalState(true);
            curCircle.transform.SetParent(transform);
                
            nextCircle = Instantiate(circleItems[0], transform.parent).GetComponent<SingleCircleItem>();
            nextCircle.transform.localPosition = new Vector3(0, 0.56f, 0);
            nextCircle.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            nextCircle.gameObject.SetActive(true);
            nextCircle.SetPreViewState();
        }
    }

    public void StartGame()
    {
        IsGaming = true;
        if (nextCircle != null)
        {
            nextCircle.gameObject.SetActive(true);
        }
    }

    public void Reset()
    {
        sphereTrans.position = new Vector3(0, 1.4f, 0);
        
        IsGaming = false;
        for (var i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        if (nextCircle != null)
        {
            Destroy(nextCircle.gameObject);
        }

        curCircle = Instantiate(initCircle, transform).GetComponent<SingleCircleItem>();
        curCircle.transform.localPosition = Vector3.zero;
        curCircle.transform.rotation = Quaternion.Euler(0, 0, -3.24f);
        curCircle.gameObject.SetActive(true);
        curCircle.SetNormalState(false);

        nextCircle = Instantiate(circleItems[Random.Range(0, circleItems.Count)], transform.parent).GetComponent<SingleCircleItem>();
        nextCircle.transform.localPosition = new Vector3(0, 0.56f, 0);
        nextCircle.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        nextCircle.gameObject.SetActive(false);
        nextCircle.SetPreViewState();
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
