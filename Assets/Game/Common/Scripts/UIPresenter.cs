using NMNH.Utility;
using UnityEngine;
using UnityEngine.UI;

public class UIPresenter : MonoBehaviour
{
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button putBlockGameBtn;
    [SerializeField] private Button textAdventureGameBtn;
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
    }
}
