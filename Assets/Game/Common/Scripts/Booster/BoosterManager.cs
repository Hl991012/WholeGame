using System;
using System.Collections.Generic;
using GameFrame;
using Newtonsoft.Json;

public class BoosterManager : Singleton<BoosterManager>
{
     private Dictionary<GameType, Dictionary<BoosterType, int>> allBooster;

     public Action<GameType, BoosterType, int> OnBoosterChanged;
     
     public BoosterManager()
     {
          var tempStr = PlayerPrefs.GetString(nameof(BoosterManager));
          allBooster = string.IsNullOrEmpty(tempStr) ? new Dictionary<GameType, Dictionary<BoosterType, int>>() : 
               JsonConvert.DeserializeObject<Dictionary<GameType, Dictionary<BoosterType, int>>>(tempStr);
          allBooster ??= new Dictionary<GameType, Dictionary<BoosterType, int>>();
     }

     public void BuyBooster(GameType gameType, BoosterType boosterType)
     {
          if (!allBooster.ContainsKey(gameType))
          {
               allBooster.Add(gameType, new Dictionary<BoosterType, int>());
          }

          if (!allBooster[gameType].ContainsKey(boosterType))
          {
               allBooster[gameType].Add(boosterType, 0);
          }

          var boosterConfig = AllBoosterConfigManager.Instance.GetBoosterConfig(boosterType);
          if (boosterConfig != null)
          {
               allBooster[gameType][boosterType] += boosterConfig.CountToBuy;
          }
          else
          {
               allBooster[gameType][boosterType] += 1;
          }
          
          OnBoosterChanged?.Invoke(gameType, boosterType, allBooster[gameType][boosterType]);
               
          Save();
     }

     public int GetBoosterCount(GameType gameType, BoosterType boosterType)
     {
          if (!allBooster.ContainsKey(gameType))
          {
               allBooster.Add(gameType, new Dictionary<BoosterType, int>());
          }

          if (!allBooster[gameType].ContainsKey(boosterType))
          {
               var boosterConfig = AllBoosterConfigManager.Instance.GetBoosterConfig(boosterType);
               if (boosterConfig != null)
               {
                    allBooster[gameType][boosterType] = boosterConfig.startCount;
               }
               else
               {
                    allBooster[gameType][boosterType] = 0;
               }
          }

          return allBooster[gameType][boosterType];
     }

     public bool UseBooster(GameType gameType, BoosterType boosterType)
     {
          int count = GetBoosterCount(gameType, boosterType);
          if (count > 0)
          {
               allBooster[gameType][boosterType]--;
               OnBoosterChanged?.Invoke(gameType, boosterType, count - 1);
               Save();
               return true;
          }

          return false;
     }
     
     private void Save()
     {
          if (allBooster != null)
          {
                PlayerPrefs.SetString(nameof(BoosterManager), JsonConvert.SerializeObject(allBooster));
          }
     }
}
