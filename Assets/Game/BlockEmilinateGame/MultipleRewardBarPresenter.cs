using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class MultipleRewardBarPresenter: MonoBehaviour
{
    [SerializeField] private RectTransform cursorRectTrans;

    [NonSerialized] public ReactiveProperty<int> CurrentRewardMultiple = new (1);

    private Sequence seq;
        
    private void OnEnable()
    {
        ResetAnimation();
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    public void PauseAnimation()
    {
        seq?.Pause();
    }

    public void PlayAnimation()
    {
        seq?.Play();
    }
        
    private void ResetAnimation()
    {
        cursorRectTrans.anchoredPosition = new Vector2(0, 50f);
        seq?.Kill();
        seq = DOTween.Sequence()
            .Append(cursorRectTrans.DOAnchorPosX(250f, 0.5f).SetEase(Ease.Linear))
            .Append(cursorRectTrans.DOAnchorPosX(-250f, 1).SetEase(Ease.Linear))
            .Append(cursorRectTrans.DOAnchorPosX(0, 0.5f).SetEase(Ease.Linear))
            .SetUpdate(true)
            .SetLink(gameObject)
            .SetLoops(-1);
        seq.onUpdate += () =>
        {
            var tempMultiple = Math.Abs(cursorRectTrans.anchoredPosition.x) switch
            {
                < 52f => 5,
                < 154f => 3,
                _ => 2,
            };

            if (CurrentRewardMultiple.Value != tempMultiple)
            {
                CurrentRewardMultiple.Value = tempMultiple;
            }
        };
    }

    private void StopAnimation()
    {
        seq?.Kill();
        cursorRectTrans.anchoredPosition = Vector2.zero;
        CurrentRewardMultiple.Value = 1;
    }
}