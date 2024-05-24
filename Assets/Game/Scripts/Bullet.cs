using System;
using GameFrame;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour, IRecyclable
{
    public bool isSubBullet;
    public float speed;
    private Enemy ignoreEnemy; // 忽略的怪物
    
    public int genSubBulletCount = 2; // 生成sub子弹的数量
    public int Penetration = 2; // 穿透力

    private bool explode = true;
    
    void Update()
    {
        transform.Translate(Vector3.up * (Time.deltaTime * speed));
    }

    /// <summary>
    /// 子弹的分裂逻辑
    /// </summary>
    /// <param name="enemy"></param>
    private void Split(Enemy enemy)
    {
        for (var i = 0; i < genSubBulletCount; i++)
        {
            var bullet = GamingPools.Instance.GetFromPool<Bullet>(RecyclableType.NormalSubBullet);
            bullet.gameObject.SetActive(true);
            bullet.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            bullet.SetIgnoreEnemy(enemy);
            bullet.isSubBullet = true;
        }
    }

    readonly RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[5];
    /// <summary>
    /// 子弹的爆炸逻辑
    /// </summary>
    private void Explode()
    {
        Array.Clear(raycastHit2Ds, 0, raycastHit2Ds.Length);
        if (Physics2D.CircleCastNonAlloc(transform.position, 2, Vector2.up, raycastHit2Ds, 0, LayerMask.GetMask("Enemy")) > 0)
        {
            foreach (var item in raycastHit2Ds)
            {
                if (item.transform != null)
                {
                    // Debug.LogError(item.transform.name);
                }
            }   
        }
    }

    private void SetIgnoreEnemy(Enemy enemy)
    {
        ignoreEnemy = enemy;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != ignoreEnemy)
            {
                enemy.BeAttacked(10);
                Penetration--;
                if (Penetration <= 0)
                {
                    GamingPools.Instance.ReturnToPool<Bullet>(gameObject);
                }
                if (!isSubBullet)
                {
                    Split(enemy);
                    
                    if (explode)
                    {
                        Explode();
                    }
                }
            }
        }
    }

    public RecyclableType RecyclableType => isSubBullet ? RecyclableType.NormalBullet : RecyclableType.NormalSubBullet;
    public void OnGet()
    {
        gameObject.SetActive(true);
        Penetration = 2;
    }

    public void OnReturn()
    {
        gameObject.SetActive(false);
    }
}
