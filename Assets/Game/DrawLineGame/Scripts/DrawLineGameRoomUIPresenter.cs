using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawLineGameRoomUIPresenter : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private Button helpBtn;
    [SerializeField] private TextMeshProUGUI helpBtnRemainCount;
    [SerializeField] private TextMeshProUGUI adBuyBoosterCountTmp;
    [SerializeField] private GameObject watchAdObj;
    [SerializeField] private GameObject countShowObj;

    private void Awake()
    {
        DrawLineGameConctrol.Instance.Register(this);
        
        backBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
        });
        
        helpBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            if(BoosterManager.Instance.UseBooster(GameType.DrawLineGame, BoosterType.Help))
            {
                DrawLineGameConctrol.Instance.DrawLineGameRoomPresenter.UseHelpBooster();    
            }
            else
            {
                WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
                {
                    if (isSuccess)
                    {
                        BoosterManager.Instance.BuyBooster(GameType.DrawLineGame, BoosterType.Help);
                    }
                });
            }
        });

        BoosterManager.Instance.OnBoosterChanged += OnBoosterChanged;
    }

    private void Start()
    {
        RefreshView();
    }

    private void RefreshView()
    {
        var helpBoosterCount = BoosterManager.Instance.GetBoosterCount(GameType.DrawLineGame, BoosterType.Help);
        watchAdObj.SetActive(helpBoosterCount <= 0);
        countShowObj.SetActive(helpBoosterCount > 0);
        if (helpBoosterCount > 0)
        {
            helpBtnRemainCount.text = $"{helpBoosterCount}";
        }
        else
        {
            var tempBoosterConfig = AllBoosterConfigManager.Instance.GetBoosterConfig(BoosterType.Help);
            var tempAddCount = tempBoosterConfig != null ? tempBoosterConfig.CountToBuy : 1;
            adBuyBoosterCountTmp.text = $"+{tempAddCount}";
        }
    }

    private void OnBoosterChanged(GameType gameType, BoosterType boosterType, int count)
    {
        if (gameType == GameType.DrawLineGame)
        {
            RefreshView();   
        }
    }
}
