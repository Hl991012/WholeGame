using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RemoveBlockGamePlayRoom : MonoBehaviour
{
    [SerializeField] private Transform blockItemParent;
    
    public Camera mainCamera;

    private Vector3 downPos;

    private bool isDragging;

    private SingleBlockItem chooseBlockItem;

    private int totalBlockCount; // 当前关卡生成的所有方块的数量

    private void Start()
    {
        LoadGame(2);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            chooseBlockItem = null;
            isDragging = false;
            downPos = Input.mousePosition;
            var ray = mainCamera.ScreenPointToRay(downPos);
            var rayCastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
            if (rayCastHit2D.transform != null)
            {
                rayCastHit2D.transform.parent.TryGetComponent(out chooseBlockItem);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (chooseBlockItem != null)
            {
                // 当前位置与按下位置距离大于5时视为拖拽游戏物体移动，否则视为点击
                if (Vector3.Distance(downPos, Input.mousePosition) > 5)
                {
                    // 开始移动
                    isDragging = true;
                    var tempPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    // 现在游戏物体只能在父物体范围内移动
                    tempPos.z = 0;
                    tempPos.x = Mathf.Clamp(tempPos.x, -1.78f, 1.78f);
                    tempPos.y = Mathf.Clamp(tempPos.y, -1.78f, 3.12f);
                    chooseBlockItem.SetPos(tempPos);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (chooseBlockItem != null && !isDragging)
            {
                ChooseBlockItem(chooseBlockItem);
            }
            
            chooseBlockItem = null;
            isDragging = false;
        }
    }

    // 根据需要生成组的数量(一个游戏物体需要生成三个，因为消除时需要三个匹配才能消除)，随机生成游戏物体在场景中
    private void LoadGame(int loadGroupCount)
    {
        totalBlockCount = loadGroupCount * 3;
        for (var i = 0; i < loadGroupCount; i++)
        {
            // 随机游戏物体的颜色和形状
            var colorIndex = Random.Range(0, RemoveBlockGameConfig.Instance.ColorConfigs.Count);
            var shapeIndex = Random.Range(0, RemoveBlockGameConfig.Instance.BlockItems.Count);
            var randomColor = RemoveBlockGameConfig.Instance.ColorConfigs[colorIndex];
            var randomShape = RemoveBlockGameConfig.Instance.BlockItems[shapeIndex];
            for (var j = 0; j < 3; j++)
            {
                var blockItem = Instantiate(randomShape, blockItemParent);
                // 随机游戏物体的位置,限制在父物体的范围内
                var randomPos = new Vector3(Random.Range(-1.78f, 1.78f), Random.Range(-1.78f, 3.12f), 0);
                blockItem.SetPos(randomPos);
                // 随机设置旋转和颜色
                blockItem.SetRotation(new Vector3(0, 0, Random.Range(0, 360f)));
                blockItem.SetColor(randomColor, colorIndex, shapeIndex);
                blockItem.SetBlockInteractable(true);
            }
        }

        // 再随机设置生成物体的覆盖层级
        for (var i = 0; i < blockItemParent.childCount; i++)
        {
            blockItemParent.GetChild(i).transform.SetSiblingIndex(Random.Range(0, blockItemParent.childCount - 1));
        }

        StartTimer();
    }

    #region 收集Block相关内容

    [SerializeField] private RectTransform basketParent; // 篮子父物体
    [SerializeField] private SingleBasketItem[] basketItems; // 所有的篮子
    [SerializeField] private ParticleSystem destroyEffect;
    
    private void ChooseBlockItem(SingleBlockItem blockItem)
    {
        if(blockItem == null) return;
        // 找到空篮子，将当前选择的放置在其中
        var tempBasket = GetEmptyBasket();
        if (tempBasket != null)
        {
            tempBasket.SetBlock(blockItem);
            
            // 判断插入的位置的index,将相同的元素放置在一起
            var insertIndex = -1;
            for (var i = basketItems.Length - 1; i >= 0 ; i--)
            {
                if (tempBasket != basketItems[i] && basketItems[i].HasPutBlock && basketItems[i].BlockItem.ShapeIndex == blockItem.ShapeIndex &&
                    basketItems[i].BlockItem.ColorIndex == blockItem.ColorIndex)
                {
                    insertIndex = basketItems[i].transform.GetSiblingIndex();
                    break;
                }
            }
            if (insertIndex >= 0)
            {
                tempBasket.transform.SetSiblingIndex(insertIndex + 1);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(basketParent);
            blockItem.MoveTo(tempBasket.transform.position, () =>
            {
                // 判断是否有可以合成的元素，如果有，则进行消除，如果没有，则判断是否还有空篮子，如果没有则游戏失败
                CheckHasBlockCanRemove();
                CheckIsGameOver();
            });
        }
    }

    // 检查篮子中是否有相同的元素可以进行消除
    private void CheckHasBlockCanRemove()
    {
        // 找到连续三个相同的形状和颜色进行消除
        var tempShapeIndex = -1;
        var tempColorIndex = -1;
        var sameBlockCount = 1;

        for (var i = 0; i < basketParent.childCount; i++)
        {
            var item = basketParent.GetChild(i).GetComponent<SingleBasketItem>();
            if (item.HasPutBlock)
            {
                if (tempShapeIndex == item.BlockItem.ShapeIndex && tempColorIndex == item.BlockItem.ColorIndex)
                {
                    sameBlockCount++;
                    if (sameBlockCount >= 3)
                    {
                        break;
                    }
                }
                else
                {
                    tempShapeIndex = item.BlockItem.ShapeIndex;
                    tempColorIndex = item.BlockItem.ColorIndex;
                    sameBlockCount = 1;
                }
            }
        }

        
        if (sameBlockCount >= 3)
        {
            totalBlockCount -= 3;
            var tempEmptyList = new List<int>();
            var tempSeq = DOTween.Sequence()
                .SetLink(gameObject)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    destroyEffect.Play();
                });
            // 重新进行排序
            for (var i = 0; i < basketParent.childCount; i++)
            {
                var item = basketParent.GetChild(i).GetComponent<SingleBasketItem>();
                
                if (item.HasPutBlock)
                {
                    if (item.BlockItem.ColorIndex == tempColorIndex &&
                        item.BlockItem.ShapeIndex == tempShapeIndex && sameBlockCount > 0)
                    {
                        tempEmptyList.Add(item.transform.GetSiblingIndex());

                        var tempBlock = item.BlockItem.gameObject;
                        tempSeq.Insert(0, item.BlockItem.transform.DOMove(new Vector3(0, -2.3f, 0), 0.45f))
                            .InsertCallback(0.45f, () =>
                            {
                                DestroyImmediate(tempBlock);
                            });
                        
                        item.SetBlock(null);
                        sameBlockCount--;
                    }
                    else
                    {
                        if (tempEmptyList.Count > 0)
                        {
                            tempEmptyList.Add(item.transform.GetSiblingIndex());
                            item.transform.SetSiblingIndex(tempEmptyList[0]);
                            tempEmptyList.RemoveAt(0);
                        }
                    }
                }
            }
        }
    }

    [SerializeField] private TextMeshProUGUI timerTmp;
    private IDisposable gameTimer;
    private void StartTimer()
    {
        var totalTime = 20;
        timerTmp.text = totalTime.ToString();
        gameTimer?.Dispose();
        gameTimer = Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                totalTime--;
                timerTmp.text = totalTime.ToString();
                if (totalTime <= 0)
                {
                    GameOver(totalBlockCount <= 0);
                    gameTimer?.Dispose();
                }
            }).AddTo(this);
    }

    private void CheckIsGameOver()
    {
        if (totalBlockCount <= 0)
        {
            GameOver(true);
            return;
        }
        
        var hasEmptyBasket = false;
        foreach (var item in basketItems)
        {
            if (!item.HasPutBlock)
            {
                hasEmptyBasket = true;
                break;
            }
        }

        if (!hasEmptyBasket)
        {
            GameOver(false);
        }
    }

    private void GameOver(bool isWin)
    {
        Debug.LogError("gameOver");
        // 分别处理胜利和失败的逻辑
        gameTimer?.Dispose();
        
    }
    
    // 得到空的篮子
    private SingleBasketItem GetEmptyBasket()
    {
        foreach (var item in basketItems)
        {
            if (!item.HasPutBlock)
                return item;
        }

        return null;
    }

    #endregion
}
