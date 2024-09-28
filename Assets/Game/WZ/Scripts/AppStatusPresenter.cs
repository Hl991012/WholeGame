using System;
using UnityEngine;

public class AppStatusPresenter : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            TextAdventureGameController.Instance.SaveData();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        
    }

    private void OnApplicationQuit()
    {
        TextAdventureGameController.Instance.SaveData();
    }
}
