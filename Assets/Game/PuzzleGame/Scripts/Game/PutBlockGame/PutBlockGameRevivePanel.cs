using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PutBlockGameRevivePanel : MonoBehaviour
{
    [SerializeField] private Button reviveBtn;
    [SerializeField] private Button abandonBtn;
    // [SerializeField] private CanvasGroup canvasGroup;

    private Sequence showAnimSeq;

    private Action OnRevive;
    
    private Action OnGiveUp;
    
    private void Awake()
    {
        reviveBtn.onClick.AddListener(() =>
        {
            // showAnimSeq?.Pause();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    BaseUtilities.PlayCommonClick();
                    OnRevive?.Invoke();
                    // showAnimSeq?.Kill();
                    gameObject.SetActive(false);
                }
                else
                {
                    // showAnimSeq?.Play();
                }
            });
        });
        
        abandonBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            gameObject.SetActive(false);
            OnGiveUp?.Invoke();
            WXSDKManager.Instance.ShowInterstitialVideo(null);
        });
    }

    public void Show(Action onRevive, Action onGiveUp)
    {
        // countDownObj.SetActive(true);
        gameObject.SetActive(true);
        // cutDownTmp.text = "5";
        // cutDownProgress.fillAmount = 1;
        OnRevive = onRevive;
        OnGiveUp = onGiveUp;
        // canvasGroup.alpha = 0;
        // showAnimSeq?.Kill();
        // reviveBtn.interactable = false;
        // abandonBtn.gameObject.SetActive(false);
        // showAnimSeq = DOTween.Sequence()
        //     .AppendInterval(0.6f)
        //     .Append(canvasGroup.DOFade(1, 0.2f))
        //     .AppendCallback(() =>
        //     {
        //         reviveBtn.interactable = true;
        //     })
        //     .AppendInterval(0.8f)
        //     .AppendCallback(() =>
        //     {
        //         cutDownTmp.text = "4";
        //     })
        //     .AppendInterval(1)
        //     .AppendCallback(() =>
        //     {
        //         cutDownTmp.text = "3";
        //     })
        //     .AppendInterval(1)
        //     .AppendCallback(() =>
        //     {
        //         cutDownTmp.text = "2";
        //     })
        //     .AppendInterval(1)
        //     .AppendCallback(() =>
        //     {
        //         cutDownTmp.text = "1";
        //     })
        //     .AppendInterval(1)
        //     .AppendCallback(() =>
        //     {
        //         cutDownTmp.text = "0";
        //     })
        //     .AppendInterval(0.3f)
        //     .SetLink(gameObject)
        //     .SetUpdate(true)
        //     .OnComplete(() =>
        //     {
        //         abandonBtn.gameObject.SetActive(true);
        //         countDownObj.SetActive(false);
        //     });
        //
        // showAnimSeq.Insert(0, DOTween.To(val =>
        // {
        //     cutDownProgress.fillAmount = val;
        // }, 1, 0, 5.3f));
    }
}
