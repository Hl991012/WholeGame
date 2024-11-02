using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleBasketItem : MonoBehaviour
{
    [SerializeField] private GameObject adIcon;
    [SerializeField] private Button unlockBtn;
    [SerializeField] private Transform blockParent;
    public bool needUnlockByAd;
    public SingleBlockItem BlockItem { get; private set; }
    public bool HasPutBlock => BlockItem != null;
    public bool HasUnlocked { get; private set; }

    private void Awake()
    {
        unlockBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    HasUnlocked = true;
                    RefreshState();
                }
            });
        });
    }

    private void OnEnable()
    {
        Clear();
    }

    public void SetBlock(SingleBlockItem block)
    {
        if(!HasUnlocked) return;
        
        if (block != null)
        {
            block.transform.SetParent(blockParent);
            block.SetBlockInteractable(false);    
        }
        
        BlockItem = block;
    }

    public void Clear()
    {
        BlockItem = null;
        // 清除生成的游戏物体
        for (var i = blockParent.childCount - 1; i >= 0 ; i--)
        {
            DestroyImmediate(blockParent.GetChild(i).gameObject);
        }

        HasUnlocked = !needUnlockByAd;
        RefreshState();
    }

    private void RefreshState()
    {
        unlockBtn.enabled = !HasUnlocked && needUnlockByAd;
        adIcon.gameObject.SetActive(!HasUnlocked && needUnlockByAd);
    }
}
