using GameFrame;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour, IRecyclable
{
    public bool isSubBullet;
    public float speed;
    private Enemy ignoreEnemy;
    
    public int genSubBulletCount = 2;
    
    void Update()
    {
        transform.Translate(Vector3.up * (Time.deltaTime * speed));
    }

    private void FenLie(Enemy enemy)
    {
        for (var i = 0; i < genSubBulletCount; i++)
        {
            var bullet = GamingPools.Instance.GetFromPool<Bullet>(RecyclableType.NormalSubBullet);
            bullet.gameObject.SetActive(true);
            bullet.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            bullet.SetIgnoreEnemy(enemy);
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
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != ignoreEnemy)
            {
                enemy.BeAttacked(10);
                gameObject.SetActive(false);
                FenLie(enemy);
            }
        }
    }

    public RecyclableType RecyclableType => isSubBullet ? RecyclableType.NormalBullet : RecyclableType.NormalSubBullet;
    public void OnGet()
    {
        gameObject.SetActive(true);
    }

    public void OnReturn()
    {
        gameObject.SetActive(false);
    }
}
