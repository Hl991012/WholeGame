using UnityEngine;
using UnityEngine.UI;

public class GameListUIPresenter : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    
    private void Awake()
    {
        backBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            gameObject.SetActive(false);
            WXSDKManager.Instance.ShowCustomAd1();
        });
    }
}
