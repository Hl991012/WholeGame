using GameFrame;
using UnityEngine;


[CreateAssetMenu(fileName = nameof(PrefabSoCenter), menuName = "SO/" + nameof(PrefabSoCenter))]
public class PrefabSoCenter : ScriptableObject
{
    public Bullet bulletPrefab;
    public Bullet subBulletPrefab;
    public Enemy enemyPrefab;

    public GameObject GetPrefabByRecyclableType(RecyclableType recyclableType)
    {
        switch (recyclableType)
        {
            case RecyclableType.NormalBullet:
            case RecyclableType.NormalSubBullet:
                return bulletPrefab.gameObject;
        }

        return null;
    }
}
