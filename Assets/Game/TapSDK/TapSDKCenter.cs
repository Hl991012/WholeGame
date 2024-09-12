using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapSDK.Core;
using TapSDK.Login;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class TapSDKCenter : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        // 核心配置 详细参数见 [TapTapSDK]
        TapTapSdkOptions coreOptions = new TapTapSdkOptions();
        // TapSDK 初始化
        TapTapSDK.Init(coreOptions);
    }

    async void Start()
    {
        try
        {
            TapTapAccount account = await TapTapLogin.Instance.GetCurrentTapAccount();
            if (account == null)
            {
                // 用户未登录,展示登录按钮
            }
            else
            {
                // 用户已登录,直接进入游戏
            }
        }
        catch (Exception e)
        {
            Debug.Log($"获取用户信息失败 {e.Message}");
        }
    }

    private async UniTask Login()
    {
        // 定义授权范围
        List<string> permissions = new List<string>();
        permissions.Add(TapTapLogin.TAP_LOGIN_SCOPE_BASIC_INFO);

        // 初始化登录请求 Task
        Task<TapTapAccount> task = TapTapLogin.Instance.LoginWithScopes(permissions.ToArray());
        var result = await task;

        // 判断登录结果
        if (task.IsCompleted)
        {
            // 登录成功
        }
        else if (task.IsCanceled)
        {
            // 登录取消
        }
        else
        {
            // 登录失败
            Debug.Log($"登录失败: {task.Exception.Message}");
        }
    }
}