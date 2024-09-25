using TMPro;
using UnityEngine;

public class StaminaUIPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI staminaCountTmp;

    private void OnEnable()
    {
        RefreshView();
    }

    private void Awake()
    {
        StageManager.Instance.OnStaminaValueChanged += RefreshView;
    }
    
    private void RefreshView()
    {
        staminaCountTmp.text = StageManager.Instance.Stamina.ToString();
    }
}
