using System;
using Cysharp.Threading.Tasks;
using NMNH.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashPresenter : MonoBehaviour
{
    [SerializeField] private Image progressSlider;
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        progressSlider.fillAmount = 0;
    }

    private async void Start()
    {
        await Load();
    }

    private async UniTask Load()
    {
        progressSlider.fillAmount = 0;
        WXSDKManager.Instance.Init();
        TextAdventureGameController.Instance.LoadData();
        progressSlider.fillAmount = 0.3f;
        await SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
        progressSlider.fillAmount = 0.9f;
        AudioManager.Instance.LoadAllAudioClip();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        progressSlider.fillAmount = 1;
        await SceneManager.UnloadSceneAsync("Splash");
        AudioManager.Instance.PlayBGM();
        GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
    }
}
