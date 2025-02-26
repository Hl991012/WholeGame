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
    [SerializeField] private Button sureBtn;

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
            CircleGameManager.Instance.StartGame();
            startGame.gameObject.SetActive(false);
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
        
        sureBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            CircleGameManager.Instance.EndGame();
            revivePanel.gameObject.SetActive(false);
            startGame.gameObject.SetActive(true);
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
        reviveBtn.gameObject.SetActive(true);
        revivePanel.SetActive(true);
        closeReviveBtn.gameObject.SetActive(true);
        sureBtn.gameObject.SetActive(false);
    }

    public void ShowGameOverPanel()
    {
        reviveBtn.gameObject.SetActive(false);
        revivePanel.SetActive(true);
        closeReviveBtn.gameObject.SetActive(false);
        sureBtn.gameObject.SetActive(true);
    }
}
