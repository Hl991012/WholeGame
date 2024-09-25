using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Button settingBtn;
    [SerializeField] private TextMeshProUGUI coinCountTmp;
    [SerializeField] private Button gameListBtn;
    [SerializeField] private StageListUIPresenter stageListUIPresenter;
    [SerializeField] private Button fastStartBtn;

    private void Awake()
    {
        WealthManager.Instance.OnWealthChanged += RefreshCoinView;
        
        settingBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
        });
        
        gameListBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            stageListUIPresenter.gameObject.SetActive(true);
        });
        
        fastStartBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            var canPlayStageLevel = StageManager.Instance.CurStageLevel <= StageConfigManager.Instance.TotalStageCount ? 
                StageManager.Instance.CurStageLevel : StageConfigManager.Instance.TotalStageCount;
            GameCenter.Instance.StartGame(canPlayStageLevel);
        });
    }

    public void RefreshView()
    {
        RefreshCoinView();
        stageListUIPresenter.RefreshView();
    }
    
    private void RefreshCoinView()
    {
        coinCountTmp.text = WealthManager.Instance.CurWealthModel.CoinCount.ToString();
    }
}
