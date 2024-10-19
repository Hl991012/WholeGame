using System.Collections.Generic;
using NMNH.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawLineGameRoomPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelTmp;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GridLayoutGroup mapGridLayoutGroup;
    [SerializeField] private CellItem cellPrefab;
    [SerializeField] private DrawLineGameOverPanel drawLineGameOverPanel;
    [SerializeField] private Color[] colors;
    
    private List<CellItem> mapCellData = new ();
    private int mapWidth;
    private int mapHeight;
    
    private Color curColor = Color.white;
    
    private List<CellItem> mapCells = new ();

    private void Awake()
    {
        DrawLineGameConctrol.Instance.Register(this);
    }

    public void StartGame()
    {
        RefreshView();
    }

    private void RefreshView()
    {
        InitMap(DrawLineGameConctrol.Instance.LoadConfig());
        levelTmp.text = $"第{DrawLineGameConctrol.Instance.GameLevel}关";
        drawLineGameOverPanel.gameObject.SetActive(false);
    }

    private void InitMap(MapData mapData)
    {
        curColor = colors[Random.Range(0, colors.Length)];
        mapWidth = mapData.w;
        mapHeight = mapData.h;

        if (mapCellData.Count > 0)
        {
            foreach (var item in mapCellData)
            {
                if (null != item)
                {
                    item.OnHide();
                    Destroy(item);
                }
            }
        }

        mapCellData.Clear(); 
        mapCells.Clear();
        
        mapGridLayoutGroup.constraintCount = mapWidth;
        
        var firstData = mapData.L[0];

        var index = 1;
        for (var i = 0; i < mapHeight; i++)
        {
            for (var j = 0; j < mapWidth; j++)
            {
                var pos = new Vector2Int(j, i);

                var canUse = IsContain(pos, mapData.L);
                
                var item = Instantiate(cellPrefab, mapGridLayoutGroup.transform);
                
                if (pos.x == firstData.x && pos.y == firstData.y)
                {
                    item.OnShow(index.ToString(), pos, canUse, true, curColor);
                }
                else
                {
                    item.OnShow(index.ToString(), pos, canUse, false, curColor);
                }
                
                item.RegisterEvent(OnBeginDrag, OnDrag, OnEndDrag, OnPointDown);

                mapCellData.Add(item);

                index++;
            }
        }
        
        foreach (var item in mapCellData)
        {
            if (item.CellPos.x == firstData.x && item.CellPos.y == firstData.y)
            {
                ToMove(item);
            }
        }
    }
    
    private bool IsContain(Vector2Int pos, MapPosData[] lists)
    {
        foreach (var item in lists)
        {
            if (item.x == pos.x && item.y == pos.y)
            {
                return true;
            }
        }
        return false;
    }
    
    private void ToMove(CellItem item)
    {
        if (mapCells.Count > 0)
        {
            mapCells[^1].SetPlayer(false);
        }
        
        if (!mapCells.Contains(item))
        {
            mapCells.Add(item);    
        }
        
        item.ShowDraw(true);
        item.SetPlayer(true);
        UpdatePlayerPosition(item);
        UpdateLineView();
        if (mapCells.Count > 1)
        {
            AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.PutUpBlock);   
            VibrateHelper.VibrateMedium();
        }
    }

    private void UndoMove(CellItem item)
    {
        item.ShowDraw(false);
        item.SetPlayer(false);
    }
    
    private void UpdatePlayerPosition(CellItem cell)
    {
        if (!playerObj.activeSelf)
            playerObj.SetActive(true);

        playerObj.transform.SetParent(cell.transform, false);
    }

    private void UpdateLineView()
    {
        foreach (var item in mapCellData)
        {
            item.HideAllLine();
            if (mapCells.Count >= 2)
            {
                if (mapCells.Contains(item))
                {
                    var index = mapCells.IndexOf(item);
                    var lastIndex = index - 1;
                    var nextIndex = index + 1;

                    if (index == 0 && nextIndex < mapCells.Count) // 第一个元素,只展示一个线
                    {
                        // 找到下一个元素
                        var nextItem = mapCells[nextIndex];
                        var angle = GetLineAngle(item.Position, nextItem.Position);
                        item.ShowLine(angle, CellItem.LineType.Line1);
                    }
                    else if (index == mapCells.Count - 1 && lastIndex >= 0) // 当前元素
                    {
                        // 找到上一个元素
                        var lastItem = mapCells[lastIndex];
                        var angle = GetLineAngle(item.Position, lastItem.Position);
                        item.ShowLine(angle, CellItem.LineType.curLine);
                    }
                    else
                    {
                        if (lastIndex >= 0 && nextIndex < mapCells.Count)
                        {
                            var lastItem = mapCells[lastIndex];
                            var nextItem = mapCells[nextIndex];
                            var angle1 = GetLineAngle(item.Position, nextItem.Position);
                            var angle2 = GetLineAngle(item.Position, lastItem.Position);
                            item.ShowLine(angle1, CellItem.LineType.Line1);
                            item.ShowLine(angle2, CellItem.LineType.Line2);
                        }
                    }
                }
            }
        }
    }

    private bool CheckGameStatus()
    {
        var isWin = true;
        foreach (var item in mapCellData)
        {
            if (item.IsDrawing) continue;
            isWin = false;
            break;
        }

        if (isWin)
        {
            DrawLineGameConctrol.Instance.Win();
            drawLineGameOverPanel.gameObject.SetActive(true);
            drawLineGameOverPanel.Show();
        }

        return isWin;
    }

    /// <summary>
    /// 使用提示道具
    /// </summary>
    public void UseHelpBooster()
    {
        // 先计算当前玩家选择的正确的最后一个元素
        var tempData = DrawLineGameConctrol.Instance.CurMapData;
        if (tempData is {L: {Length: > 0} })
        {
            for (var i = 0; i < mapCells.Count && i < tempData.L.Length; i++)
            {
                if (mapCells[i].CellPos.x != tempData.L[i].x || mapCells[i].CellPos.y != tempData.L[i].y)
                {
                    var tempCount = mapCells.Count - i;
                    for (var j = 0; j < tempCount; j++)
                    {
                        var deleteTarget = mapCells[^1];
                        mapCells.RemoveAt(mapCells.Count - 1);
                        UndoMove(deleteTarget);
                    }
                    break;
                }
            }
            
            // lastTarget = mapCells[i - 1];
            ToMove(mapCells[^1]);
            
            for (var j = 0; j < 3; j++)
            {
                var tempIndex = mapCells.Count;
                if (tempIndex < tempData.L.Length)
                {
                    foreach (var item in mapCellData)
                    {
                        if (item.CellPos.x == tempData.L[tempIndex].x &&
                            item.CellPos.y == tempData.L[tempIndex].y)
                        {
                            // lastTarget = item;
                            ToMove(item);
                            if (CheckGameStatus())
                            {
                                return;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DrawLineGameConctrol.Instance.GameLevel++;
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            UseHelpBooster();
        }
    }

    #region 滑动事件

    bool isBeginDrag;
    CellItem lastTarget;

    private void OnBeginDrag(PointerEventData eventData, CellItem cellItem)
    {
        if (!cellItem.IsPlayer)
            return;
        isBeginDrag = true;
        lastTarget = cellItem;
    }
    
    private void OnDrag(PointerEventData eventData, CellItem cellItem)
    {
        if (!isBeginDrag) return;

        var exchangeObj = eventData.pointerCurrentRaycast.gameObject;

        if (exchangeObj == null) return;

        if (!exchangeObj.CompareTag("Player")) return;

        var target = exchangeObj.GetComponent<CellItem>();

        if (target == null || !target.IsCanUse) return;

        if (lastTarget == target) return;
        
        if (lastTarget.CellPos.x != target.CellPos.x && lastTarget.CellPos.y != target.CellPos.y)
            return;

        if (mapCells.Contains(target))
        {
            var tempIndex = mapCells.IndexOf(target);
            var tempCount = mapCells.Count - tempIndex;
            for (var i = 0; i < tempCount; i++)
            {
                var deleteTarget = mapCells[^1];
                mapCells.RemoveAt(mapCells.Count - 1);
                UndoMove(deleteTarget);
            }
            lastTarget = target;
            ToMove(target);
        }
        else
        {
            if (lastTarget.CellPos.x == target.CellPos.x && Mathf.Abs(lastTarget.CellPos.y - target.CellPos.y) > 1)
                return;
            if (lastTarget.CellPos.y == target.CellPos.y && Mathf.Abs(lastTarget.CellPos.x - target.CellPos.x) > 1)
                return;
            lastTarget = target;
            ToMove(lastTarget);
            CheckGameStatus();
        }
    }

    private void OnEndDrag(PointerEventData eventData, CellItem cellItem)
    {
        if (!isBeginDrag) return;

        isBeginDrag = false;
    }

    private void OnPointDown(PointerEventData pointerEventData, CellItem cellItem)
    {
        if (mapCells.Contains(cellItem))
        {
            var tempIndex = mapCells.IndexOf(cellItem);
            var tempCount = mapCells.Count - tempIndex;
            for (var i = 0; i < tempCount; i++)
            {
                var deleteTarget = mapCells[^1];
                mapCells.RemoveAt(mapCells.Count - 1);
                UndoMove(deleteTarget);
            }
            lastTarget = cellItem;
            ToMove(cellItem);
        }
    }

    #endregion

    private float GetLineAngle(Vector2 from, Vector2 to)
    {
        if (Mathf.Abs(from.x - to.x) <= 0.1f)
        {
            return from.y <= to.y ? 0 : 180;
        }

        if (Mathf.Abs(from.y - to.y) <= 0.1f)
        {
            return from.x >= to.x ? 90 : 270;
        }

        return 0;
    }
}
