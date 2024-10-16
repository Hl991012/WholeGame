using System.Collections.Generic;
using UnityEngine;

public class ComboUIPresenter : MonoBehaviour
{
    [SerializeField] private List<SingleCombo> comboList;
    
    private void Start()
    {
        ComboManager.Instance.OnComboStateChanged += OnComboStateChanged;
    }

    private int curUseIndex;

    private void OnComboStateChanged(GameType gameType, ComboStateModel comboStateModel, Vector3 pos, int addScore)
    {
        var currentComboCount = comboStateModel.ComboCount;

        if (currentComboCount > 0)
        {
            comboList[curUseIndex].Show(currentComboCount, addScore);
            SetComboPos(comboList[curUseIndex], pos);
            curUseIndex++;
            if (curUseIndex >= 3)
            {
                curUseIndex = 0;
            }
        }
    }

    private void SetComboPos(Component singleCombo, Vector2 pos)
    {
        singleCombo.transform.position = pos;
        
        // 如果超出边缘，则偏移一些
        var halfScreenWidth = Screen.width / (Screen.height / 1560f) / 2f;
        var temp = 310;
        var tempRectTrans = singleCombo.transform.GetComponent<RectTransform>();
        if (halfScreenWidth - temp < tempRectTrans.anchoredPosition.x)
        {
            tempRectTrans.anchoredPosition = new Vector3(halfScreenWidth - temp, tempRectTrans.anchoredPosition.y, 0);
        }
        
        if (tempRectTrans.anchoredPosition.x < temp - halfScreenWidth)
        {
            tempRectTrans.anchoredPosition = new Vector3(temp - halfScreenWidth, tempRectTrans.anchoredPosition.y, 0);
        }
    }
    
    private void OnDestroy()
    {
        ComboManager.Instance.OnComboStateChanged -= OnComboStateChanged;
    }
}
