using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PutBlockGameRevivePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cutDownTmp;
    [SerializeField] private Button reviveBtn;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image cutDownProgress;

    private Sequence showAnimSeq;

    private Action OnRevive;
    
    private void Awake()
    {
        reviveBtn.onClick.AddListener(() =>
        {
            showAnimSeq?.Pause();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    BaseUtilities.PlayCommonClick();
                    OnRevive?.Invoke();
                    showAnimSeq?.Kill();
                    gameObject.SetActive(false);
                }
                else
                {
                    showAnimSeq?.Play();
                }
            });
        });
    }

    public void Show(Action onRevive, Action onGiveUp)
    {
        gameObject.SetActive(true);
        cutDownTmp.text = "5";
        cutDownProgress.fillAmount = 1;
        OnRevive = onRevive;
        canvasGroup.alpha = 0;
        showAnimSeq?.Kill();
        reviveBtn.interactable = false;
        showAnimSeq = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, 0.2f))
            .AppendCallback(() =>
            {
                reviveBtn.interactable = true;
            })
            .AppendInterval(0.8f)
            .AppendCallback(() =>
            {
                cutDownTmp.text = "4";
            })
            .AppendInterval(1)
            .AppendCallback(() =>
            {
                cutDownTmp.text = "3";
            })
            .AppendInterval(1)
            .AppendCallback(() =>
            {
                cutDownTmp.text = "2";
            })
            .AppendInterval(1)
            .AppendCallback(() =>
            {
                cutDownTmp.text = "1";
            })
            .AppendInterval(1)
            .AppendCallback(() =>
            {
                cutDownTmp.text = "0";
            })
            .AppendInterval(0.3f)
            .SetLink(gameObject)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onGiveUp?.Invoke();
            });

        showAnimSeq.Insert(0, DOTween.To(val =>
        {
            cutDownProgress.fillAmount = val;
        }, 1, 0, 5.3f));
    }
}
