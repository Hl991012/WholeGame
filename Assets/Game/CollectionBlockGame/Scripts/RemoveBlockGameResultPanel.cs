using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoveBlockGameResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject infoObj;
    [SerializeField] private GameObject passObj;
    [SerializeField] private GameObject loseObj;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button nextLevelBtn;
    [SerializeField] private Button onceAgainBtn;
    [SerializeField] private Button shareBtn;

    private void Awake()
    {
        nextLevelBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            RemoveBlockGameManager.Instance.RemoveBlockGamePlayRoom.StartGame();
            infoObj.SetActive(false);
        });
        
        onceAgainBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            RemoveBlockGameManager.Instance.RemoveBlockGamePlayRoom.StartGame();
            infoObj.SetActive(false);
        });
        
        shareBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.Share();
        });
    }

    public void Show(bool isWin)
    {
        infoObj.SetActive(true);
        passObj.SetActive(isWin);
        loseObj.SetActive(!isWin);
        title.text = isWin ? "胜利" : "失败";
    }
}
