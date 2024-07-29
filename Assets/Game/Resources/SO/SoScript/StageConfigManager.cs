using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
                instance = Resources.LoadAsync<ScriptableObject>("SO/StageConfigSO").GetAwaiter().GetResult() as StageConfigManager;
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
