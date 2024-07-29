using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = nameof(StageManager), menuName = "SO/" + nameof(StageManager))]
public class StageConfigManager : ScriptableObject
{
    private static StageConfigManager instance;
    public static StageConfigManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Addressables.LoadAssetAsync<ScriptableObject>("StageConfigSO").WaitForCompletion() as StageConfigManager;
            }
            return instance;
        }
    }
    
    public SerializableDictionary<int, StageConfigModel> AllStageConfigModels = new ();

    public StageConfigModel GetStageConfig(int stageLevel)
    {
        if (AllStageConfigModels.TryGetValue(stageLevel, out var config))
        {
            return config;
        }

        return null;
    }

    public int TotalStageCount => AllStageConfigModels.Count;
}

[Serializable]
public class StageConfigModel
{
    public int Level;
    public GameObject StagePrefab;
}
