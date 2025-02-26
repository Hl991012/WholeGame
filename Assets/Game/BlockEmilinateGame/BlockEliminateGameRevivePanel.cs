using System;
using PutBlockGame;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockEliminateGameRevivePanel : MonoBehaviour
{
    [SerializeField] private Button onceAgainBtn;
    [SerializeField] private Button reviveBtn;
    [SerializeField] private Button shareBtn;
    [SerializeField] private TextMeshProUGUI hintTmp;

    private bool isGameOver;
    private Action OnRevive;
    
    private void Awake()
    {
        onceAgainBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            gameObject.SetActive(false);
            BlockEliminateStageManager.Instance.StartNewGame();
        });
        
        reviveBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    gameObject.SetActive(false);
                    OnRevive?.Invoke();
                }
            });
        });
        
        shareBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.Share();
        });
    }

    public BlockEliminateGameRevivePanel Init(bool gameOver, Action onRevive)
    {
        isGameOver = gameOver;
        OnRevive = onRevive;
        return this;
    }

    public void Show()
    {
        PlayRoomPresenter.Instance.BlockEliminateGame.SetInputEnable(false);
        Observable.Timer(TimeSpan.FromSeconds(1.5f))
            .Subscribe(_ =>
            {
                PlayRoomPresenter.Instance.BlockEliminateGame.SetInputEnable(true);
                WXSDKManager.Instance.ShowInterstitialVideo(null);
                gameObject.SetActive(true);
        
                onceAgainBtn.gameObject.SetActive(true);
                reviveBtn.gameObject.SetActive(!isGameOver);
                shareBtn.gameObject.SetActive(isGameOver);

                hintTmp.text = isGameOver ? "很遗憾！再来一次吧！" : "就差一点点了，加油！！！";
            }).AddTo(this);
    }
}
