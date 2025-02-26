using BlockEliminateGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyGoodsUIPresenter : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buyByCoinBtn;
    [SerializeField] private Button buyByAdBtn;
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI priceTmp;

    private int curPrice;

    private GameModel.RewardModel curRewardModel;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            gameObject.SetActive(false);
        });
        
        buyByCoinBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            if (CurrencyManager.Coin < curPrice)
            {
                MainSceneCenter.Instance.ShowTips("金币不足");
                return;
            }

            if (CurrencyManager.CostCoin(curPrice))
            {
                BaseUtilities.DealReward(curRewardModel);
                gameObject.SetActive(false);
            }
        });
        
        buyByAdBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    BaseUtilities.DealReward(curRewardModel);
                    gameObject.SetActive(false);
                }
            });
        });
    }

    public void Show(int price, GameModel.RewardModel rewardModel)
    {
        if(rewardModel == null) return;

        gameObject.SetActive(true);
        
        curPrice = price;
        curRewardModel = rewardModel;

        iconImg.sprite = ResourcesCenter.Instance.GetRewardIcon(rewardModel);

        priceTmp.text = price.ToString();
    }
}
