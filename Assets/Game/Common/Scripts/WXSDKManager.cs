using System;
using GameFrame;
using UnityEngine;
using WeChatWASM;

public class WXSDKManager : Singleton<WXSDKManager>
{
    private bool hasInit;

    private WXRewardedVideoAd wxRewardedVideoAd;

    private WXInterstitialAd wxInterstitialAd;
    
    public void Init()
    {
        #if UNITY_EDITOR
        #else
            WX.InitSDK(val =>
        {
            hasInit = true;
            
            wxRewardedVideoAd = WX.CreateRewardedVideoAd(
                new WXCreateRewardedVideoAdParam()
                {
                    adUnitId = "adunit-6918133c0430e1e9",
                    multiton = true
                });
            
            wxInterstitialAd = WX.CreateInterstitialAd(
                new WXCreateInterstitialAdParam()
                {
                    adUnitId = "adunit-dc63d74f56278361"
                });
        });
        #endif
    }

    public void ShowRewardVideo(Action<bool> onClose)
    {
        if (!hasInit || wxRewardedVideoAd == null)
        {
            onClose?.Invoke(false);
            return;
        }
        
        wxRewardedVideoAd.Show(val =>
        {
            wxRewardedVideoAd.OnClose((WXRewardedVideoAdOnCloseResponse res)=>
            {
                if ((res != null && res.isEnded) || res == null)
                {
                    // 正常播放结束，可以下发游戏奖励
                    onClose?.Invoke(true);
                }
                else
                {
                    // 播放中途退出，不下发游戏奖励
                    onClose?.Invoke(false);
                }
                wxRewardedVideoAd.OffClose(null);
            });
        }, val =>
        {
            onClose?.Invoke(false);
        });
    }

    // private long lastTimeWatchInterstitialVideoTime;
    
    public void ShowInterstitialVideo(Action onClose)
    {
        if (!hasInit || wxInterstitialAd == null)
        {
            onClose?.Invoke();
            return;
        }
        
        wxInterstitialAd.Show(val =>
        {
            wxInterstitialAd.OnClose(()=>
            {
                onClose?.Invoke();
                Debug.Log("播放插屏广告");
                wxRewardedVideoAd.OffClose(null);
            });
        });
    }
}
