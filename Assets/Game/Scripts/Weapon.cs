using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected Transform firePos;
    
    protected WeaponModel WeaponModel { get;  set; }

    protected float nextFireTime;

    protected abstract void Fire();

    public void Init(WeaponModel weaponModel)
    {
        WeaponModel = weaponModel;
    }
    
    private void Update()
    {
        if (Time.realtimeSinceStartup > nextFireTime)
        {
            Fire();
        }
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

public class WeaponModel
{
    // 枪类型
    public WeaponType WeaponType { get; set; }

    // 攻击间隔
    public float BaseAttackInterval { get; set; }

    // 基础伤害
    public int BaseAttackPower { get; set; }
    
    // 基础攻击范围
    public float BaseAttackRange { get; set; }
}

public enum WeaponType
{
    PelletGun, // 弹丸枪
    LaserGun, // 激光枪
}
