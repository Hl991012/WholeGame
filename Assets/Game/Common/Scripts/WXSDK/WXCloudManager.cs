using System;
using System.Collections;
using System.Collections.Generic;
using GameFrame;
using Newtonsoft.Json;
using UnityEngine;
using WeChatWASM;

public class WXCloudManager : Singleton<WXCloudManager>
{
    public void Init()
    {
        var callFunctionInitParam = new CallFunctionInitParam()
        {
            env = "cloudbase-1gocxxxb90f46de1",
            traceUser = true,
        };
        WX.cloud.Init(callFunctionInitParam);
    }
    
    public void GetPutBlockRankInfo(Action<bool, List<SingleRankInfo>, SingleRankInfo> onComplete)
    {
        var callFunctionParam = new CallFunctionParam()
        {
            name = "GetPutBlockGameRankInfo",
            data = "{}",
            fail = val =>
            {
                // Debug.LogError("调用失败" + val.errMsg + "" + val.result);
                onComplete?.Invoke(false, null, null);
            },
            success = val =>
            {
                // Debug.LogError("调用成功" + val.result + " " + val.callbackId);

                try
                {
                    var response = JsonConvert.DeserializeObject<ResponseInfo>(val.result);
                    if (response is { State: 1 })
                    {
                        onComplete?.Invoke(true, response.RankData, response.SelfData);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
                
                onComplete?.Invoke(false, null, null);
            },
            complete = val =>
            {
                // Debug.LogError("调用完成" + val.result);
            }
        };
        WX.cloud.CallFunction(callFunctionParam);
    }
    
    public void UpdatePutBlockRankScore(int score, Action<bool> onComplete)
    {
        // 判断玩家是否登录
        if (WXSDKManager.Instance.IsLogin())
        {
            UpdateScore();
        }
        else
        {
            WXSDKManager.Instance.RequestUserInfo(UpdateScore, () =>
            {
                MainSceneCenter.Instance.ShowTips("需要授权信息才能上传分数"); 
            });
        }

        void UpdateScore()
        {
            var singleRankInfo = new SingleRankInfo()
            {
                Score = score,
            };
        
            var callFunctionParam = new CallFunctionParam()
            {
                name = "UpdatePutBlockGameScore",
                data = JsonConvert.SerializeObject(singleRankInfo),
                fail = val =>
                {
                    // Debug.LogError("调用失败" + val.errMsg + "" + val.result);
                    onComplete?.Invoke(false);
                },
                success = val =>
                {
                    // Debug.LogError("调用成功" + val.result + " " + val.callbackId);
                    onComplete?.Invoke(true);
                },
                complete = val =>
                {
                    // Debug.LogError("调用完成" + val.result);
                }
            };
            WX.cloud.CallFunction(callFunctionParam);   
        }
    }
    
    public class SingleRankInfo
    {
        [JsonProperty("rank")] public int Rank { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("score")] public int Score { get; set; }
        
        [JsonProperty("avatar")] public string AvatarUrl { get; set; }
    }
    
    public class ResponseInfo
    {
        [JsonProperty("state")] public int State { get; set; }
        [JsonProperty("rank_data")] public List<SingleRankInfo> RankData { get; set; }
        [JsonProperty("self_data")] public SingleRankInfo SelfData { get; set; }
    }


    #region 新内容
    
    
    

    #endregion
}
