using DG.Tweening;
using UnityEngine;

public class SingleCircleItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] sprites;
    [SerializeField] private Collider2D[] colliders;
    [SerializeField] private int totalCount;

    private void Awake()
    {
        RemainCount = totalCount;
    }

    public int RemainCount { get; private set; }

    public bool OnBeAttack()
    {
        RemainCount--;
        return RemainCount <= 0;
    }

    public void SetPreViewState()
    {
        foreach (var item in sprites)
        {
            item.color = new Color(1, 1, 1, 0.2f);
        }

        foreach (var item in colliders)
        {
            item.enabled = false;
        }

        transform.localScale = Vector3.one * 1.35f;
    }

    public void SetNormalState(bool needAnim)
    {
        if (needAnim)
        {
            var tempSeq = DOTween.Sequence()
                .SetLink(gameObject)
                .SetUpdate(true);
            
            foreach (var item in sprites)
            {
                tempSeq.Join(item.DOColor(new Color(1, 1, 1, 1), 0.2f));
            }
            
            tempSeq.Join(transform.DOScale(Vector3.one, 0.2f));
        }
        else
        {
            foreach (var item in sprites)
            {
                item.color = new Color(1, 1, 1, 1);
            }   
            transform.localScale = Vector3.one;
        }
        
        foreach (var item in colliders)
        {
            item.enabled = true;
        }
    }
}
