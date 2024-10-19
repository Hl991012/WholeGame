using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawLineGameRoomUIPresenter : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private Button helpBtn;
    [SerializeField] private TextMeshProUGUI helpBtnRemainCount;
    [SerializeField] private GameObject watchAdObj;
    [SerializeField] private GameObject countShowObj;

    private void Awake()
    {
        DrawLineGameConctrol.Instance.Register(this);
        
        backBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
        });
        
        helpBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            DrawLineGameConctrol.Instance.DrawLineGameRoomPresenter.UseHelpBooster();
        });
    }

    public void RefreshView()
    {
        
    }
}
