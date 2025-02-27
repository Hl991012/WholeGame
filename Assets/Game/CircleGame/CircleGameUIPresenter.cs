using BlockEliminateGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CircleGameUIPresenter : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private Button startGame;
    [SerializeField] private TextMeshProUGUI curScoreTmp;
    [SerializeField] private TextMeshProUGUI maxScoreTmp;
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private Button closeReviveBtn;
    [SerializeField] private Button reviveBtn;
    [SerializeField] private Button leaderBoardBtn;

    #region 游戏结算相关内容

    [SerializeField] private GameObject gameResultPanel;
    [SerializeField] private TextMeshProUGUI gameScoreTmp;
    [SerializeField] private SingleRewardItem rewardItem;
    [SerializeField] private Button normalGetBtn;
    [SerializeField] private Button adGetBtn;
    [SerializeField] private MultipleRewardBarPresenter multipleRewardBarPresenter;

    #endregion

    private void Awake()
    {
        backBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
            WXSDKManager.Instance.ShowInterstitialVideo(null);
            WXSDKManager.Instance.ShowCustomAd1();
        });
        
        startGame.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            
            // 判断金币是否足够，如果足够则开始游戏
            if (PlayerPrefs.GetInt("circle_game_play_count") == 0 || CurrencyManager.Coin >= 50)
            {
                CircleGameManager.Instance.StartGame();
                startGame.gameObject.SetActive(false);
            }
            else
            {
                MainSceneCenter.Instance.ShowTips("金币不足，请游玩关卡或者无尽模式获得金币！");
            }
        });
        
        closeReviveBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            revivePanel.gameObject.SetActive(false);
            startGame.gameObject.SetActive(true);
            CircleGameManager.Instance.EndGame();
        });
        
        reviveBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                CircleGameManager.Instance.Revive();
                revivePanel.gameObject.SetActive(false);
            });
        });
        
        leaderBoardBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            
        });
        
        normalGetBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            CircleGameManager.Instance.EndGame();
            gameResultPanel.gameObject.SetActive(false);
            startGame.gameObject.SetActive(true);
            // 增加金币
            CurrencyManager.AddCoin(CircleGameManager.Instance.CalculateRewardCoinCount());
        });
        
        adGetBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            multipleRewardBarPresenter.PauseAnimation();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    CircleGameManager.Instance.EndGame();
                    gameResultPanel.gameObject.SetActive(false);
                    startGame.gameObject.SetActive(true);
                    // 增加金币
                    var canGetCoinCount = CircleGameManager.Instance.CalculateRewardCoinCount() *
                                          multipleRewardBarPresenter.CurrentRewardMultiple.Value;
                    CurrencyManager.AddCoin(canGetCoinCount);
                }
                else
                {
                    multipleRewardBarPresenter.PlayAnimation();
                }
            });
        });

        CircleGameManager.Instance.OnScoreChanged += OnScoreChanged;
    }

    private void Start()
    {
        OnScoreChanged();
    }

    private void OnScoreChanged()
    {
        curScoreTmp.text = CircleGameManager.Instance.CurScore.ToString();
        maxScoreTmp.text = CircleGameManager.Instance.MaxScore.ToString();
    }

    public void ShowRevivePanel()
    {
        revivePanel.SetActive(true);
    }

    public void ShowGameOverPanel()
    {
        revivePanel.SetActive(false);
        gameResultPanel.SetActive(true);
        gameScoreTmp.text = CircleGameManager.Instance.CurScore.ToString();
        // 计算当前能获得的金币
        GameModel.RewardModel rewardModel = new GameModel.RewardModel()
        {
            quantity = CircleGameManager.Instance.CalculateRewardCoinCount()
        };
        rewardItem.RefreshView(rewardModel);
    }
}
