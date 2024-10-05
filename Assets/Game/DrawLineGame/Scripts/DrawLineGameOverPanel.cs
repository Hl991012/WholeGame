using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DrawLineGameOverPanel : MonoBehaviour
{
    [SerializeField] private Button continueBtn;
    [SerializeField] private CanvasGroup canvasGroup;
    
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
        canvasGroup.alpha = 0;
        showAnimSeq?.Kill();
        continueBtn.interactable = false;
        showAnimSeq = DOTween.Sequence()
            .AppendInterval(0.6f)
            .Append(canvasGroup.DOFade(1, 0.7f))
            .SetLink(gameObject)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                continueBtn.interactable = true;
            });
    }
}
