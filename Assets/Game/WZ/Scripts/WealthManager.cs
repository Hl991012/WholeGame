using System;
using System.IO;
using Game.Save;
using GameFrame;
using Newtonsoft.Json;
using UnityEngine;

public class WealthManager : Singleton<WealthManager>
{
    #region 数据存储相关内容

    private SaveKey<WealthModel> SaveKey = new (nameof(WealthManager));

    private WealthModel tempModel;
    
    public WealthModel CurWealthModel => tempModel ??= SaveKey.Load(new WealthModel());
    
    public void Save()
    {
        SaveKey.Save(CurWealthModel);
    }

    #endregion
    
    public Action OnWealthChanged;

    public void AddCoin(int count)
    {
        CurWealthModel.CoinCount += count;
        OnWealthChanged?.Invoke();
    }

    public void CostCoin(int count)
    {
        CurWealthModel.CoinCount -= count;
        CurWealthModel.CoinCount = Mathf.Max(0, CurWealthModel.CoinCount);
        OnWealthChanged?.Invoke();
    }
}

[Serializable]
public class WealthModel
{
    [JsonProperty("coin_count")]
    public int CoinCount { get; set; }
}
