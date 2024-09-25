using System;
using Pathfinding;
using UniRx;
using UnityEngine;

public class Ghost : VersionedMonoBehaviour 
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject ghostTrigger1;
    [SerializeField] private GameObject ghostTrigger2;

    IAstarAI ai;

    private Vector3 originalPos;

    private IDisposable timer;

    void OnEnable ()
    {
        ai = GetComponent<IAstarAI>();
        if (ai != null) ai.onSearchPath += Update;

        originalPos = transform.position;
    }

    void OnDisable () {
        if (ai != null) ai.onSearchPath -= Update;
    }
    
    void Update () {
        if (target != null && ai != null) ai.destination = target.position;
    }

    public void BeRelease(Transform followTarget)
    {
        timer?.Dispose();
        timer = Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            ghostTrigger1.gameObject.SetActive(false);
            ghostTrigger2.gameObject.SetActive(false);
            ai.canMove = true;
            target = followTarget;
            StageLoadPresenter.Instance.AddActivityGhost(this);
        }).AddTo(this);
    }

    public void Stop()
    {
        ai.canMove = false;
        target = null;
        ghostTrigger1.gameObject.SetActive(true);
        ghostTrigger2.gameObject.SetActive(true);
        timer?.Dispose();
    }

    public void ResetState()
    {
        ai.canMove = false;
        target = null;
        transform.position = originalPos;
        ghostTrigger1.gameObject.SetActive(true);
        ghostTrigger2.gameObject.SetActive(true);
        timer?.Dispose();
    }
}
