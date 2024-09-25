using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SingleStarItem : MonoBehaviour
{
    private Image starImg;
    private Sequence starAnim;

    private void Awake()
    {
        starImg = transform.GetComponent<Image>();
    }

    public void RefreshView(bool visible, bool needShowAnim, float animDelay = 0)
    {
        starImg.color = visible ? Color.yellow : Color.gray;
        
        // starAnim?.Kill();
        // if (visible && needShowAnim)
        // {
        //     transform.localScale = Vector3.one * 5;
        //     starAnim = DOTween.Sequence()
        //         .AppendInterval(animDelay)
        //         .Append(transform.DOScale(1, 0.6f))
        //         .SetUpdate(true)
        //         .SetLink(gameObject)
        //         .SetEase(Ease.Linear);
        // }
    }
}
