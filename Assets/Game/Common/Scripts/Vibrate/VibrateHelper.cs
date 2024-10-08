using System;
using Game.Manager;
using WeChatWASM;

public class VibrateHelper
{
    private static Action<GeneralCallbackResult> vibrateShortComplete = callback => { };
    private static Action<VibrateShortFailCallbackResult> vibrateShortFail = callback => { };
    private static Action<GeneralCallbackResult> vibrateShortSuccess = callback => { };
    
    public static void VibrateLight()
    {
        if (!SettingManager.IsVibrationOpen) return;
       
        if (!SettingManager.IsVibrationOpen) return;
#if UNITY_EDITOR
//         // WX.VibrateShort(null);
// #elif UNITY_ANDROID
//             AndroidVibration.Vibrate(milliseconds);
// #elif UNITY_IOS
//             IOSHelper.VibrateLight();            
#else
            var vibrateShortOption = new VibrateShortOption()
            {
                type = "light",
                complete = vibrateShortComplete,
                fail = vibrateShortFail,
                success = vibrateShortSuccess
            };
            WX.VibrateShort(vibrateShortOption);
#endif
    }

    public static void VibrateMedium()
    {
        if (!SettingManager.IsVibrationOpen) return;
        
#if UNITY_EDITOR
        // WX.VibrateShort(null);
// #elif UNITY_ANDROID
//             AndroidVibration.Vibrate(milliseconds);
// #elif UNITY_IOS
//             IOSHelper.VibrateMedium();            
#else
            var vibrateShortOption = new VibrateShortOption()
            {
                type = "medium",
                complete = vibrateShortComplete,
                fail = vibrateShortFail,
                success = vibrateShortSuccess
            };
            WX.VibrateShort(vibrateShortOption);
#endif
    }

    public static void VibrateHeavy()
    {
        if (!SettingManager.IsVibrationOpen) return;
     
        if (!SettingManager.IsVibrationOpen) return;
#if UNITY_EDITOR
        // WX.VibrateLong(null);
// #elif UNITY_ANDROID
//             AndroidVibration.Vibrate(milliseconds);
// #elif UNITY_IOS
//             IOSHelper.VibrateHeavy();            
#else
            var vibrateShortOption = new VibrateShortOption()
            {
                type = "heavy",
                complete = vibrateShortComplete,
                fail = vibrateShortFail,
                success = vibrateShortSuccess
            };
            WX.VibrateShort(vibrateShortOption);
#endif
    }
}