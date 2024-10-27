using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SingleBlockItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    public void SetPos(Vector3 pos, bool needShowAnim)
    {
        if (!needShowAnim)
        {
            transform.position = pos;   
        }
    }

    public void SetRotation(Vector3 euler)
    {
        transform.rotation = Quaternion.Euler(euler);
    }

    public void SetColor(Color color)
    {
        icon.color = color;
    }
}
