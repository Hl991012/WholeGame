using System;
using GameFrame;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyFactroy : MonoSingleton<EnemyFactroy>
{
    private void Awake()
    {
        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            GenEnemy();   
            // if (Random.Range(0, 10) > 5)
            // {
            //     GenEnemy();
            // }
        }).AddTo(this);
    }

    private void GenEnemy()
    {
        var enemy = GamingPools.Instance.GetFromPool<Enemy>(RecyclableType.Enemy);
        enemy.transform.position = new Vector3(Random.Range(-2f, 2f), 6, 0);
        var enemyModel = new EnemyModel()
        {
            Hp = 100,
            AttackDamage = 10,
            AttackInterval = 5f,
            AttackRange = 2f,
            MoveSpeed = 1f,
        };
        enemy.Init(enemyModel);
    }
}
