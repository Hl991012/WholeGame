using UnityEngine;
using UnityEngine.UI;

public class ReviveUI : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button reviveByCoinBtn;
    [SerializeField] private Button reviveByAdBtn;

    public void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.EndGame(false);
        });
        
        reviveByCoinBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            if (WealthManager.Instance.CurWealthModel.CoinCount < 50)
            {
                UIPresenter.Instance.ShowPopUps("金币不足");
            }
            else
            {
                WealthManager.Instance.AddCoin(-50);
                GameCenter.Instance.Revive();
            }
        });
        
        reviveByAdBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.Revive();
        });
    }
}
