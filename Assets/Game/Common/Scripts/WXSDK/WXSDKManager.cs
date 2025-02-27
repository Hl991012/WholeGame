using System;
using GameFrame;
using UnityEngine;
using WeChatWASM;

public class WXSDKManager : Singleton<WXSDKManager>
{
    private bool hasInit;

    private static WXRewardedVideoAd wxRewardedVideoAd;

    private static WXInterstitialAd wxInterstitialAd;

    private static WXCustomAd wXCustomAd;
    
    private static WXCustomAd wXCustomAd_1;

    private Action<WXRewardedVideoAdOnCloseResponse> onCloseRewardedVideoAd;
    private Action onCloseInterstitialVideo;

    private Action OnError = () =>
    {
        if (MainSceneCenter.Instance != null)
        {
            MainSceneCenter.Instance.ShowTips("内容制作中。。。");
        }
    };

    public void Init()
    {
#if UNITY_EDITOR

#else
            WX.InitSDK(val =>
        {
            hasInit = true;
            
            GetSetting();

            wxRewardedVideoAd = WX.CreateRewardedVideoAd(
                new WXCreateRewardedVideoAdParam()
                {
                    adUnitId = "adunit-6918133c0430e1e9",
                    multiton = true
                });
            
            wxRewardedVideoAd.Load();
            
            wxInterstitialAd = WX.CreateInterstitialAd(
                new WXCreateInterstitialAdParam()
                {
                    adUnitId = "adunit-dc63d74f56278361"
                });
            
            wxInterstitialAd.Load();

            wxInterstitialAd.OnError((WXADErrorResponse result) =>
            {
                // OnError?.Invoke();
                Debug.LogError("被动广告错误" + result.ToString());
            });
            
            wxRewardedVideoAd.OnError((WXADErrorResponse result) =>
            {
                OnError?.Invoke();
                Debug.LogError("主动广告错误" + result.ToString());
            });
            
            wXCustomAd.OnError((WXADErrorResponse result) =>
            {
                // OnError?.Invoke();
                Debug.LogError("自定义广告错误" + result.ToString());
            });

            wXCustomAd_1.OnError((WXADErrorResponse result) =>
            {
                // OnError?.Invoke();
                Debug.LogError("自定义广告错误" + result.ToString());
            });
        });

        var windowWidth = 0;
        var windowHeight = 0;
        GetSystemInfoAsyncOption tempGetSystemInfoAsyncOption = new GetSystemInfoAsyncOption()
        {
            success = val =>
            {
                windowWidth = (int)val.windowWidth;
                windowHeight = (int)val.windowHeight;
            },
            fail = val =>
            {
                Debug.LogError("获取系统信息失败");
            },
            complete = val =>
            {
                // 展示自定义广告
                wXCustomAd = WX.CreateCustomAd(new WXCreateCustomAdParam()
                {
                    adIntervals = 30,
                    adUnitId = "adunit-e45f98074d27985a",
                    style = new CustomStyle()
                    {
                        left = windowWidth / 2 - 144,
                        top = windowHeight - 84,
                        width = windowWidth, 
                    },
                });
                
                // 展示自定义广告
                wXCustomAd_1 = WX.CreateCustomAd(new WXCreateCustomAdParam()
                {
                    adIntervals = 30,
                    adUnitId = "adunit-8c7aa40c3efd332e",
                    style = new CustomStyle()
                    {
                        left = windowWidth - 72,
                        top = 190,
                        width = 72, 
                    },
                });
            }
        };
        
        WX.GetSystemInfoAsync(tempGetSystemInfoAsyncOption);

#endif
        
    }

    public void ShowRewardVideo(Action<bool> onClose)
    {
#if UNITY_EDITOR
        onClose?.Invoke(true);
        return;
#endif
        
        if (!hasInit || wxRewardedVideoAd == null)
        {
            onClose?.Invoke(false);
            return;
        }
        
        wxRewardedVideoAd.Show(val =>
        {
            onCloseRewardedVideoAd = (WXRewardedVideoAdOnCloseResponse res) =>
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

                wxRewardedVideoAd.OffClose(onCloseRewardedVideoAd);
                onCloseRewardedVideoAd = null;
            };
            
            wxRewardedVideoAd.OnClose(onCloseRewardedVideoAd);
        }, val =>
        {
            wxRewardedVideoAd.Load();
            onClose?.Invoke(false);
        });
    }
    
    public void ShowInterstitialVideo(Action onClose)
    {
        if (!hasInit || wxInterstitialAd == null)
        {
            onClose?.Invoke();
            return;
        }

        onCloseInterstitialVideo = () =>
        {
            onClose?.Invoke();
            // Debug.Log("播放插屏广告");
            wxInterstitialAd.OffClose(onCloseInterstitialVideo);
        };
        
        wxInterstitialAd.Show(val =>
        {
            wxInterstitialAd.OnClose(onCloseInterstitialVideo);
        }, val =>
        {
            onCloseInterstitialVideo?.Invoke();
            wxInterstitialAd.OnLoad(OnLoad);
            wxInterstitialAd.Load();
        });
    }

    private static Action<WXADLoadResponse> OnLoad = val =>
    {
        wxInterstitialAd.OffLoad(OnLoad);
    };

    public bool IsShowBanner { get; private set; } = false;

    public void ShowCustomAd()
    {
        if (!hasInit || wXCustomAd == null || IsShowBanner)
        {
            return;
        }

        // Debug.Log("展示自定义广告");
        IsShowBanner = true;
        wXCustomAd.Show(failed: response =>
        {
            IsShowBanner = false;
        });
    }

    public void CloseCustomAd()
    {
        IsShowBanner = false;
        if (!hasInit || wXCustomAd == null)
        {
            return;
        }
        
        wXCustomAd.Hide();
    }
    
    
    public bool IsShowBanner1 { get; private set; } = false;

    public void ShowCustomAd1()
    {
        if (!hasInit || wXCustomAd_1 == null || IsShowBanner1)
        {
            return;
        }

        IsShowBanner1 = true;
        wXCustomAd_1.Show(failed: response =>
        {
            IsShowBanner1 = false;
        });
    }

    public void CloseCustomAd1()
    {
        IsShowBanner1 = false;
        if (!hasInit || wXCustomAd_1 == null)
        {
            return;
        }
        
        wXCustomAd_1.Hide();
    }

    #region 分享相关内容

    public void Share()
    {
        if(!hasInit)
        {
            if (MainSceneCenter.Instance != null)
            {
                MainSceneCenter.Instance.ShowTips("内容制作中。。。");
            }    
            return;
        }

        ShareAppMessageOption shareAppMessageOption = new ShareAppMessageOption()
        {
            imageUrl =
                "https://mmocgame.qpic.cn/wechatgame/X4cGHmN8OVbp11yKfO0IgxCGJFJTCfibW74e82Z4vSUdHJN6NTkwfz3rnRX7OITIJ/0",
            imageUrlId = "",
            title = "这游戏太棒了，和我一起玩吧！！！"
        };
        
        WX.ShareAppMessage(shareAppMessageOption);
    }

    #endregion

    #region 订阅功能

    SubscriptionsSetting subscriptionsSetting = null;   
    
    public void GetSetting()
    {
        var getSettingOption = new GetSettingOption()
        {
            fail = val =>
            {
                // Debug.LogError("获得Setting信息失败");
            },
            success = val =>
            {
                // Debug.LogError("获得Setting信息成功");
                subscriptionsSetting = val.subscriptionsSetting;
                authSetting = val.authSetting;
            },
            complete = val =>
            {
                // Debug.LogError("获得Setting信息完成");
            },
            withSubscriptions = true,
        };
        
        WX.GetSetting(getSettingOption);
    }

    public void ShowSubscribeMessage()
    {
        // 如果用户打开了订阅消息的总开关
        if (subscriptionsSetting is { mainSwitch: true })
        {
            if (!subscriptionsSetting.itemSettings.ContainsKey("SYS_MSG_TYPE_WHATS_NEW"))
            {
                RequestSubscribeSystemMessageOption requestSubscribeMessageOption = new RequestSubscribeSystemMessageOption()
                {
                    msgTypeList = new string[]{"SYS_MSG_TYPE_WHATS_NEW"},
                    complete = val =>
                    {
                        // Debug.LogError("订阅完成");
                        WX.OffTouchEnd();
                    },
                    fail = val =>
                    {
                        // Debug.LogError("订阅失败" + val.errCode + "  " + val.errMsg);
                    },
                    success = val =>
                    {
                        // Debug.LogError("订阅成功");
                    },
                };
        
                WX.RequestSubscribeSystemMessage(requestSubscribeMessageOption);
            }
            else
            {
                // Debug.LogError("包含更新订阅消息");
            }
        }
    }

    #endregion

    #region 获取用户信息相关内容

    private AuthSetting authSetting;

    // 是否已经拿到用户信息
    public bool IsLogin()
    {
#if UNITY_EDITOR
        return true;
#endif
        
        return authSetting != null && authSetting.ContainsKey("scope.userInfo") && authSetting["scope.userInfo"];
    }
    
    public void RequestUserInfo(Action onSuccess)
    {
        // 请求用户授权
        GetUserInfoOption callback = new GetUserInfoOption();
        callback.complete += val =>
        {
            GetSetting();
        };

        callback.success = val =>
        {
            onSuccess?.Invoke();
        };
        WX.GetUserInfo(callback);
    }

    #endregion
}
