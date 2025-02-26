using UnityEngine;
using UnityEngine.UI;

public class SingleGameBtnItem : MonoBehaviour
{
    public GameType gameType;
    [SerializeField] private Button clickBtn;
    [SerializeField] private GameObject watchAdObj;

    private void OnEnable()
    {
        RefreshView();
    }

    private void Awake()
    {
        clickBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();

            if (PlayerPrefs.GetInt($"game_{gameType}", 0) == 1)
            {
                GameCenter.Instance.ChangeState(GameCenter.GameState.Game, gameType);   
            }
            else
            {
                WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
                {
                    if(isSuccess)
                    {
                        MainSceneCenter.Instance.ShowTips("解锁成功");
                        PlayerPrefs.SetInt($"game_{gameType}", 1);
                        RefreshView();
                    }
                });
            }
        });
    }

    private void RefreshView()
    {
        watchAdObj.SetActive(PlayerPrefs.GetInt($"game_{gameType}", 0) != 1);
    }
}
