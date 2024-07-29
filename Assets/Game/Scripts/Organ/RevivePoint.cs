using UnityEngine;

public class RevivePoint : MonoBehaviour
{
    private void OnEnable()
    {
        transform.GetComponent<SpriteRenderer>().color = Color.clear;
    }
}
