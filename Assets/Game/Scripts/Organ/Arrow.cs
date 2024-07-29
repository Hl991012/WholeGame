using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float moveSpeed;
    
    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") && other.transform != transform.parent)
        {
            Destroy(gameObject);
        }
    }
}
