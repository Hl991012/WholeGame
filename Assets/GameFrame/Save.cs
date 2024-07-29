using System;
using UnityEngine;

namespace Game.Save
{
    public static class JsonExtension
    {
        public static T Deserialize<T>(this string json) where T : class, new()
        {
            return json is null or "" ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public static string Serialize<T>(this T obj) where T : class, new()
        {
            return obj is null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }

    public readonly struct SaveKey<T> where T : class, new()
    {
        private readonly string key;

        public SaveKey(string key) => this.key = key;

        public T Load(T defaultValue)
        {
            if (key is null or "") throw new Exception("Key为null或空");

            try
            {
                return PlayerPrefs.GetString(key, null).Deserialize<T>() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public void Save(T model)
        {
            if(model == null) return;
            
            if (key is null or "") return;

            if (model.Serialize() is { } json)
            {
                PlayerPrefs.SetString(key, json);
            }
        }

        public void DeleteKey()
        {
            PlayerPrefs.DeleteKey(key);
        }

        public bool HasSavedData()
        {
            return PlayerPrefs.HasKey(key);
        }

        public static implicit operator string(SaveKey<T> saveKey) => saveKey.key;

        public override string ToString() => key;
    }
}