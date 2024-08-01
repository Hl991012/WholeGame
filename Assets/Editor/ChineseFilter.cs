using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ChineseFilter
{
    [MenuItem("RavenKit/ChineseFilter")]
    public static void BuildAssetBundlesByTarget()
    {
        int progress = 0;

        EditorUtility.DisplayProgressBar("过滤中", "进度：" + progress * 100 + "%", progress);

        List<string> result = new List<string>();
        result.Add(_commonText);

        progress = 40;
        var files = FilterFiles(
            new List<string> { "Assets/Game"},
            new List<string> { ".cs", ".json", ".csv" });
        result = result.Union(files).ToList<string>();
        EditorUtility.DisplayProgressBar("过滤中", "进度：" + progress + "%", progress);

        progress = 60;
        files = FilterScenes(new List<string> { "Assets/Game" });
        result = result.Union(files).ToList<string>();
        EditorUtility.DisplayProgressBar("过滤中", "进度：" + progress + "%", progress);

        progress = 80;
        files = FilterPrefabs(new List<string> { "Assets/Game" });
        result = result.Union(files).ToList<string>();

        string savePath = "Assets/Chinese.txt";
        SaveText(savePath, result);
        EditorUtility.DisplayProgressBar("过滤中", "进度：" + progress + "%", progress);

        progress = 100;
        EditorUtility.DisplayProgressBar("过滤中", "进度：" + progress + "%", progress);

        EditorUtility.ClearProgressBar();
    }

    private static string _commonText = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private static List<string> FilterFiles(List<string> pathList, List<string> extList)
    {
        List<string> result = new List<string>();

        foreach (var path in pathList)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var fileInfo in fileInfos)
            {
                string filePath = fileInfo.FullName;
                string ext = Path.GetExtension(filePath);
                if (extList.IndexOf(ext) != -1)
                {
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    foreach (var line in lines)
                    {
                        if (line.Contains("//") || line.Contains("/*") || line.Contains("*/")
                            || line.Contains("#region") || line.Contains("* @") || line.Contains("Debug."))
                        {
                            continue;
                        }

                       //  if (IsContainChinese(line))
                        {
                            result.Add(GetChinese(line));
                        }
                    }
                }
            }
        }

        return result;
    }

    private static bool IsContainChinese(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (Convert.ToInt32(Convert.ToChar(text.Substring(i, 1))) > Convert.ToInt32(Convert.ToChar(128)))
            {
                return true;
            }
        }

        return false;
    }

    private static string GetChinese(string text)
    {
        string chinese = "";
        for (int i = 0; i < text.Length; i++)
        {
            if (Convert.ToInt32(Convert.ToChar(text.Substring(i, 1))) > Convert.ToInt32(Convert.ToChar(128)))
            {
                chinese += text.Substring(i, 1);
            }
        }

        return chinese;
    }

    private static List<string> FilterScenes(List<string> searchInFolders)
    {
        List<string> result = new List<string>();

        List<string> assetsPath = GetAssetsPath("t:Scene", searchInFolders.ToArray());
        foreach (var variable in assetsPath)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(variable);
            var texts = GameObject.FindObjectsOfType<TMP_Text>();
            foreach (var text in texts)
            {
                // if (IsContainChinese(text.text))
                {
                    result.Add(GetChinese(text.text));
                }
            }
        }

        return result;
    }

    private static List<string> FilterPrefabs(List<string> searchInFolders)
    {
        List<string> result = new List<string>();

        List<string> assetsPath = GetAssetsPath("t:Prefab", searchInFolders.ToArray());
        foreach (var variable in assetsPath)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(variable);
            var texts = go.GetComponentsInChildren<TMP_Text>();
            foreach (var text in texts)
            {
                if (IsContainChinese(text.text))
                {
                    result.Add(GetChinese(text.text));
                }
            }
        }

        return result;
    }

    private static List<string> GetAssetsPath(string filter, string[] searchInFolders)
    {
        List<string> assetPath = new List<string>();

        string[] result = AssetDatabase.FindAssets(filter, searchInFolders);
        foreach (var variable in result)
        {
            string path = AssetDatabase.GUIDToAssetPath(variable);
            assetPath.Add(path);
        }

        return assetPath;
    }

    private static void SaveText(string path, List<string> date)
    {
        StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8);
        for (int i = 0; i < date.Count; i++)
        {
            streamWriter.Write(date[i]);
            if (i != date.Count - 1)
            {
                streamWriter.Write("\r\n");
            }
        }

        streamWriter.Close();
    }
}