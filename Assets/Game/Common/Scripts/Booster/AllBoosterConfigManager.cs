using Cysharp.Threading.Tasks;
using PuzzleGame.Gameplay.Boosters;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AllBoosterConfigManager), menuName = nameof(AllBoosterConfigManager))]
public class AllBoosterConfigManager : ScriptableObject
{
    private static AllBoosterConfigManager instance;
    public static AllBoosterConfigManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAsync<ScriptableObject>("SO/AllBoosterConfig").GetAwaiter().GetResult() as AllBoosterConfigManager;
            }
            return instance;
        }
    }
    
    public SerializableDictionary<BoosterType, SingleBoosterConfig> BoosterConfigs;

    public SingleBoosterConfig GetBoosterConfig(BoosterType boosterType)
    {
        return BoosterConfigs.TryGetValue(boosterType, out var config) ? config : null;
    }
}
