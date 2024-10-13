using UnityEngine;
using UnityEngine.UI;

public class UIPresenter : MonoBehaviour
{
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button putBlockGameBtn;
    [SerializeField] private Button textAdventureGameBtn;
    [SerializeField] private Button drawLineGameBtn;
    [SerializeField] private Button game2048Btn;
    [SerializeField] private Button x2BlockGameBtn;
    [SerializeField] private SettingPanel settingPanel;
    
    private void Awake()
    {
        settingBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            settingPanel.gameObject.SetActive(true);
        });
        
        putBlockGameBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.ChangeState(GameCenter.GameState.Game, GameType.PutBlockGame);
        });
        
        textAdventureGameBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.ChangeState(GameCenter.GameState.Game, GameType.TextAdventure);
        });
        
        drawLineGameBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.ChangeState(GameCenter.GameState.Game, GameType.DrawLineGame);
        });
        
        game2048Btn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.ChangeState(GameCenter.GameState.Game, GameType.Game2048);
        });
        
        x2BlockGameBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.ChangeState(GameCenter.GameState.Game, GameType.X2BlockGame);
        });
    }
}
