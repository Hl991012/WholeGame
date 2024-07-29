// using Game.Helper;
// using Game.Manager;
// using NMNH.Utility;
//
// namespace Game.Helper
// {
//     public class VibrateHelper
//     {
//         public static void Vibrate()
//         {
//             if (!SettingManager.IsVibrationOpen) return;
//             BaseUtilities.VibrateMedium();
//         }
//
//         public static void WeakVibrate()
//         {
//             if (!SettingManager.IsVibrationOpen) return;
//             BaseUtilities.VibrateLight();
//         }
//
//         public static void VibrateHeavy()
//         {
//             if (!SettingManager.IsVibrationOpen) return;
//             BaseUtilities.VibrateHeavy();
//         }
//
//         public static void StartContinuityVibration(float delayTime, float deltaSecond)
//         {
//             GameHelper.StartContinuityVibration(delayTime, deltaSecond);
//         }
//
//         public static void StopContinuityVibration()
//         {
//             GameHelper.StopContinuityVibration();
//         }
//     }
// }