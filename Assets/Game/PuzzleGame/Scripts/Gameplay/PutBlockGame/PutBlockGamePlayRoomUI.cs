using System;
using PuzzleGame;
using PuzzleGame.Gameplay.Puzzle1010;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PutBlockGamePlayRoomUI : MonoBehaviour
{
    [SerializeField] private PutBlockGameController putBlockGameController;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button addPutAreaBtn;
    [SerializeField] private GameObject addPutAreaAdIcon;
    
    [SerializeField] private Button refreshBoosterBtn;
    [SerializeField] private GameObject refreshBoosterWatchAdObj;
    [SerializeField] private GameObject refreshBoosterRemainShowObj;
    [SerializeField] private TextMeshProUGUI refreshBoosterRemainCountTmp;
    [SerializeField] private TextMeshProUGUI adAddRefreshBoosterCountTmp;
    [SerializeField] private TextMeshProUGUI refreshBoosterUserCountTmp;
    
    [SerializeField] private Button undoBoosterBtn;
    [SerializeField] private GameObject undoBoosterWatchAdObj;
    [SerializeField] private GameObject undoBoosterRemainShowObj;
    [SerializeField] private TextMeshProUGUI undoBoosterRemainCountTmp;
    [SerializeField] private TextMeshProUGUI adAddUndoBoosterCountTmp;
    [SerializeField] private TextMeshProUGUI undoBoosterUserCountTmp;

    private PutBlockGameState putBlockGameState;
    
    private void Awake()
    {
        backBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            UserProgress.Current.SaveGameState(putBlockGameController.ID);
            GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
        });
        
        addPutAreaBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    putBlockGameState.UnlockedPutArea = true;
                    UserProgress.Current.SaveGameState(putBlockGameController.ID);
                    RefreshView();
                }
            });
        });
        
        refreshBoosterBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            if (BoosterManager.Instance.UseBooster(GameType.PutBlockGame, BoosterType.Refresh))
            {
                putBlockGameController.RefreshAllFigure();
                putBlockGameState.AddUseBoosterCount(BoosterType.Refresh);
                UserProgress.Current.SaveGameState(putBlockGameController.ID);
            }
            else
            {
                WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
                {
                    if (isSuccess)
                    {
                        BoosterManager.Instance.BuyBooster(GameType.PutBlockGame, BoosterType.Refresh);
                    }
                });
            }
        });
        
        undoBoosterBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            if (BoosterManager.Instance.UseBooster(GameType.PutBlockGame, BoosterType.Undo))
            {
                putBlockGameController.UnDo();
                putBlockGameState.AddUseBoosterCount(BoosterType.Undo);
                UserProgress.Current.SaveGameState(putBlockGameController.ID);
                RefreshUndoBooster();
            }
            else
            {
                WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
                {
                    if (isSuccess)
                    {
                        BoosterManager.Instance.BuyBooster(GameType.PutBlockGame, BoosterType.Undo);
                    }
                });
            }
        });

        BoosterManager.Instance.OnBoosterChanged += OnBoosterChanged;
    }

    private void Start()
    {
        putBlockGameState = UserProgress.Current.GetGameState<PutBlockGameState>(putBlockGameController.ID);
        putBlockGameState.StateUpdate += RefreshView;
        RefreshView();
    }

    private void RefreshView()
    {
        addPutAreaBtn.gameObject.SetActive(!putBlockGameState.UnlockedPutArea);
        addPutAreaAdIcon.SetActive(!putBlockGameState.UnlockedPutArea);
        RefreshRefreshBooster();
        RefreshUndoBooster();
    }

    private void RefreshRefreshBooster()
    {
        var refreshBoosterCount = BoosterManager.Instance.GetBoosterCount(GameType.PutBlockGame, BoosterType.Refresh);
        refreshBoosterWatchAdObj.SetActive(refreshBoosterCount <= 0);
        refreshBoosterRemainShowObj.SetActive(refreshBoosterCount > 0);
        if (refreshBoosterCount > 0)
        {
            refreshBoosterRemainCountTmp.text = $"{refreshBoosterCount}";
        }
        else
        {
            var tempBoosterConfig = AllBoosterConfigManager.Instance.GetBoosterConfig(BoosterType.Refresh);
            var tempAddCount = tempBoosterConfig != null ? tempBoosterConfig.CountToBuy : 1;
            adAddRefreshBoosterCountTmp.text = $"+{tempAddCount}";
        }
        
        putBlockGameState ??= UserProgress.Current.GetGameState<PutBlockGameState>(putBlockGameController.ID);
        var boosterUseCount = putBlockGameState.GetBoosterUseCount(BoosterType.Refresh);
        refreshBoosterBtn.interactable = boosterUseCount < 1;
        refreshBoosterUserCountTmp.text = $"{boosterUseCount}/{1}";
    }
    
    private void RefreshUndoBooster()
    {
        var undoBoosterCount = BoosterManager.Instance.GetBoosterCount(GameType.PutBlockGame, BoosterType.Undo);
        undoBoosterWatchAdObj.SetActive(undoBoosterCount <= 0);
        undoBoosterRemainShowObj.SetActive(undoBoosterCount > 0);
        if (undoBoosterCount > 0)
        {
            undoBoosterRemainCountTmp.text = $"{undoBoosterCount}";
        }
        else
        {
            var tempBoosterConfig = AllBoosterConfigManager.Instance.GetBoosterConfig(BoosterType.Undo);
            var tempAddCount = tempBoosterConfig != null ? tempBoosterConfig.CountToBuy : 1;
            adAddUndoBoosterCountTmp.text = $"+{tempAddCount}";
        }
        
        putBlockGameState ??= UserProgress.Current.GetGameState<PutBlockGameState>(putBlockGameController.ID);
        var boosterUseCount = putBlockGameState.GetBoosterUseCount(BoosterType.Undo);
        undoBoosterBtn.interactable = boosterUseCount < 3 && putBlockGameState.CanUndo;
        undoBoosterUserCountTmp.text = $"{boosterUseCount}/{3}";
    }

    private void OnBoosterChanged(GameType gameType, BoosterType boosterType, int count)
    {
        if(gameType != GameType.PutBlockGame) return;

        switch (boosterType)
        {
            case BoosterType.Refresh:
                RefreshRefreshBooster();
                break;
            case BoosterType.Undo:
                RefreshUndoBooster();
                break;
        }
    }
}
