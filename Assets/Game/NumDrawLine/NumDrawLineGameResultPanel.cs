using NumDrawLine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumDrawLineGameResultPanel : MonoBehaviour
{
    [SerializeField] private NumDrawLinePlayRoom numDrawLinePlayRoom;
    [SerializeField] private TextMeshProUGUI curScoreTmp;
    [SerializeField] private Button shareBtn;
    [SerializeField] private Button onceAgainBtn;

    private void Awake()
    {
        shareBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.Share();
        });
        
        onceAgainBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            numDrawLinePlayRoom.ReplayGame();
            gameObject.SetActive(false);
        });
    }

    public void Show()
    {
        gameObject.SetActive(true);
        curScoreTmp.text = NumDrawLineGameManager.Instance.Data.CurScore.ToString();
    }
}
