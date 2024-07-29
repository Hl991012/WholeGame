// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Security.Cryptography;
// using System.Text;
// using Game.Manager;
// using NMNH.Utility;
// using UniRx;
// using UnityEngine;
//
// namespace Game.Helper
// {
//     public static class GameHelper
//     {
//         private static Dictionary<string, IDisposable> vibrationDisposableDict = new Dictionary<string, IDisposable>();
//
//         private const string K = "GameHelper221017";
//         public static string CurrentJmKey => K + K;
//
//         private struct Keys
//         {
//             internal const string LATEST_LOGIN_DAY_KEY = GameHelper.STORAGE_KEY_PREFIX + GameHelper.StoragePathKey.GAME_HELPER + "LLDK";
//             internal const string TOTAL_LOGIN_DAY_COUNT = GameHelper.STORAGE_KEY_PREFIX + GameHelper.StoragePathKey.GAME_HELPER + "TLDC";
//             //// 3.0版本添加
//             //internal const string LATEST_LAUNCH_VERSION_KEY = GameHelper.STORAGE_KEY_PREFIX + GameHelper.StoragePathKey.GAME_HELPER + "LLVK";
//             //// 3.0版本添加
//             //internal const string FIRST_LAUNCH_VERSION_KEY = GameHelper.STORAGE_KEY_PREFIX + GameHelper.StoragePathKey.GAME_HELPER + "FLVK";
//         }
//
//         //public static void SendGaming(string category, string eventName, string label, object value = null, Dictionary<string, object> extra = null)
//         //{
//         //    var e = new Dictionary<string, object>();
//         //    if (extra != null && extra.Count > 0)
//         //    {
//         //        foreach (var pair in extra)
//         //        {
//         //            e[pair.Key] = pair.Value;
//         //        }
//         //    }
//
//         //    Tracker.Send(category, eventName, label, value, e);
//         //}
//         
//         //数量显示成字符串
//         public static string DisplayCount(int count)
//         {
//             if (count >= 10000)
//             {
//                 return $"{count / 1000}K";
//             }
//
//             return count.ToString();
//         }
//
//         #region 震动和音效
//
//         public static void StartContinuityVibration(float delayTime, float deltaSecond = 0.5f)
//         {
//             StopContinuityVibration();
//             var vibrationDisposable = Observable.Timer(TimeSpan.FromSeconds(delayTime), TimeSpan.FromSeconds(deltaSecond))
//                 .Subscribe(_ => VibrateHelper.WeakVibrate());
//             vibrationDisposableDict[Time.time.ToString()] = vibrationDisposable;
//         }
//
//         public static void StopContinuityVibration()
//         {
//             foreach (var pair in vibrationDisposableDict)
//             {
//                 pair.Value.Dispose();
//             }
//         }
//
//         public static void PlayClickSoundAndVibration()
//         {
//             SEPlayer.Instance.PlayOneShot(SEPlayer.SoundEffectType.CommonClick);
//             VibrateHelper.Vibrate();
//         }
//
//         public static void PlayClickErrorSoundAndVibration()
//         {
//             SEPlayer.Instance.PlayOneShot(SEPlayer.SoundEffectType.ClickError);
//             VibrateHelper.Vibrate();
//         }
//
//         #endregion
//
//         #region 计算天数通用
//
//         public static string GetRemainTimeText(long endTime, string timeOverKey)
//         {
//             var remainTime = endTime - BaseUtilities.GetMillisecondTimestamp();
//             if (remainTime >= 0)
//             {
//                 var timeSpan = TimeSpan.FromMilliseconds(remainTime);
//
//                 if (timeSpan.Days > 0)
//                 {
//                     return timeSpan.Days > 1 ? I18N.TextWithArgs("day_plural", timeSpan.Days.ToString()) : I18N.TextWithArgs("day_singular", timeSpan.Days.ToString());
//                 }
//
//                 if (timeSpan.Hours > 0)
//                 {
//                     return timeSpan.Hours > 1 ? I18N.TextWithArgs("hour_plural", timeSpan.Hours.ToString()) : I18N.TextWithArgs("hour_singular", timeSpan.Hours.ToString());
//                 }
//
//                 if (timeSpan.Minutes >= 0)
//                 {
//                     return timeSpan.Minutes > 1 ? I18N.TextWithArgs("minute_plural", timeSpan.Minutes.ToString()) : I18N.TextWithArgs("minute_singular", timeSpan.Minutes.ToString());
//                 }
//             }
//
//             return I18N.Text(timeOverKey);
//         }
//
//         public static int TodayTotalCountForKey(string countKey)
//         {
//             var t = DateTime.Now;
//             var dayString = t.Year + "/" + t.Month + "/" + t.Day;
//             if (PlayerPrefs.GetString(countKey + "_DAY_KEY", "") == dayString)
//             {
//                 return PlayerPrefs.GetInt(countKey, 0);
//             }
//             else
//             {
//                 PlayerPrefs.SetString(countKey + "_DAY_KEY", dayString);
//                 PlayerPrefs.SetInt(countKey, 0);
//                 return 0;
//             }
//         }
//
//         public static void RefreshTotalLoginDayCount()
//         {
//             var t = DateTime.Now;
//             var dayString = t.Year + "/" + t.Month + "/" + t.Day;
//             if (PlayerPrefs.GetString(Keys.LATEST_LOGIN_DAY_KEY, "") != dayString)
//             {
//                 var addedCount = PlayerPrefs.GetInt(Keys.TOTAL_LOGIN_DAY_COUNT, 0) + 1;
//                 PlayerPrefs.SetInt(Keys.TOTAL_LOGIN_DAY_COUNT, addedCount);
//                 PlayerPrefs.SetString(Keys.LATEST_LOGIN_DAY_KEY, dayString);
//             }
//         }
//
//         public static void TodayNeedAddCountForKey(string countKey)
//         {
//             var t = DateTime.Now;
//             var dayString = t.Year + "/" + t.Month + "/" + t.Day;
//
//             if (PlayerPrefs.GetString(countKey + "_DAY_KEY", "") == dayString)
//             {
//                 PlayerPrefs.SetInt(countKey, PlayerPrefs.GetInt(countKey, 0) + 1);
//             }
//             else
//             {
//                 PlayerPrefs.SetString(countKey + "_DAY_KEY", dayString);
//                 PlayerPrefs.SetInt(countKey, 1);
//             }
//         }
//
//         public static void TodayNeedAddCountForKey(string countKey, int count)
//         {
//             var t = DateTime.Now;
//             var dayString = t.Year + "/" + t.Month + "/" + t.Day;
//
//             if (PlayerPrefs.GetString(countKey + "_DAY_KEY", "") == dayString)
//             {
//                 PlayerPrefs.SetInt(countKey, PlayerPrefs.GetInt(countKey, 0) + count);
//             }
//             else
//             {
//                 PlayerPrefs.SetString(countKey + "_DAY_KEY", dayString);
//                 PlayerPrefs.SetInt(countKey, 1);
//             }
//         }
//
//         #endregion
//
//         #region ====压缩jsonString====
//         public static string CompressJsonString(string json)
//         {
//             StringBuilder sb = new StringBuilder();
//             using (StringReader reader = new StringReader(json))
//             {
//                 int ch;
//                 int lastch = -1;
//                 bool isQuoteStart = false;
//                 while ((ch = reader.Read()) > -1)
//                 {
//                     if ((char)lastch != '\\' && (char)ch == '\"')
//                     {
//                         if (!isQuoteStart)
//                         {
//                             isQuoteStart = true;
//                         }
//                         else
//                         {
//                             isQuoteStart = false;
//                         }
//                     }
//                     if (!Char.IsWhiteSpace((char)ch) || isQuoteStart)
//                     {
//                         sb.Append((char)ch);
//                     }
//                     lastch = ch;
//                 }
//             }
//             return sb.ToString();
//         }
//         #endregion
//
//         #region ====读取Resources文件====
//
//         public static string ReadResourcesJsonText(string path)
//         {
//             var textAsset = Resources.Load<TextAsset>(path);
//
//             if (textAsset.IsNull() || textAsset.text.IsNullOrEmpty())
//             {
//                 // Debug.LogError("ReadResourcesJsonText path: <" + path + "> failed");
//                 return null;
//             }
//             try
//             {
//                 return Decrypt(textAsset.text, CurrentJmKey);
//             }
//             catch
//             {
//                 //LogHelper.Log(e + "ReadResourcesJsonText path: <" + path + "> failed");
//                 return textAsset.text;
//             }
//         }
//         #endregion
//
//         #region ==== PlayerPref加解密存储 =====
//         public static string GetPlayerPrefForString(string key, string defaultValue = "")
//         {
//             var storageKey = PlayerPrefs.GetString(key + ".sk", "");
//
//             if (storageKey.IsNullOrEmpty())
//             {
//                 return PlayerPrefs.GetString(key, defaultValue);
//             }
//
//             var str = PlayerPrefs.GetString(key, defaultValue);
//
//             try
//             {
//                 return Decrypt(str, storageKey);
//             }
//             catch
//             {
//                 return str;
//             }
//         }
//
//         public static void SetPlayerPrefForString(string key, string strData)
//         {
//             try
//             {
//                 PlayerPrefs.SetString(key, Encrypt(strData, CurrentJmKey));
//                 PlayerPrefs.SetString(key + ".sk", CurrentJmKey);
//             }
//             catch
//             {
//                 //LogHelper.Log(e);
//                 // Debug.LogError("SetPlayerPrefForString error, key:" + key + e);
//                 PlayerPrefs.SetString(key, strData);
//                 PlayerPrefs.SetString(key + ".sk", "");
//             }
//         }
//
//         public static int GetPlayerPrefForInt(string key, int defaultValue)
//         {
//             var storageKey = PlayerPrefs.GetString(key + ".sk", "");
//             var str = PlayerPrefs.GetString(key, "");
//             if (storageKey.IsNullOrEmpty() || str.IsNullOrEmpty())
//             {
//                 return PlayerPrefs.GetInt(key, defaultValue);
//             }
//             try
//             {
//                 return int.Parse(Decrypt(str, storageKey));
//             }
//             catch
//             {
//                 //LogHelper.Log(e);
//                 // Debug.LogError("GetPlayerPrefForInt error, key:" + key + e);
//                 return PlayerPrefs.GetInt(key, defaultValue);
//             }
//         }
//
//         public static void SetPlayerPrefForInt(string key, int dataValue)
//         {
//             try
//             {
//                 PlayerPrefs.SetString(key, Encrypt(dataValue.ToString(), CurrentJmKey));
//                 PlayerPrefs.SetString(key + ".sk", CurrentJmKey);
//             }
//             catch
//             {
//                 //LogHelper.Log(e);
//                 // Debug.LogError("SetPlayerPrefForString error, key:" + key + e);
//                 PlayerPrefs.SetInt(key, dataValue);
//                 PlayerPrefs.SetString(key + ".sk", "");
//             }
//         }
//
//         public static void DeletePlayerPrefKey(string key)
//         {
//             PlayerPrefs.DeleteKey(key);
//             PlayerPrefs.DeleteKey(key + ".sk");
//         }
//
//         #endregion
//
//         #region ========加解密========
//         /// <summary> 
//         /// 加密数据 
//         /// </summary> 
//         /// <param name="text">要加密的内容</param> 
//         /// <param name="sKey">key，必须为32位</param> 
//         /// <returns></returns> 
//         public static string Encrypt(string text, string sKey)
//         {
//
//             byte[] keyArray = Encoding.UTF8.GetBytes(sKey);
//
//             RijndaelManaged encryption = new RijndaelManaged();
//
//             encryption.Key = keyArray;
//
//             encryption.Mode = CipherMode.ECB;
//
//             encryption.Padding = PaddingMode.PKCS7;
//
//             ICryptoTransform cTransform = encryption.CreateEncryptor();
//
//             byte[] encryptArray = Encoding.UTF8.GetBytes(text);
//
//             byte[] resultArray = cTransform.TransformFinalBlock(encryptArray, 0, encryptArray.Length);
//
//             return Convert.ToBase64String(resultArray, 0, resultArray.Length);
//
//         }
//
//         /// <summary> 
//         /// 解密数据 
//         /// </summary> 
//         /// <param name="text"></param> 
//         /// <param name="sKey"></param> 
//         /// <returns></returns> 
//         public static string Decrypt(string text, string sKey)
//         {
//             byte[] keyArray = Encoding.UTF8.GetBytes(sKey);
//
//             RijndaelManaged decipher = new RijndaelManaged();
//
//             decipher.Key = keyArray;
//
//             decipher.Mode = CipherMode.ECB;
//
//             decipher.Padding = PaddingMode.PKCS7;
//
//             ICryptoTransform cTransform = decipher.CreateDecryptor();
//
//             byte[] encryptArray = Convert.FromBase64String(text);
//
//             byte[] resultArray = cTransform.TransformFinalBlock(encryptArray, 0, encryptArray.Length);
//             return Encoding.UTF8.GetString(resultArray);
//
//         }
//
//         #endregion
//
//         #region 存储Key相关
//
//         public const string STORAGE_KEY_PREFIX = "SR.SH.";
//
//         public struct StoragePathKey
//         {
//             public const string DEBUG_OPTIONAL = "DO.";
//             public const string AB_MANAGER = "AB.";
//             public const string PUSH_NOTIFICATION_MANAGER = "PNM.";
//             public const string GAME_HELPER = "GH.";
//         }
//
//         #endregion
//     }
// }
//
