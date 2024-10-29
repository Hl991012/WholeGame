using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SingleBlockItem : MonoBehaviour
{
    [SerializeField] private GameObject colliderObj;
    [SerializeField] private Image icon;

    public int ColorIndex { get; private set; }

    public int ShapeIndex { get; private set; }

    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetRotation(Vector3 euler)
    {
        transform.rotation = Quaternion.Euler(euler);
    }

    public void SetColor(Color color, int colorIndex, int shapeIndex)
    {
        icon.color = color;
        ColorIndex = colorIndex;
        ShapeIndex = shapeIndex;
    }

    private Sequence moveAnim;
    public void MoveTo(Vector3 targetPos, Action onComplete)
    {
        moveAnim = DOTween.Sequence()
            .SetLink(gameObject)
            .SetUpdate(true);
        moveAnim.Append(transform.DOMove(targetPos, 0.2f))
            .Join(transform.DORotate(Vector3.zero, 0.2f))
            .Join(transform.DOScale(1, 0.2f))
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    public void SetBlockInteractable(bool interactable)
    {
        colliderObj.SetActive(interactable);
    }
}
