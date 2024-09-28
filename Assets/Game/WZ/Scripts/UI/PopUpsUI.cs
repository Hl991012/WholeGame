using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopUpsUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI hintTextTmp;

    private Sequence anim;
    
    public void Show(string des)
    {
        anim?.Kill();
        hintTextTmp.text = des;
        canvasGroup.alpha = 0;
        anim = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, 0.2f))
            .AppendInterval(0.4f)
            .Append(canvasGroup.DOFade(0, 0.2f))
            .SetLink(gameObject)
            .SetUpdate(true);
    }
}
