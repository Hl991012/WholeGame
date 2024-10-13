using DG.Tweening;
using NMNH.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawLineGameOverPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hintTmp;
    [SerializeField] private Button continueBtn;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private string[] hints;
    
    private Sequence showAnimSeq;
    
    private void Awake()
    {
        continueBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            DrawLineGameConctrol.Instance.DrawLineGameRoomPresenter.StartGame();
            canvasGroup.alpha = 1;
            showAnimSeq?.Kill();
            showAnimSeq = DOTween.Sequence()
                .Append(canvasGroup.DOFade(0, 0.5f))
                .SetLink(gameObject)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        });

    }
    
    public void Show()
    {
        hintTmp.text = hints[Random.Range(0, hints.Length)];
        AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Win);
        canvasGroup.alpha = 0;
        showAnimSeq?.Kill();
        showAnimSeq = DOTween.Sequence()
            .AppendInterval(0.3f)
            .Append(canvasGroup.DOFade(1, 0.7f))
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                DrawLineGameConctrol.Instance.DrawLineGameRoomPresenter.StartGame();
            })
            .SetLink(gameObject)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                WXSDKManager.Instance.ShowInterstitialVideo(null);
                gameObject.SetActive(false);
            });
    }
}
