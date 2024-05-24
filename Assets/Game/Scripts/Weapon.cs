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
