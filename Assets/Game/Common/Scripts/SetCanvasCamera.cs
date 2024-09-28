using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class SetCanvasCamera : MonoBehaviour
{
    private Canvas _canvas;

    private void OnEnable()
    {
        _canvas ??= GetComponent<Canvas>();
        _canvas.worldCamera ??= Camera.main;
    }
}
