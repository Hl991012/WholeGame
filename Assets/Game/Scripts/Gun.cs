using GameFrame;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform firePos;
    public float attackRadius;
    public float attackInterval = 2f;

    public Enemy enemy;

    private float nextFireTime;

    private void Update()
    {
        if (Time.realtimeSinceStartup > nextFireTime)
        {
            Fire();
        }
    }
    
    private void Fire()
    {
        transform.LookAt(enemy.transform);
        nextFireTime += attackInterval;
        var bullet = GamingPools.Instance.GetFromPool<Bullet>(RecyclableType.NormalBullet);
        bullet.transform.position = firePos.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position; // 圆心位置，这里假设是该脚本挂载的游戏对象的位置

        for (int i = 0; i < 32; i++)
        {
            var angle = i * (360f / 32) * Mathf.Deg2Rad;
            var p1 = new Vector3(center.x + attackRadius * Mathf.Cos(angle), center.y + attackRadius * Mathf.Sin(angle), center.z);
            var p2 = new Vector3(center.x + attackRadius * Mathf.Cos(angle + (360f / 32) * Mathf.Deg2Rad), center.y + attackRadius * Mathf.Sin(angle + (360f / 32) * Mathf.Deg2Rad), center.z);
            Gizmos.DrawLine(p1, p2);
        }
    }
}
