using System;
using Game.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button vibrationButton;
    [SerializeField] private GameObject soundEffectOn;
    [SerializeField] private GameObject soundEffectOff;
    [SerializeField] private GameObject musicOn;
    [SerializeField] private GameObject musicOff;
    [SerializeField] private GameObject vibrationOn;
    [SerializeField] private GameObject vibrationOff;

    [SerializeField] private TextMeshProUGUI versionText;

    protected  void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            gameObject.SetActive(false);
        });
        
        soundEffectButton.onClick.AddListener(() =>
        {
            SettingManager.SwitchSoundEffect();
            RefreshSoundEffectButton();
            BaseUtilities.PlayCommonClick();
        });
        
        musicButton.onClick.AddListener(() =>
        {
            SettingManager.SwitchMusic();
            RefreshMusicButton();
            BaseUtilities.PlayCommonClick();
        });
        
        vibrationButton.onClick.AddListener(() =>
        {
            SettingManager.SwitchVibration();
            RefreshVibrationButton();
            BaseUtilities.PlayCommonClick();
        });
        

        versionText.text = "v" + Application.version;
    }

    private void Start()
    {
        RefreshSoundEffectButton();
        RefreshMusicButton();
        RefreshVibrationButton();
    }

    private void RefreshSoundEffectButton()
    {
        soundEffectOn.SetActive(SettingManager.IsSoundEffectOpen);
        soundEffectOff.SetActive(!SettingManager.IsSoundEffectOpen);
    }
    private void RefreshMusicButton()
    {
        musicOn.SetActive(SettingManager.IsMusicOpen);
        musicOff.SetActive(!SettingManager.IsMusicOpen);
    }
    private void RefreshVibrationButton()
    {
        vibrationOn.SetActive(SettingManager.IsVibrationOpen);
        vibrationOff.SetActive(!SettingManager.IsVibrationOpen);
    }
}
