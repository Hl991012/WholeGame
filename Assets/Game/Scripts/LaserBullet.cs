using System;
using UniRx;
using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    private IDisposable timer;

    private float curAttackInterval;

    private float nextAttackTime;
    
    private RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[10];
    
    public void Emission(float attackDuration, float attackInterval, Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.gameObject.SetActive(true);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        curAttackInterval = attackInterval;
        nextAttackTime = Time.time;
        
        var center = new Vector2((startPosition.x + endPosition.x) / 2, (startPosition.y + endPosition.y) / 2);
        var attackSize = new Vector2(0.1f, Vector2.Distance(startPosition, endPosition));

        var attackEndTime = Time.time + attackDuration;
        
        timer?.Dispose();
        timer = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(attackInterval)).Subscribe(_ =>
        {
            if (Time.time > attackEndTime)
            {
                lineRenderer.gameObject.SetActive(false);   
            }
            else
            {
                Array.Clear(raycastHit2Ds, 0, raycastHit2Ds.Length);
                if (Physics2D.BoxCastNonAlloc(center, attackSize, Vector2.Angle(Vector2.up, (endPosition - startPosition)), Vector2.zero, raycastHit2Ds, LayerMask.GetMask("Enemy")) > 0)
                {
                    foreach (var item in raycastHit2Ds)
                    {
                        if (item.transform != null)
                        {
                            Debug.LogError(item.transform.name);
                        }
                    }
                }
            }
        }).AddTo(this);
    }
    

    // private void OnTriggerExit(Collider other)
    // {
    //     if (Time.time > nextAttackTime)
    //     {
    //         nextAttackTime += curAttackInterval;
    //         
    //     }
    // }
    
}
