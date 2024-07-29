// using NMNH.Utility;
// using UnityEngine;
// using System;
// using Game.Sound;
// using Game.Helper;
// using Newtonsoft.Json;
// using Game.Save;
//
// namespace Game.Manager
// {
//     public static class SettingManager
//     {
//         #region 数据存储和加载
//         private readonly static SaveKey<SaveModel> SAVE_KEY = new SaveKey<SaveModel>(GameHelper.STORAGE_KEY_PREFIX + "SettingManager");
//
//         static SaveModel saveModel;
//
//         class SaveModel
//         {
//             [JsonProperty("is_sound_effect_open")]
//             public bool isSoundEffectOpen = true;
//             [JsonProperty("is_music_open")]
//             public bool isMusicOpen = true;
//             [JsonProperty("is_vibration_open")]
//             public bool isVibrationOpen = true;
//         }
//
//         static void Load() => saveModel = SAVE_KEY.Load(new SaveModel());
//
//         public static void Save() => SAVE_KEY.Save(saveModel);
//         #endregion
//
//         private struct Keys
//         {
//             internal const string SOUND_EFFECT_KEY = GameHelper.STORAGE_KEY_PREFIX + "SETM." + "SEK";
//             internal const string MUSIC_KEY = GameHelper.STORAGE_KEY_PREFIX + "SETM." + "MK";
//             internal const string VIBRATION_KEY = GameHelper.STORAGE_KEY_PREFIX + "SETM." + "VK";
//         }
//
//         private static bool isTmpSoundEffectNeedSilence = false;
//         private static bool isTmpMusicNeedSilence = false;
//         private static bool isTmpVibrationNeedSilence = false;
//
//         public static bool IsSoundEffectOpen => saveModel.isSoundEffectOpen && !isTmpSoundEffectNeedSilence;
//
//         public static bool IsMusicOpen => saveModel.isMusicOpen && !isTmpMusicNeedSilence;
//
//         public static bool IsVibrationOpen => saveModel.isVibrationOpen && !isTmpVibrationNeedSilence;
//
//         public static event Action OnSoundEffectClose;
//         public static event Action OnSoundEffectOpen;
//
//         public static void Init()
//         {
//             Load();
//         }
//
//         public static void SwitchSoundEffect()
//         {
//             saveModel.isSoundEffectOpen = !saveModel.isSoundEffectOpen;
//             NeedTempChangeSoundEffectNeedSilence(false);
//             if (saveModel.isSoundEffectOpen)
//             {
//                 OnSoundEffectOpen?.Invoke();
//             }
//             else
//             {
//                 OnSoundEffectClose?.Invoke();
//             }
//         }
//         public static void SwitchMusic()
//         {
//             saveModel.isMusicOpen = !saveModel.isMusicOpen;
//             NeedTempChangeBgmNeedSilence(false);
//         }
//         public static void SwitchVibration()
//         {
//             saveModel.isVibrationOpen = !saveModel.isVibrationOpen;
//             NeedTempChangeVibrationNeedSilence(false);
//         }
//
//         public static void NeedTempChangeSoundEffectNeedSilence(bool needSilence)
//         {
//             isTmpSoundEffectNeedSilence = needSilence;
//             if (needSilence)
//             {
//                 SEPlayer.Instance.SetMute(true);
//                 OnSoundEffectClose?.Invoke();
//             }
//             else
//             {
//                 if (IsSoundEffectOpen)
//                 {
//                     SEPlayer.Instance.SetMute(false);
//                     OnSoundEffectOpen?.Invoke();
//                 }
//                 else
//                 {
//                     SEPlayer.Instance.SetMute(true);
//                     OnSoundEffectClose?.Invoke();
//                 }
//             }
//         }
//
//         public static void NeedTempChangeBgmNeedSilence(bool needSilence)
//         {
//             isTmpMusicNeedSilence = needSilence;
//             if (needSilence)
//             {
//                 SoundPresenter.Instance.StopBGM();
//             }
//             else
//             {
//                 if (IsMusicOpen)
//                 {
//                     SoundPresenter.Instance.PlayBGM();
//                 }
//                 else
//                 {
//                     SoundPresenter.Instance.StopBGM();
//                 }
//             }
//         }
//
//         public static void NeedTempChangeVibrationNeedSilence(bool needSilence)
//         {
//             isTmpVibrationNeedSilence = needSilence;
//         }
//     }
// }