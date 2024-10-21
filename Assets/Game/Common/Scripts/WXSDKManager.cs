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
            
            
            Debug.LogError("创建" + wXCustomAd.ToString());

            wxInterstitialAd.OnError((WXADErrorResponse result) =>
            {
                OnError?.Invoke();
                Debug.LogError("被动广告错误" + result.ToString());
            });
            
            wxRewardedVideoAd.OnError((WXADErrorResponse result) =>
            {
                OnError?.Invoke();
                Debug.LogError("主动广告错误" + result.ToString());
            });
            
            wXCustomAd.OnError((WXADErrorResponse result) =>
            {
                OnError?.Invoke();
                Debug.LogError("自定义广告错误" + result.ToString());
            });
        });
            
        
        
        GetSystemInfoAsyncOption tempGetSystemInfoAsyncOption = new GetSystemInfoAsyncOption()
        {
            success = val =>
            {
                var width = Mathf.Max(300, (int)val.windowWidth);
                
                wXCustomAd = WX.CreateCustomAd(new WXCreateCustomAdParam()
                {
                    adIntervals = 30,
                    adUnitId = "adunit-e45f98074d27985a",
                    style = new CustomStyle()
                    {
                        left = 0,
                        top = (int)val.windowHeight - width,
                        width = width,
                    },
                });
                
                wXCustomAd.OnLoad(val =>
                {
                    Debug.LogError("自定义广告加载成功" + val.rewardValue + "  " + val.shareValue + "  " + wXCustomAd.style.ToString());
                    ShowCustomAd();
                });
            },
            fail = val =>
            {
                Debug.LogError("获取系统信息失败");
            },
            complete = val =>
            {
                // 展示自定义广告
                
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
            Debug.Log("播放插屏广告");
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
        if (!hasInit || wXCustomAd == null)
        {
            Debug.Log("展示自定义广告失败" + hasInit + "  " + (wXCustomAd == null));
            return;
        }
        
        if(GameCenter.Instance.CurGameState == GameCenter.GameState.Home)
            return;

        Debug.Log("展示自定义广告");
        IsShowBanner = true;
        wXCustomAd.Show();
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
}
