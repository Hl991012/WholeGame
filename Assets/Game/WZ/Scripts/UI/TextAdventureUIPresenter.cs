using GameFrame;
using UnityEngine;

public class TextAdventureUIPresenter : MonoBehaviour
{
    [SerializeField] private TextAdventureStageListUI textAdventureStageListUI;
    [SerializeField] private GameUI gameUI;
    [SerializeField] private ResultUI resultUI;
    [SerializeField] private ReviveUI reviveUI;
    [SerializeField] private PopUpsUI popUpsUI;

    private void Start()
    {
        TextAdventureGameController.Instance.OnGameStateChanged += OnGameStateChanged;
        OnGameStateChanged();
    }

    private void OnGameStateChanged()
    {
        textAdventureStageListUI.gameObject.SetActive(TextAdventureGameController.Instance.CurGameState == TextAdventureGameController.GameState.Home);
        gameUI.gameObject.SetActive(TextAdventureGameController.Instance.CurGameState == TextAdventureGameController.GameState.Playing ||
                                    TextAdventureGameController.Instance.CurGameState == TextAdventureGameController.GameState.Reviving ||
                                    TextAdventureGameController.Instance.CurGameState == TextAdventureGameController.GameState.Settlement);
        resultUI.gameObject.SetActive(TextAdventureGameController.Instance.CurGameState == TextAdventureGameController.GameState.Settlement);
        reviveUI.gameObject.SetActive(TextAdventureGameController.Instance.CurGameState == TextAdventureGameController.GameState.Reviving);
        switch (TextAdventureGameController.Instance.CurGameState)
        {
            case TextAdventureGameController.GameState.Home:
                textAdventureStageListUI.RefreshView();
                break;
            case TextAdventureGameController.GameState.Playing:
                gameUI.RefreshView();
                break;
            case TextAdventureGameController.GameState.Settlement:
                WXSDKManager.Instance.ShowInterstitialVideo(null);
                resultUI.RefreshView();
                break;
        }
    }

    public void ShowPopUps(string des)
    {
        popUpsUI.Show(des);
    }
}
