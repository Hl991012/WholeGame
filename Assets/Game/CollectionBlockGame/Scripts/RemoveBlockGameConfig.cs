using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(RemoveBlockGameConfig), menuName = "SO/" + nameof(RemoveBlockGameConfig))]
public class RemoveBlockGameConfig : ScriptableObject
{
    private static RemoveBlockGameConfig instance;
    public static RemoveBlockGameConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAsync<ScriptableObject>("SO/RemoveBlockGameConfig").GetAwaiter().GetResult() as RemoveBlockGameConfig;
            }
            return instance;
        }
    }

    // 配置游戏中能生成的游戏物体的形状
    public List<SingleBlockItem> BlockItems;
    // 配置游戏中能生成游戏物体的颜色
    public List<Color> ColorConfigs;
    
    public List<SingleLevelConfig> LevelConfigs;

    public SingleLevelConfig GetConfigByLevel(int level)
    {
        level = Mathf.Clamp(level, 1, LevelConfigs.Count - 1);
        return LevelConfigs[level];
    }
    
    public class SingleLevelConfig
    {
        public int level;
        public int loadBlockGroupCount;
        public int countDown;
    }
}

