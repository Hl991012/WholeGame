using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageListUIPresenter : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private TextMeshProUGUI coinCountTmp;
    [SerializeField] private EnhancedScroller enhancedScroller;
    [SerializeField] private StageLevelGroup stageLevelGroup;
    [SerializeField] private Button backBtn;

    private List<List<StageConfigModel>> data = new ();

    private void Awake()
    {
        backBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            gameObject.SetActive(false);
        });
        
        enhancedScroller.Delegate = this; 
        
        data.Clear();
        var temp = StageConfigManager.Instance.AllStageConfigModels.Values.ToList();
        for (var i = 0; i < temp.Count;)
        {
            data.Add(temp.GetRange(i, Mathf.Min(3, temp.Count - i)));
            i += 3;
        }
        
        WealthManager.Instance.OnWealthChanged += RefreshCoinView;
    }

    public void RefreshView()
    {
        enhancedScroller.ReloadData();
        RefreshCoinView();
    }
    
    private void RefreshCoinView()
    {
        coinCountTmp.text = WealthManager.Instance.CurWealthModel.CoinCount.ToString();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 255;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cellView = enhancedScroller.GetCellView(stageLevelGroup);
        (cellView as StageLevelGroup)?.Init(data[dataIndex]);
        return cellView;
    }
}
