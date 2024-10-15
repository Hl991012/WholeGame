using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboUIPresenter : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform comboRectTrans;
    [SerializeField] private TextMeshProUGUI comboCount;
    [SerializeField] private TextMeshProUGUI addScoreTmp;
        
    private Sequence comboAnimSeq;
    
    private void OnEnable()
    {
        canvasGroup.alpha = 0;
    }

    private void Start()
    {
        ComboManager.Instance.OnComboStateChanged += OnComboStateChanged;
    }

    private void OnComboStateChanged(GameType gameType, ComboStateModel comboStateModel, Vector3 pos, int addScore)
    {
        var currentComboCount = comboStateModel.ComboCount;
        comboRectTrans.gameObject.SetActive(currentComboCount > 0);
        comboCount.gameObject.SetActive(currentComboCount > 1);
        if (currentComboCount > 1)
        {
            comboCount.text = $"+{currentComboCount}";
        }

        if (currentComboCount > 0)
        {
            addScoreTmp.text = addScore <= 0 ? "" : $"+{addScore}";
            SetComboPos(pos);
            PlayComboAnim();   
        }
    }

    private void SetComboPos(Vector2 pos)
    {
        comboRectTrans.transform.position = pos;
        
        // 如果超出边缘，则偏移一些
        var halfScreenWidth = Screen.width / (Screen.height / 1560f) / 2f;
        var temp = comboCount.gameObject.activeSelf ? 230 : 159;
        if (halfScreenWidth - temp < comboRectTrans.anchoredPosition.x)
        {
            comboRectTrans.anchoredPosition = new Vector2(halfScreenWidth - temp, comboRectTrans.anchoredPosition.y);
        }

        if (comboRectTrans.anchoredPosition.x < temp - halfScreenWidth)
        {
            comboRectTrans.anchoredPosition = new Vector2(temp - halfScreenWidth, comboRectTrans.anchoredPosition.y);
        }
    }

    private void PlayComboAnim()
    {
        comboAnimSeq?.Kill();
        canvasGroup.alpha = 0;
        addScoreTmp.alpha = 0;
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

    private void OnDestroy()
    {
        ComboManager.Instance.OnComboStateChanged -= OnComboStateChanged;
    }
}
