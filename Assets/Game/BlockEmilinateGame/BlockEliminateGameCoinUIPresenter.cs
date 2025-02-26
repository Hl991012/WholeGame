using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BlockEliminateGameCoinUIPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCountTmp;
    [SerializeField] private Button addBtn;
    [SerializeField] private Image coinPrefab;
    [SerializeField] private Transform flyEndPos;
    [SerializeField] private Transform flyStartPos;

    public bool needShowIncreaseAnim;
    public bool isCameraMode;
    
    private int showCoinCount;
    private bool isPlayingAnim;

    public void PlayCollectAnim(int addCoinCount, Action onComplete)
    {
        isPlayingAnim = true;
        showCoinCount = CurrencyManager.Coin - addCoinCount;
        coinCountTmp.text = showCoinCount.ToString();
        // 展示动画
        var tempSeq = DOTween.Sequence();
        // 生成8个图标飞向入口
        var tempObj = new GameObject();
        tempObj.transform.SetParent(transform);
        tempObj.transform.localScale = Vector3.one;
        tempObj.transform.localPosition = new Vector2(-121f, 1.5f);
        var singleCount = addCoinCount / 8;

        var range = isCameraMode ? 0.7f : 80f;
        
        for (var i = 0; i < 8; i++)
        {
            var rectTrans = Instantiate(coinPrefab, tempObj.transform).GetComponent<RectTransform>();
            rectTrans.localScale = Vector2.zero;
            rectTrans.gameObject.SetActive(true);
            rectTrans.position = flyStartPos.position;
            var index = i;
            tempSeq.Insert(0.1f * i, rectTrans.DOScale(0.4f, 0.1f));
            tempSeq.Insert(0.1f * i, rectTrans.DOMove(rectTrans.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0), 0.1f));
            tempSeq.Insert(0.1f * (5 + index), rectTrans.DOMove(flyEndPos.position, 0.4f));
            tempSeq.Insert(0.1f * (5 + index), rectTrans.DOScale(0.31f, 0.4f));
            tempSeq.InsertCallback(0.1f * (5 + index) + 0.4f, () =>
            {
                if (index == 7)
                {
                    showCoinCount = CurrencyManager.Coin;
                }
                else
                {
                    showCoinCount += singleCount;
                }
                coinCountTmp.text = showCoinCount.ToString();
            });
        }

        tempSeq.AppendInterval(0.15f)
            .SetLink(gameObject)
            .SetUpdate(true)
            .onComplete += () =>
        {
            DestroyImmediate(tempObj);
            isPlayingAnim = false;
            onComplete?.Invoke();
        };
    }

    private void Awake()
    {
        addBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            
        });
    }

    private void RefreshView()
    {
        if (!needShowIncreaseAnim && !isPlayingAnim)
        {
            coinCountTmp.text = CurrencyManager.Coin.ToString();
        }
        else
        {
            if (showCoinCount > CurrencyManager.Coin)
            {
                coinCountTmp.text = CurrencyManager.Coin.ToString();
            }
        }
    }

    private void OnEnable()
    {
        CurrencyManager.OnWealthChanged += RefreshView;
        coinCountTmp.text = CurrencyManager.Coin.ToString();
        showCoinCount = CurrencyManager.Coin;
    }

    private void OnDisable()
    {
        CurrencyManager.OnWealthChanged -= RefreshView;
    }
}
