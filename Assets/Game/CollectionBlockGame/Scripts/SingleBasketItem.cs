using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBasketItem : MonoBehaviour
{
    [SerializeField] private Transform blockParent;
    public SingleBlockItem BlockItem { get; private set; }
    public bool HasPutBlock => BlockItem != null;

    public void SetBlock(SingleBlockItem block)
    {
        if (block != null)
        {
            block.transform.SetParent(blockParent);
            block.SetBlockInteractable(false);    
        }
        
        BlockItem = block;
    }
}
