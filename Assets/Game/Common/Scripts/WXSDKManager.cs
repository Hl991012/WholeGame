using System;
using System.Collections;
using System.Collections.Generic;
using GameFrame;
using UnityEngine;
using WeChatWASM;

public class WXSDKManager : Singleton<WXSDKManager>
{
    private bool hasInit = false;

    private WXRewardedVideoAd wxRewardedVideoAd;
    
    public void Init()
    {
        WX.InitSDK(val =>
        {
            hasInit = true;
            
            wxRewardedVideoAd = WX.CreateRewardedVideoAd(
                new WXCreateRewardedVideoAdParam()
                {
                    adUnitId = "adunit-6918133c0430e1e9",
                    multiton = true
                });
        });
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
}
