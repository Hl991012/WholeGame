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
    [SerializeField] private Button refreshBtn;
    [SerializeField] private TextMeshProUGUI refreshRemainCountTmp;

    private PutBlockGameState putBlockGameState;
    
    private void Awake()
    {
        putBlockGameState = UserProgress.Current.GetGameState<PutBlockGameState>(putBlockGameController.ID);

        putBlockGameState.StateUpdate += RefreshView;
        
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
        
        refreshBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    putBlockGameController.RefreshAllFigure();
                    putBlockGameState.UseRefreshBoosterCount++;
                    UserProgress.Current.SaveGameState(putBlockGameController.ID);
                    RefreshView();
                }
            });
        });
        
        RefreshView();
    }

    private void RefreshView()
    {
        addPutAreaBtn.gameObject.SetActive(!putBlockGameState.UnlockedPutArea);
        addPutAreaAdIcon.SetActive(!putBlockGameState.UnlockedPutArea);
        refreshBtn.interactable = putBlockGameState.UseRefreshBoosterCount < 1;
        refreshRemainCountTmp.text = $"{putBlockGameState.UseRefreshBoosterCount}/{1}";
    }
}
