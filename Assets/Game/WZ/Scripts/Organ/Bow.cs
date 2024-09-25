using UnityEngine;


public class Bow : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    public float attackInterval;

    private float timer = 0;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > attackInterval)
        {
            Fire();
            timer = 0;
        }
    }

    private void Fire()
    {
        // 对象池
        var obj = Instantiate(arrow, transform);
        obj.SetActive(true);
        obj.transform.localPosition = Vector3.zero;
    }
}
