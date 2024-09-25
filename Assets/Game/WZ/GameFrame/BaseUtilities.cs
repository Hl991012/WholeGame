using System;
using NMNH.Utility;

public class BaseUtilities
{
    public static void PlayCommonClick()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.CommonClick);
    }
    

    /// <summary>
    /// 返回值毫秒
    /// </summary>
    public static long GetClientMillisecondTimestamp()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    public static DateTime UtcTimeStampToLocalDateTime(long unixMillisecondTime)
    {
        return TimeZoneInfo
            .ConvertTime(DateTimeOffset.FromUnixTimeMilliseconds(unixMillisecondTime), TimeZoneInfo.Local).DateTime;
    }


// #if UNITY_EDITOR
//         private static int TotalVibrateTimes = 0;
// #endif
//         public static void VibrateMedium(long milliseconds = 20)
//         {
//             if (!SettingManager.IsVibrationOpen) return;
// #if UNITY_EDITOR
//             TotalVibrateTimes += 1;
// #elif UNITY_ANDROID
//             AndroidVibration.Vibrate(milliseconds);
// #elif UNITY_IOS
//             IOSHelper.VibrateMedium();            
// #else
//             Handheld.Vibrate();
// #endif
//         }

//         public static void VibrateHeavy(long milliseconds = 20)
//         {
//             if (!SettingManager.IsVibrationOpen) return;
// #if UNITY_EDITOR
//             TotalVibrateTimes += 1;
// #elif UNITY_ANDROID
//             AndroidVibration.Vibrate(milliseconds);
// #elif UNITY_IOS
//             IOSHelper.VibrateHeavy();            
// #else
//             Handheld.Vibrate();
// #endif
//         }

//         public static void VibrateLight(long milliseconds = 20)
//         {
//             if (!SettingManager.IsVibrationOpen) return;
// #if UNITY_EDITOR
//             TotalVibrateTimes += 1;
// #elif UNITY_ANDROID
//             AndroidVibration.Vibrate(milliseconds);
// #elif UNITY_IOS
//             IOSHelper.VibrateLight();            
// #else
//             Handheld.Vibrate();
// #endif
//         }
}