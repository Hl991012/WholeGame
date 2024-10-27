using GameFrame;
using PutBlockGame;
using TMPro;
using UnityEngine;
using WeChatWASM;

public class MainSceneCenter : MonoSingleton<MainSceneCenter>
{
    [SerializeField] private UIPresenter uiPresenter;
    [SerializeField] private PlayRoomPresenter playRoomPresenter;
    [SerializeField] private PopUpsUI popUpsUI;
    [SerializeField] private TMP_FontAsset tmpFontAsset;
    
    private void Awake()
    {
        GameCenter.Instance.onGameStateChanged += OnGameStateChanged;
        
        WX.GetWXFont("", font =>
        {
            tmpFontAsset.fallbackFontAssetTable.Add(TMP_FontAsset.CreateFontAsset(font));
        });
    }

    private void Start()
    {
        GameCenter.Instance.ChangeState(GameCenter.GameState.Game, GameType.PutBlockGame);
    }

    private void OnGameStateChanged()
    {
        uiPresenter.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Home);
        playRoomPresenter.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Game);
        switch (GameCenter.Instance.CurGameState)
        {
            case GameCenter.GameState.Home:
                break;
            case GameCenter.GameState.Game:
                playRoomPresenter.RefreshView();
                break;
        }
    }

    public void ShowTips(string tips)
    {
        popUpsUI.Show(tips);
    }
}
