using System;
using System.Collections.Generic;
using BlockEliminateGame;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NMNH;
using PutBlockGame;
using TMPro;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class SingleTargetItem : MonoBehaviour
{
    [SerializeField] private Image targetIcon;
    [SerializeField] private TextMeshProUGUI targetCount;
    [SerializeField] private GameObject completeHintObj;
    [SerializeField] private Image completeTargetIcon;
    
    private int remainTargetCount = 0;

    private GameModel.StageTargetItemModel curStageTargetModel;

    private void Awake()
    {
        PlayRoomPresenter.Instance.BlockEliminateGame.OnCollectBarrier += OnCollectBarrier;
    }
    
    public SingleTargetItem Init(GameModel.StageTargetItemModel model)
    {
        curStageTargetModel = model;
        return this; 
    }

    public void RefreshView()
    {
        if (curStageTargetModel == null) return;
        
        // 刷新icon
        targetIcon.sprite = ResourcesCenter.Instance.GetBarrierIcon(curStageTargetModel.BarrierType);
        targetIcon.preserveAspect = true;
        remainTargetCount = curStageTargetModel.TargetCount;
        RefreshTarget();
    }

    private void RefreshTarget()
    {
        if (curStageTargetModel == null) return;
        targetCount.text = remainTargetCount > 0 ? remainTargetCount.ToString() : "";
        completeHintObj.SetActive(remainTargetCount <= 0);
    }

    public void PlayIconFlyAni(GameModel.BarrierType barrierType,  Vector3 genPos)
    {
        if(curStageTargetModel == null) return;
        var blowUpEndValue = 0.8f;
        var scaleEndValue = 0.34f; 
        
        switch (barrierType)
                {
                    // 这些类型是收集物类型，需要生成一张图片飞向目标
                    default:
                        // 生成一张图片然后飞向目标位置
                        var tempImage1 = Instantiate(completeTargetIcon, genPos, Quaternion.identity, transform);
                        tempImage1.sprite = targetIcon.sprite;
                        tempImage1.preserveAspect = true;
                        tempImage1.transform.localScale = Vector3.zero;
                        tempImage1.gameObject.SetActive(true);
           
                        DOTween.Sequence().Append(tempImage1.transform.DOScale(blowUpEndValue, 0.5f).SetEase(Ease.OutElastic))
                            .Insert(0.4f, tempImage1.transform.DOMove(targetIcon.transform.position, 0.3f).SetEase(Ease.InQuart))
                            .Insert(0.5f, tempImage1.transform.DOScale(scaleEndValue, 0.2f))
                            .SetLink(gameObject)
                            .SetUpdate(true)
                            .OnComplete(() =>
                            {
                                Destroy(tempImage1.gameObject);
                                remainTargetCount--;
                                remainTargetCount = Mathf.Max(0, remainTargetCount);
                                RefreshTarget();
                                PlayTargetAnim();
                            });
                        break;
                }
    }
    
    private Tween tween;
    
    private void PlayTargetAnim()
    {
        var parent = targetCount.transform.parent;
        parent.localScale = Vector3.one;
        tween ??= parent.DOPunchScale(new Vector3(-0.1f, 0.2f, -0.1f), 0.5f)
            .SetAutoKill(false)
            .SetUpdate(true)
            .SetLink(gameObject);
        tween.Restart();
    }

    private void OnCollectBarrier(GameModel.BarrierType barrierType, Vector3 pos)
    {
        if(curStageTargetModel == null || curStageTargetModel.BarrierType != barrierType) return;
        PlayIconFlyAni(barrierType, pos);
    }
}
