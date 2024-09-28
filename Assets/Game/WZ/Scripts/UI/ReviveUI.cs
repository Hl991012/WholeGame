using UnityEngine;
using UnityEngine.UI;

public class ReviveUI : MonoBehaviour
{
    [SerializeField] private TextAdventureUIPresenter textAdventureUIPresenter;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button reviveByCoinBtn;
    [SerializeField] private Button reviveByAdBtn;

    public void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            TextAdventureGameController.Instance.EndGame(false);
        });
        
        reviveByCoinBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            if (WealthManager.Instance.CurWealthModel.CoinCount < 50)
            {
                textAdventureUIPresenter.ShowPopUps("金币不足");
            }
            else
            {
                WealthManager.Instance.AddCoin(-50);
                TextAdventureGameController.Instance.Revive();
            }
        });
        
        reviveByAdBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            TextAdventureGameController.Instance.Revive();
        });
    }
}
