using GameFrame;
using Newtonsoft.Json;
using UnityEngine;

public class RemoveBlockGameManager : Singleton<RemoveBlockGameManager>
{
    #region 注册内容相关

    public RemoveBlockGamePlayRoom RemoveBlockGamePlayRoom { get; private set; }

    public void Register(RemoveBlockGamePlayRoom removeBlockGamePlayRoom) => RemoveBlockGamePlayRoom = removeBlockGamePlayRoom;

    #endregion
    
    
    private SaveModel saveModel;
    public SaveModel Data
    {
        get
        {
            saveModel ??= JsonConvert.DeserializeObject<SaveModel>(
                PlayerPrefs.GetString(nameof(RemoveBlockGameManager)));
            saveModel ??= new SaveModel();
            return saveModel;
        }

        private set => saveModel = value;
    }

    public void ChallengeSuccess()
    {
        Data.CurLevel++;
        Data.CurLevel = Mathf.Clamp(Data.CurLevel, 1, RemoveBlockGameConfig.Instance.LevelConfigs.Count - 1);
        SaveToLocal();
    }

    private void SaveToLocal()
    {
        var tempJson = JsonConvert.SerializeObject(saveModel);
        PlayerPrefs.SetString(nameof(RemoveBlockGameManager), tempJson);
    }
        
    public class SaveModel
    {
        public int CurLevel { get; set; } = 1;
    }
}   
