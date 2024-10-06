using Game.Manager;
using UnityEngine;
using WeChatWASM;

public class VibrateHelper
{
    public static void Vibrate()
    {
        if (!SettingManager.IsVibrationOpen) return;
        VibrateMedium();
    }

    public static void WeakVibrate()
    {
        if (!SettingManager.IsVibrationOpen) return;
        VibrateLight();
    }

    public static void VibrateHeavy()
    {
        if (!SettingManager.IsVibrationOpen) return;
        VibrateHeavy();
    }
    
    private static void VibrateMedium(long milliseconds = 20)
    {
        if (!SettingManager.IsVibrationOpen) return;
        
#if UNITY_EDITOR
        // WX.VibrateShort(null);
// #elif UNITY_ANDROID
//             AndroidVibration.Vibrate(milliseconds);
// #elif UNITY_IOS
//             IOSHelper.VibrateMedium();            
#else
            WX.VibrateShort(null);
#endif
    }

    private static void VibrateHeavy(long milliseconds = 20)
    {
        if (!SettingManager.IsVibrationOpen) return;
#if UNITY_EDITOR
        // WX.VibrateLong(null);
// #elif UNITY_ANDROID
//             AndroidVibration.Vibrate(milliseconds);
// #elif UNITY_IOS
//             IOSHelper.VibrateHeavy();            
 #else
            WX.VibrateShort(null);
#endif
    }

    private static void VibrateLight()
    {
        if (!SettingManager.IsVibrationOpen) return;
 #if UNITY_EDITOR
//         // WX.VibrateShort(null);
// #elif UNITY_ANDROID
//             AndroidVibration.Vibrate(milliseconds);
// #elif UNITY_IOS
//             IOSHelper.VibrateLight();            
#else
            WX.VibrateShort(null);
#endif
    }
}