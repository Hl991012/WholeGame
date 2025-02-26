using BlockEliminateGame;
using DG.Tweening;
using PuzzleGame;
using PuzzleGame.Gameplay;
using PuzzleGame.Gameplay.Puzzle1010;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PutBlockGameOverPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curScoreTmp;
    [SerializeField] private Button onceAgainBtn;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button shareBtn;
    [SerializeField] private SingleRewardItem[] rewardItems;
    
    private Sequence showAnimSeq;

    private GameModel.RewardModel curRewardModel;
    
    private void Awake()
    {
        onceAgainBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            PutBlockGameController.Instance.ReplayGame();
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
            
            MainSceneCenter.Instance.GetRewardUIPresenter.Init(curRewardModel).Show();
        });
        
        shareBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.Share();
        });
    }

    public void Show(GameModel.RewardModel rewardModel)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        showAnimSeq?.Kill();
        onceAgainBtn.interactable = false;
        showAnimSeq = DOTween.Sequence()
            .AppendInterval(0.5f)
            .Append(canvasGroup.DOFade(1, 0.7f))
            .SetLink(gameObject)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                onceAgainBtn.interactable = true;
            });
        
        curScoreTmp.text = UserProgress.Current.GetGameState<GameStateBaseModel>(UserProgress.Current.CurrentGameId).Score
            .ToString();

        curRewardModel = rewardModel;
        
        // 刷新奖励
        for (var i = 0; i < rewardItems.Length; i++)
        {
            rewardItems[i].gameObject.SetActive(i < 1);

            if (i < 1)
            {
                rewardItems[i].RefreshView(rewardModel);
            }
        }
    }
}
