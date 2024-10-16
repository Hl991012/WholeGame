using DG.Tweening;
using TMPro;
using UnityEngine;

public class SingleCombo : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform comboRectTrans;
    [SerializeField] private TextMeshProUGUI comboCountTmp;
    [SerializeField] private TextMeshProUGUI addScoreTmp;
        
    private Sequence comboAnimSeq;
    
    private void OnEnable()
    {
        canvasGroup.alpha = 0;
    }
    
    public void Show(int comboCount, int addScore)
    {
        comboCountTmp.text = $"+{comboCount.ToString()}";
        addScoreTmp.text = $"+{addScore}";
        canvasGroup.alpha = 0;
        addScoreTmp.alpha = 0;
        comboRectTrans.anchoredPosition = Vector2.zero;
        
        comboCountTmp.gameObject.SetActive(comboCount > 1);
        
        comboAnimSeq?.Kill();
        comboAnimSeq = DOTween.Sequence();
        comboAnimSeq.SetLink(gameObject)
            .SetUpdate(true)
            .onComplete += () =>
        {
            canvasGroup.alpha = 0;
            addScoreTmp.alpha = 0;
            
        };
        comboAnimSeq
            .Append(canvasGroup.DOFade(1, 0.3f))
            .Join(comboRectTrans.DOAnchorPosY(-80, 0.7f))
            .Insert(0.4f, canvasGroup.DOFade(0, 0.3f))
            .AppendCallback(() =>
            {
                addScoreTmp.alpha = 1;
            })
            .AppendInterval(1f);
    }
}
