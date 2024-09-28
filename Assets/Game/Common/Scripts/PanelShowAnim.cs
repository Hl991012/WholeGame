using System;
using DG.Tweening;
using UnityEngine;

public class PanelShowAnim : MonoBehaviour
{
    private Sequence showAnimSeq;

    private CanvasGroup canvasGroup;
    
    private void OnEnable()
    {
        TryGetComponent(out canvasGroup);
        canvasGroup ??= gameObject.AddComponent<CanvasGroup>();
        PlayShowAnim();
    }

    private void PlayShowAnim()
    {
        if(canvasGroup == null) return;
        showAnimSeq?.Kill();
        canvasGroup.alpha = 0.8f;
        canvasGroup.interactable = false;
        transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        showAnimSeq = DOTween.Sequence()
            .Append(transform.DOScale(1.01f, 0.1f).SetEase(Ease.Linear))
            .Append(transform.DOScale(1f, 0.05f).SetEase(Ease.Linear))
            .Insert(0,canvasGroup.DOFade(1f, 0.05f).SetEase(Ease.Linear))
            .SetLink(gameObject)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
            });   
    }

    private void OnDisable()
    {
        showAnimSeq?.Kill();
        showAnimSeq = null;
    }
}
