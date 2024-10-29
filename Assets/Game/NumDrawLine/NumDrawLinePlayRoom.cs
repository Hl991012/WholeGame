using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace NumDrawLine
{
    public class NumDrawLinePlayRoom : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform mapParent;
        [SerializeField] private Vector2Int mapSize;
        [SerializeField] private SingleCellItem singleCellItemPrefab;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private ParticleSystem mergeEffect;
        
        private SingleCellItem[,] map;

        private int[,] mapSaveData;

        private void Start()
        {
            map = new SingleCellItem[mapSize.x,mapSize.y];
            mapSaveData = new int[mapSize.x, mapSize.y];
            InitMap();
        }

        // 初始化格子
        private void InitMap()
        {
            // 创建格子,判断有没有存档，如果有存档，则加载存档，否则随机生成对应的元素
            var tempSaveMapData = NumDrawLineGameManager.Instance.Data.MapData;
            if (tempSaveMapData != null)
            {
                for (var i = 0; i < mapSize.x; i++)
                {
                    for (var j = 0; j < mapSize.y; j++) 
                    {
                        var cell = Instantiate(singleCellItemPrefab, mapParent);
                        var cellDataModel = new CellDataModel()
                        {
                            Coordinate = new Vector2Int(i, j),
                            Num = tempSaveMapData[i, j]
                        };
                        cell.Register(cellDataModel, OnPointerDown, OnPointerUp, OnPointerEnter, OnPointerExit, OnPointerClick);
                        map[i, j] = cell;
                        cell.SetPos(CoordinateToAnchorPos(new Vector2Int(i, j)), false);
                    }
                }
            }
            else
            {
                for (var i = 0; i < map.GetLength(0); i++)
                {
                    for (var j = 0; j < map.GetLength(1); j++)
                    {
                        CreatNewCellItem(new Vector2Int(i, j));
                    }
                }
                SaveGameState();
            }
        }
    
        // 生成随机的数字
        private int GetRandomNum()
        {
            var temp = Random.Range(1, 7);
            return (int)Mathf.Pow(2, temp);
        }

        #region CellItem的操作

        private SingleCellItem firstChooseItem;

        private SingleCellItem lastChooseItem;

        private readonly List<SingleCellItem> chooseItems = new ();

        private void OnPointerDown(PointerEventData eventData, SingleCellItem singleCellItem)
        {
            if(curUseBoosterType != BoosterType.None) return;
        
            chooseItems.Clear();
            firstChooseItem = singleCellItem;
            lastChooseItem = singleCellItem;
            chooseItems.Add(singleCellItem);
            RefreshLine();
            needRefreshLine = true;
            singleCellItem.ShowChooseAnim();
        }

        private void OnPointerEnter(PointerEventData eventData, SingleCellItem singleCellItem)
        {
            if(curUseBoosterType != BoosterType.None) return;
        
            if(firstChooseItem == null) return;
        
            // 判断是否在最后一个元素的周围
            if (IsInAround(lastChooseItem.CellDataModel.Coordinate, singleCellItem.CellDataModel.Coordinate))
            {
                // 如果等于最后一个的前一个，则取消当前选择的最后一个
                if (chooseItems.Count > 1 && chooseItems[^2] == singleCellItem)
                {
                    chooseItems.RemoveAt(chooseItems.Count - 1);
                    lastChooseItem = chooseItems[^1];
                    RefreshLine();
                }
                else
                {
                    if(chooseItems.Contains(singleCellItem)) return;
                
                    // 如果只选择了一个，则判断当前数字是否相等，相等才能增加
                    if (chooseItems.Count == 1 && chooseItems[0].CellDataModel.Num == singleCellItem.CellDataModel.Num)
                    {
                        lastChooseItem = singleCellItem;
                        chooseItems.Add(singleCellItem);
                        RefreshLine();
                        singleCellItem.ShowChooseAnim();
                    }
                    else if(chooseItems.Count > 1) // 如果只选择了多个，则判断当前数字是否相等或者是当期数字的二倍
                    {
                        if (lastChooseItem.CellDataModel.Num * 2 == singleCellItem.CellDataModel.Num ||
                            lastChooseItem.CellDataModel.Num == singleCellItem.CellDataModel.Num)
                        {
                            lastChooseItem = singleCellItem;
                            chooseItems.Add(singleCellItem);
                            RefreshLine();
                            singleCellItem.ShowChooseAnim();
                        }
                    }
                }
            }
        }

        private void OnPointerUp(PointerEventData eventData, SingleCellItem singleCellItem)
        {
            if(curUseBoosterType != BoosterType.None) return;
        
            // 合并选择的列表
            if (chooseItems.Count >= 3)
            {
                canvasGroup.blocksRaycasts = false;
                Observable.Timer(TimeSpan.FromSeconds(0.6f)).Subscribe(_ =>
                {
                    canvasGroup.blocksRaycasts = true;
                }).AddTo(this);
                
                if (chooseItems.Count >= 5)
                {
                    mapParent.DOShakePosition(0.35f, Vector3.one * 20, 20, 270, true);
                }
            
                lastChooseItem.SetNum(CalculateNum(chooseItems));
                foreach (var item in chooseItems)
                {
                    // 销毁除了最后的一个元素
                    if (item != lastChooseItem)
                    {
                        item.transform.SetAsLastSibling();
                        item.ShowBeMergeAnim(lastChooseItem.transform.position);
                        map[item.CellDataModel.Coordinate.x, item.CellDataModel.Coordinate.y] = null;
                    }
                }

                //播放粒子动画
                mergeEffect.transform.position = lastChooseItem.transform.position;
                mergeEffect.Play();
                lastChooseItem.ShowMergeAnim();
                
                UpdateCellItems();
            }
            firstChooseItem = null;
            lastChooseItem = null;
            chooseItems.Clear();
            needRefreshLine = false;
            lineRenderer.positionCount = 0;
        }

        private void OnPointerExit(PointerEventData eventData, SingleCellItem singleCellItem)
        {
            
        }
        
        private void OnPointerClick(PointerEventData eventData, SingleCellItem singleCellItem)
        {
            switch (curUseBoosterType)
            {
                case BoosterType.None:
                    break;
                case BoosterType.Destroy:
                    map[singleCellItem.CellDataModel.Coordinate.x, singleCellItem.CellDataModel.Coordinate.y] = null;
                    Destroy(singleCellItem.gameObject);
                    UpdateCellItems();
                    break;
            }

            curUseBoosterType = BoosterType.None;
            OnBoosterStateChanged?.Invoke(curUseBoosterType);
        }

        // 计算合成后的数字，计算规则：所有的数字加起来,最后向上得到最接近的2的n次方
        private int CalculateNum(List<SingleCellItem> cellItems)
        {
            var tempNum = 0;
            foreach (var item in cellItems)
            {
                tempNum += item.CellDataModel.Num;
            }
        
            return Mathf.NextPowerOfTwo(tempNum);
        }
    
        // 计算合成后增加的分数，计算规则：所有的数字加起来
        private int CalculateScore(List<SingleCellItem> cellItems)
        {
            var tempScore = 0;
            foreach (var item in cellItems)
            {
                tempScore += item.CellDataModel.Num;
            }
        
            return tempScore;
        }
    

        // 更新最新的所有的cellItem状态
        private void UpdateCellItems()
        {
            // 利用双指针计算竖排第一个空的位置以及下一个不空的位置，将不为空的位置的元素移动到第一个空的位置
            for (var i = 0; i < mapSize.x; i++)
            {
                var temp = 0;
                for (var j = 0; j < mapSize.y; j++)
                {
                    var nextNotEmptyIndex = mapSize.y;
                    if (map[i, j] == null)
                    {
                        var firstEmptyIndex = j;
                        // 找到下一个不为空的填入这个位置
                        for (var k = j + 1; k < mapSize.y; k++)
                        {
                            if (map[i, k] != null)
                            {
                                nextNotEmptyIndex = k;
                                break;
                            }
                        }

                        if (nextNotEmptyIndex >= mapSize.y)
                        {
                            // 生成新的item
                            CreatNewCellItem(new Vector2Int(i, firstEmptyIndex), new Vector2Int(i, nextNotEmptyIndex + temp), true);
                            temp++;
                        }
                        else
                        {
                            // 将原来的item移动到第一个空的位置
                            var tempCell = map[i, nextNotEmptyIndex];
                            map[i, firstEmptyIndex] = tempCell;
                            map[i, nextNotEmptyIndex] = null;
                            tempCell.CellDataModel.Coordinate = new Vector2Int(i, firstEmptyIndex);
                            tempCell.SetPos(CoordinateToAnchorPos(new Vector2Int(i, firstEmptyIndex)), true);
                        }
                    }
                }
            }
            SaveGameState();
            if (CheckIsGameOver())
            {
                Debug.LogError("游戏失败");
            }
        }

        private bool CheckIsGameOver()
        {
            // 计算规则，只要存在可以连续消除的三个元素就不算输
            for (var i = 0; i < mapSize.x; i++)
            {
                for (var j = 0; j < mapSize.y; j++)
                {
                    if (map[i, j] == null) continue;
                    var cellItem = map[i, j];
                    var tempCellList = GetCellCanMergeItems(cellItem, true);
                    foreach (var item in tempCellList)
                    {
                        var temp = GetCellCanMergeItems(item, false);
                        if (temp.Contains(cellItem))
                        {
                            temp.Remove(cellItem);   
                        }

                        if (temp.Count > 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    
        private List<SingleCellItem> GetCellCanMergeItems(SingleCellItem singleCellItem, bool needEqual)
        {
            var tempCellList = new List<SingleCellItem>();
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    var tempX = singleCellItem.CellDataModel.Coordinate.x + i;
                    var  tempY = singleCellItem.CellDataModel.Coordinate.y + j;

                    if (tempX >= 0 && tempX < mapSize.x && tempY >= 0 && tempY < mapSize.y)
                    {
                        if (map[tempX, tempY] != null)
                        {
                            if (needEqual && singleCellItem.CellDataModel.Num == map[tempX, tempY].CellDataModel.Num)
                            {
                                tempCellList.Add(map[tempX, tempY]);
                            }
                            else if (!needEqual && singleCellItem.CellDataModel.Num * 2 == map[tempX, tempY].CellDataModel.Num)
                            {
                                tempCellList.Add(map[tempX, tempY]);
                            }
                        }
                    }
                }
            }

            return tempCellList;
        }

        private void CreatNewCellItem(Vector2Int coordinate, Vector2Int creatPos = new Vector2Int() , bool needMoveAnim = false)
        {
            var cell = Instantiate(singleCellItemPrefab, mapParent);
            var cellDataModel = new CellDataModel()
            {
                Coordinate = new Vector2Int(coordinate.x, coordinate.y),
                Num = GetRandomNum()
            };
            cell.Register(cellDataModel, OnPointerDown, OnPointerUp, OnPointerEnter, OnPointerExit, OnPointerClick);
            map[coordinate.x, coordinate.y] = cell;

            if (needMoveAnim)
            {
                cell.SetPos(CoordinateToAnchorPos(creatPos), false);
                cell.SetPos(CoordinateToAnchorPos(coordinate), true);
            }
            else
            {
                cell.SetPos(CoordinateToAnchorPos(coordinate), false);
            }
        }
    
        #endregion

        #region 刷新连线
    
        private bool needRefreshLine;

        private Camera mainCamera;

        private void RefreshLine()
        {
            lineRenderer.positionCount = chooseItems.Count + 1;
            for (var i = 0; i < chooseItems.Count; i++)
            {
                lineRenderer.SetPosition(i, chooseItems[i].AnchorPos);
            }
        }

        private void RefreshLineEndPos()
        {
            if(!needRefreshLine) return;
        
            mainCamera??= Camera.main;
            if (mainCamera != null && lineRenderer.positionCount > chooseItems.Count)
            {
                // var screenPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
                // screenPoint.z = -10;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mapParent, Input.mousePosition, mainCamera,
                    out var anchorPos);
                lineRenderer.SetPosition(chooseItems.Count, anchorPos);
            }
        }

        #endregion

        // 判断一个元素是否在另一个元素周围一个坐标的距离
        private bool IsInAround(Vector2Int center, Vector2Int targetPos)
        {
            return Mathf.Abs(center.x - targetPos.x) <= 1 && Mathf.Abs(center.y - targetPos.y) <= 1;
        }
    
        private const float Space = 10; // cellItem中间的space
        private const float CellWidth = 100; // cellItem的宽度

        // 坐标转位置
        private Vector2 CoordinateToAnchorPos(Vector2Int coordinate)
        {
            // 计算x
            float x = 0;
            float y = 0;
            // 计算x坐标
            if (mapSize.x % 2 == 0)
            {
                // 在坐标轴左边
                if (coordinate.x < mapSize.x / 2)
                {
                    var index = (mapSize.x / 2) - coordinate.x;
                    x = -((index - 1) * (CellWidth + Space) + CellWidth / 2f + Space / 2f);
                }
                // 在坐标轴右边
                else 
                {
                    var index = coordinate.x - (mapSize.x / 2) + 1;
                    x = (index - 1) * (CellWidth + Space) + CellWidth / 2f + Space / 2f;
                }
            }
            else
            {
                // 在坐标轴左边
                if (coordinate.x <= mapSize.x / 2)
                {
                    var index = (mapSize.x / 2) - coordinate.x;
                    x = -(index * (CellWidth + Space));
                }
                // 在坐标轴右边
                else
                {
                    var index = coordinate.x - mapSize.x / 2;
                    x = index * (CellWidth + Space);
                }
            }
            
            // 计算y坐标
            if (mapSize.y % 2 == 0)
            {
                // 在坐标轴下边
                if (coordinate.y < mapSize.y / 2)
                {
                    var index = (mapSize.y / 2) - coordinate.y;
                    y = -((index - 1) * (CellWidth + Space) + CellWidth / 2f + Space / 2f);
                }
                // 在坐标轴上边
                else
                {
                    var index = coordinate.y - (mapSize.y / 2) + 1;
                    y = (index - 1) * (CellWidth + Space) + CellWidth / 2f + Space / 2f;
                }
            }
            else
            {
                // 在坐标轴下边
                if (coordinate.y <= mapSize.y / 2)
                {
                    var index = (mapSize.y / 2) - coordinate.y;
                    y = -(index * (CellWidth + Space));
                }
                // 在坐标轴上边
                else
                {
                    var index = coordinate.y - mapSize.y / 2;
                    y = index * (CellWidth + Space);
                }
            }
        
            return new Vector2(x, y);
        }

        private void Update()
        {
            RefreshLineEndPos();
        }

        #region 使用道具相关
    
        private BoosterType curUseBoosterType;

        public Action<BoosterType> OnBoosterStateChanged;

        public void UserBooster(BoosterType boosterType)
        {
            // 判断道具是否足够
            curUseBoosterType = boosterType;
            // switch (boosterType)
            // {
            //     case 
            // }
            OnBoosterStateChanged?.Invoke(curUseBoosterType);
        }

        #endregion

        #region 数据保存相关
        
        
        private void SaveGameState()
        {
            for (var i = 0; i < map.GetLength(0); i++)
            {
                for (var j = 0; j < map.GetLength(1); j++)
                {
                    mapSaveData[i, j] = map[i, j].CellDataModel?.Num ?? 2;
                }
            }
            NumDrawLineGameManager.Instance.UpdateGameMapData(mapSaveData);
        }

        #endregion
    }

    public enum BoosterType
    {
        None,
        Destroy,
    }
}