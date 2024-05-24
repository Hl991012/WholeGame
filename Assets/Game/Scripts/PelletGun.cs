using GameFrame;
using UnityEngine;

public class PelletGun : Weapon
{
    protected override void Fire()
    {
        nextFireTime += WeaponModel.BaseAttackInterval;
        var bullet = GamingPools.Instance.GetFromPool<Bullet>(RecyclableType.NormalBullet);
        bullet.transform.position = firePos.position;
        bullet.isSubBullet = false;
    }

    private void OnDrawGizmos()
    {
        if (WeaponModel != null)
        {
            Gizmos.color = Color.red;
            Vector3 center = transform.position; // 圆心位置，这里假设是该脚本挂载的游戏对象的位置

            for (int i = 0; i < 32; i++)
            {
                var angle = i * (360f / 32) * Mathf.Deg2Rad;
                var p1 = new Vector3(center.x + WeaponModel.BaseAttackRange * Mathf.Cos(angle), center.y + WeaponModel.BaseAttackRange * Mathf.Sin(angle), center.z);
                var p2 = new Vector3(center.x + WeaponModel.BaseAttackRange * Mathf.Cos(angle + (360f / 32) * Mathf.Deg2Rad), center.y + WeaponModel.BaseAttackRange * Mathf.Sin(angle + (360f / 32) * Mathf.Deg2Rad), center.z);
                Gizmos.DrawLine(p1, p2);
            }   
        }
    }
}
