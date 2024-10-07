using System;
using NMNH.Utility;

public class BaseUtilities
{
    public static void PlayCommonClick()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.CommonClick);
        // VibrateHelper.WeakVibrate();
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
}