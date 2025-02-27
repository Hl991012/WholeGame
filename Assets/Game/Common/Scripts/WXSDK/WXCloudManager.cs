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
            env = "zjyxj-cloud-2gw1pptrb141820c",
            traceUser = true,
        };
        WX.cloud.Init(callFunctionInitParam);
    }
    
    public void GetPutBlockRankInfo(Action<bool, List<SingleRankInfo>> onComplete)
    {
        var callFunctionParam = new CallFunctionParam()
        {
            name = "GetPutBlockGameRankInfo",
            data = "{}",
            fail = val =>
            {
                // Debug.LogError("调用失败" + val.errMsg + "" + val.result);
                onComplete?.Invoke(false, null);
            },
            success = val =>
            {
                // Debug.LogError("调用成功" + val.result + " " + val.callbackId);

                try
                {
                    var response = JsonConvert.DeserializeObject<ResponseInfo>(val.result);
                    if (response != null && response.State == 1)
                    {
                        if (response.Data is { Count: > 0 })
                        {
                            foreach (var item in response.Data)
                            {
                                Debug.LogError(item.ToString());
                            }
                            
                            onComplete?.Invoke(true, response.Data);
                            return;
                        }
                        
                        onComplete?.Invoke(false, null);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
                
                onComplete?.Invoke(false, null);
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
        var singleRankInfo = new SingleRankInfo()
        {
            Score = score,
        };
        
        var callFunctionParam = new CallFunctionParam()
        {
            name = "SetPutBlockGameScore",
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
    
    public class SingleRankInfo
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("score")] public int Score { get; set; }

        public override string ToString()
        {
            return $"{Name}/{Score}";
        }
    }
    
    public class ResponseInfo
    {
        [JsonProperty("state")] public int State { get; set; }
        [JsonProperty("data")] public List<SingleRankInfo> Data { get; set; }
    }


    #region 新内容
    
    
    

    #endregion
}
