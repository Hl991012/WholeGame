using System;
using BlockEliminateGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleBoosterItem : MonoBehaviour
{
    [SerializeField] private Button clickBtn;
    [SerializeField] private GameObject addObj;
    [SerializeField] private TextMeshProUGUI countTmp;
    [SerializeField] private BoosterType boosterType;
    
    public BoosterType BoosterType => boosterType;

    private void Awake()
    {
        clickBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();

            var boosterQuality = BlockEliminateStageManager.Instance.GetBoosterQuantity(boosterType);

            if (boosterQuality > 0)
            {
                BlockEliminateStageManager.Instance.UseBooster(boosterType);
            }
            else
            {
                var getCount = boosterType switch
                {
                    BoosterType.Refresh => 2,
                    _ => 1,
                };
                // 打开购买弹窗
                MainSceneCenter.Instance.BuyGoodsUIPresenter.Show(BlockEliminateStageManager.Instance.GetBoosterPrice(boosterType), new GameModel.RewardModel()
                {
                    boosterType = boosterType,
                    quantity = getCount,
                    type = GameModel.RewardModel.RewardType.Booster
                });
            }
        });

        BlockEliminateStageManager.Instance.OnBoosterChanged += RefreshView;
    }

    private void OnEnable()
    {
        RefreshView();
    }

    private void RefreshView()
    {
        // 刷新个数
        var boosterQuality = BlockEliminateStageManager.Instance.GetBoosterQuantity(boosterType);
        addObj.SetActive(boosterQuality <= 0);
        countTmp.text = $"x{boosterQuality}";
    }

    public void Click()
    {
        clickBtn.onClick?.Invoke();
    }
}
