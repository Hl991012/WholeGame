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
}
