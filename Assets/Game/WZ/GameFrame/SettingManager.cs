using NMNH.Utility;
using System;
using Newtonsoft.Json;
using Game.Save;

namespace Game.Manager
{
    public static class SettingManager
    {
        #region 数据存储和加载
        private static readonly SaveKey<SaveModel> SaveKey = new ("SettingManager");

        private static SaveModel tempModel;
        
        static SaveModel saveModel => tempModel ??= SaveKey.Load(new SaveModel());

        class SaveModel
        {
            [JsonProperty("is_sound_effect_open")]
            public bool isSoundEffectOpen = true;
            [JsonProperty("is_music_open")]
            public bool isMusicOpen = true;
            [JsonProperty("is_vibration_open")]
            public bool isVibrationOpen = true;
        }
        
        public static void Save() => SaveKey.Save(saveModel);
        #endregion

        private static bool isTmpSoundEffectNeedSilence = false;
        private static bool isTmpMusicNeedSilence = false;
        private static bool isTmpVibrationNeedSilence = false;

        public static bool IsSoundEffectOpen => saveModel.isSoundEffectOpen && !isTmpSoundEffectNeedSilence;

        public static bool IsMusicOpen => saveModel.isMusicOpen && !isTmpMusicNeedSilence;

        public static bool IsVibrationOpen => saveModel.isVibrationOpen && !isTmpVibrationNeedSilence;

        public static event Action OnSoundEffectClose;
        public static event Action OnSoundEffectOpen;

        public static void SwitchSoundEffect()
        {
            saveModel.isSoundEffectOpen = !saveModel.isSoundEffectOpen;
            NeedTempChangeSoundEffectNeedSilence(false);
            if (saveModel.isSoundEffectOpen)
            {
                OnSoundEffectOpen?.Invoke();
            }
            else
            {
                OnSoundEffectClose?.Invoke();
            }
            Save();
        }
        public static void SwitchMusic()
        {
            saveModel.isMusicOpen = !saveModel.isMusicOpen;
            if (saveModel.isMusicOpen)
            {
                AudioManager.Instance.PlayBGM();
            }
            else
            {
                AudioManager.Instance.StopBgm();
            }
            Save();
        }
        public static void SwitchVibration()
        {
            saveModel.isVibrationOpen = !saveModel.isVibrationOpen;
            NeedTempChangeVibrationNeedSilence(false);
        }

        private static void NeedTempChangeSoundEffectNeedSilence(bool needSilence)
        {
            isTmpSoundEffectNeedSilence = needSilence;
            if (needSilence)
            {
                AudioManager.Instance.SetMute(true);
                OnSoundEffectClose?.Invoke();
            }
            else
            {
                if (IsSoundEffectOpen)
                {
                    AudioManager.Instance.SetMute(false);
                    OnSoundEffectOpen?.Invoke();
                }
                else
                {
                    AudioManager.Instance.SetMute(true);
                    OnSoundEffectClose?.Invoke();
                }
            }
        }

        private static void NeedTempChangeVibrationNeedSilence(bool needSilence)
        {
            isTmpVibrationNeedSilence = needSilence;
        }
    }
}