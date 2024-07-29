using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 注意: Dialog在Addressable中的名字要和脚本一致
/// </summary>
public static class DialogManager
{
    private static Transform dialogParent;

    private static bool instantiated = false;

    private static Transform DialogParent
    {
        get
        {
            if (dialogParent == null)
            {
                if (instantiated)
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    return null;
#else
                    dialogParent = new GameObject("DialogParent").transform;
                    GameObject.DontDestroyOnLoad(dialogParent.gameObject);
                    dialogParent.tag = "DialogParent";
                    instantiated = true;
#endif
                }

                dialogParent = new GameObject("DialogParent").transform;
                GameObject.DontDestroyOnLoad(dialogParent.gameObject);
                dialogParent.tag = "DialogParent";
                instantiated = true;
            }
            return dialogParent;
        }
    }

    public static bool IsDestroyed()
    {
        return dialogParent == null;
    }

    private static readonly Dictionary<string, BaseDialog> loadedDialogs = new Dictionary<string, BaseDialog>();

    public static bool IsDialogLoaded<T>() where T : BaseDialog
    {
        var name = typeof(T).Name;
        if (loadedDialogs.ContainsKey(name) && loadedDialogs[name] != null)
        {
            return true;
        }
        return false;
    }

    public static T GetDialog<T>() where T : BaseDialog
    {
        var name = typeof(T).Name;
        if (loadedDialogs.ContainsKey(name) && loadedDialogs[name] != null && !loadedDialogs[name].HasBeDestroyed)
        {
            return loadedDialogs[name] as T;
        }
        T t = Addressables.InstantiateAsync(name, DialogParent).WaitForCompletion().GetComponent<T>();
        loadedDialogs[name] = t;
        return t;
    }

    public static bool IsDialogShowing<T>() where T : BaseDialog
    {
        var name = typeof(T).Name;
        if (loadedDialogs.ContainsKey(name) && loadedDialogs[name] != null)
        {
            var d = loadedDialogs[name];
            return d.IsShowing;
        }
        else
        {
            return false;
        }
    }

    public static bool IsAnyDialogShowing()
    {
        foreach (var pair in loadedDialogs)
        {
            if (pair.Value.IsShowing)
            {
                return true;
            }
        }
        return false;
    }

    public static void DismissAllDialog()
    {
        foreach (var pair in loadedDialogs)
        {
            if (pair.Value.IsShowing)
            {
                pair.Value.Dismiss();
            }
        }
    }
}