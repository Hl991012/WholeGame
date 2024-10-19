using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NMNH.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game2048Conctrol : MonoBehaviour
{
    [SerializeField] private GameType curGameType;
    [SerializeField] private Button backBtn;
    [SerializeField] private Transform emptyGridParent;
    [SerializeField] private Transform tileParent;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileSettings tileSettings;
    [SerializeField] private UnityEvent<int> scoreUpdated;//传递分数的事件（整数）强转,每当分数更新则调用事件
    [SerializeField] private UnityEvent<int> bestScoreUpdated;//Unity事件告诉UI最佳分数已改变
    [SerializeField] private Game2048ResultPanel game2048ResultPanel;

    [SerializeField] private Button undoBtn;
    [SerializeField] private Button restartBtn;
    
    private readonly IInputManager inputManager = new MultipleInputManager(new KeyboardInputManager(), new SwipeInputManager());//接口的好处
    private readonly Transform[,] tilePositions = new Transform[GridSize, GridSize]; // 二维数组存位置
    private readonly Tile[,] tiles = new Tile[GridSize, GridSize]; // 游戏开始时新建格子
    private static int GridSize = 4;
    
    private bool isPlayingAnim;//bug修补：进行动画禁用移动。跟踪动画->设置在没有进行动画的情况下才尝试移动。带来新bug：无法移动时还需等待。添加一个字段跟踪格子是否真实移动并更新。

    private void Awake()
    {
        undoBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    LoadLastStates();
                }
            });
        });
        
        restartBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            RestartGame();
            WXSDKManager.Instance.ShowInterstitialVideo(null);
        });
        
        backBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
        });
    }

    void Start()
    {
        GetTilePositions();
        StartGame();
    }

    //避免一直按键：确保用户在最后一帧没有按任何方向
    private int lastXInput;
    private int lastYInput;
        
    void Update()
    {
        var input = inputManager.GetInput();
        //完成x和y的输入调用trymove函数实现移动
        if (!isPlayingAnim)//更新的动画是添加此内容的最佳（简）位置
            TryMove(input.XInput, input.YInput);//浮点值并不想要：四舍五入判断是否输入 问题：抓取并尝试移动无论当前是否正在设置动画->添加一个字段更新这个点
    }

    private void StartGame()
    {
        var tempState = curGameType.GetGameData<Game2048SaveModel>().CurGameStateSaveBaseModel;
        if (tempState is not Game2048StateModel)
        {
            TrySpawnTile();
            TrySpawnTile();
        }
        else
        {
            var tempTailValues = (tempState as Game2048StateModel).tailValues;
            for (var x = 0; x < GridSize; x++)
            {
                for(var y = 0; y < GridSize; y++)
                {
                    tiles[x, y] = null;
                    if(tempTailValues[x, y] == 0) continue;
                    var tile = Instantiate(tilePrefab, tileParent);
                    tile.SetValue(tempTailValues[x, y]);
                    tiles[x, y] = tile;
                }
            }
        }
        
        UpdateTilePositions(true);
        RefreshBackBtnState();
        
        scoreUpdated.Invoke(curGameType.GetGameData<Game2048SaveModel>().CurScore);
        bestScoreUpdated.Invoke(curGameType.GetGameData<Game2048SaveModel>().BestScore);
    }

    //重启游戏
    public void RestartGame()
    {
        ClearGame();
        StartGame();
    }

    private void RefreshBackBtnState()
    {
        undoBtn.interactable = curGameType.GetGameStates().Count > 1;
    }
    
    //分数
    public void AddScore(int value)
    {
        curGameType.UpDataScore<Game2048SaveModel>(value);
        scoreUpdated.Invoke(curGameType.GetGameData<Game2048SaveModel>().CurScore);
        bestScoreUpdated.Invoke(curGameType.GetGameData<Game2048SaveModel>().BestScore);
    }

    //初始化全局位置
    private void GetTilePositions()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(emptyGridParent.GetComponent<RectTransform>()); //迫使网络布局立即计算位置和大小在初始化前进行更新
        var x = 0;
        var y = 0;
        foreach(Transform item in emptyGridParent)
        {
            tilePositions[x,y] = item;
            x++;
            if(x >= GridSize)
            {
                x = 0;
                y++;
            }
        }
    }
    private bool TrySpawnTile()
    {
        var availableSpots = new List<Vector2Int>();//指向int向量的空列表储存可用点
        //列出可用点
        for (var x = 0; x < GridSize; x++)
        {
            for(var y = 0; y < GridSize; y++)
            {
                if (tiles[x, y] == null)
                    availableSpots.Add(new Vector2Int(x, y));
            }
        }
        
        //检查
        if (!availableSpots.Any())
            return false;
        //获得一个随机索引
        var randomIndex = Random.Range(0, availableSpots.Count);
        var spot = availableSpots[randomIndex]; 
        //manager类拓展了模型行为，可用访问实例化函数预制件做平铺
        var tile = Instantiate(tilePrefab, tileParent);
        //集值函数
        tile.SetValue(GetRandomValue());
        tiles[spot.x, spot.y] = tile;//初始化
        return true;
    }
    //随机数
    private int GetRandomValue()
    {
        var rand = Random.Range(0f, 1f); // 0-1
        return rand < .8f ? 2 : 4; // 80%为2
    }
    //摆放
    private void UpdateTilePositions(bool instant)//告诉平铺设置动画到新位置的函数
    {
        //不是一个即时的位置更新：判断
        if (!instant)
        {
            isPlayingAnim = true;
            //需要一定时间完成动画 解决：不使用太多关于细节处理->例程功能
            StartCoroutine(WaitForTileAnimation()); //重载接受一个返回枚举数的函数：等待平铺动画
        }
        for (var x = 0; x < GridSize; x++)
            for (var y = 0; y < GridSize; y++)
                //阵列中循环是否有格子
                if (tiles[x, y] != null)
                    //生成平铺后调用
                    tiles[x, y].SetPosition(tilePositions[x, y].position, instant);

        if (instant)
        {
            curGameType.AddGameState(new Game2048StateModel { tailValues = GetCurrentTileValues() });
            RefreshBackBtnState();
        }
    }

    private IEnumerator WaitForTileAnimation()//返回一个等待数秒的函数
    {
        yield return new WaitForSeconds(tileSettings.AnimationTime);//获取动画时间
        //等待结束后添加一些生成逻辑
        if(!TrySpawnTile())//代表是否成功
        {
            //理论上永远不会发生，只有在成功移动后才能生成一个互动程序：没有剩余空格
            Debug.LogError("移动失败"); 
        }
        //新格子没有初始化位置：更新平铺位置
        UpdateTilePositions(true);
        //检查是否还有路->结束页面
        if (!AnyMovesLeft())
        {
            game2048ResultPanel.Show();
        }
        isPlayingAnim = false;//正在设置动画 = false
    }

    private bool AnyMovesLeft()//任何剩余移动
    {
        return CanMoveLeft() || CanMoveUp() || CanMoveRight() || CanMoveDown();
    }

    private bool tilesUpdated;//若没有在任何函数中运行则意味着没有可用的移动

    //移动
    private void TryMove(int x, int y)
    {
        if (x == 0 && y == 0)
            return;

        //通过获取输入的绝对值确保输入一个方向
        if(Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
        {
            return;
        }

        tilesUpdated = false;//初始化

        switch (x)
        {
            case 0 when y > 0:
                TryMoveUp();
                break;
            case 0:
                TryMoveDown();
                break;
            case < 0:
                TryMoveLeft();
                break;
            default:
                TryMoveRight();
                break;
        }

        AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Stab);
        //只有实际进行平铺更新时才会调用更新平铺位置函数。若不调用则没有动画可以等待
        if(tilesUpdated)//成功移动后调用位置更新函数
        {
            //更新将游戏状态推送到堆栈：创建游戏设置分数字段在当前储存,指定移动增加计数
            //使用用户首选类储存游戏最佳分数->简单的事务：存储角色串和数字并检索
            UpdateTilePositions(false);//平衡处理
            RefreshBackBtnState();
            VibrateHelper.VibrateHeavy();
        }

    }

    private int[,] GetCurrentTileValues()
    {
        //数组记录操作记录
        var result = new int[GridSize, GridSize];
        for (var x = 0; x < GridSize; x++)
            for (var y = 0; y < GridSize; y++)
                if (tiles[x, y] != null)
                    result[x, y] = tiles[x, y].GetValue();//必须公开来获取值->Tile.cs
        return result;//现已将游戏状态推到堆栈上->恢复函数
    }

    // //加载最后游戏状态：将游戏状态推到堆栈上,将状态从堆栈中移除，恢复游戏
    public void LoadLastStates()
    {
        //检查措施-用户可以随时按下撤销按钮
        //确保未在移动中
        if (isPlayingAnim)
            return;
        //堆栈无任何内容
        if (curGameType.GetGameStates().Count <= 0)
            return;
        //获取上一个游戏状态
        curGameType.PopGameState<Game2048StateModel>();
        var previousGameState = curGameType.PopGameState<Game2048StateModel>();

        //清除当前场景在所有磁贴中的循环
        foreach (var t in tiles)
        {
            if (t == null) continue;
            DestroyImmediate(t.gameObject);
        }
            
        //销毁后则循环
        for(var x = 0; x < GridSize; x ++)
            for(var y = 0; y < GridSize; y++)
            {
                tiles[x, y] = null;//先放置磁贴观察情况
                //检查前一个状态是否有值
                if(previousGameState.tailValues[x, y] == 0)
                    continue;//略过
    
                //无值则创建
                var tile = Instantiate(tilePrefab, tileParent);
                tile.SetValue(previousGameState.tailValues[x, y]);//设置为当前状态
                tiles[x, y] = tile;//确保放在正确位置
            }
        //调用更新执行一系列操作
        UpdateTilePositions(true);
    }

    private bool TileExistsBetween(int x, int y, int x2, int y2)
    {
        //水平｜垂直
        if (x == x2)
            return TileExistsBetweenVertical(x, y, y2);//纵向检查
        else if (y == y2)
            //横向检查
            return TileExistsBetweenHorizontal(x, x2, y);
        //错误-备用措施
        Debug.LogError($"BETWEEN CHECK - INVALID PARAMETERS ({x},{y}) ({x2},{y2})");
        return true;
    }

    private bool TileExistsBetweenHorizontal(int x, int x2, int y)//水平
    {
        var minX = Mathf.Min(x, x2);
        var maxX = Mathf.Max(x, x2);
        //从最小值右侧的一个平铺开始
        for (int xIndex = minX + 1; xIndex < maxX; xIndex++)
            if (tiles[xIndex, y] != null)//一块磁贴挡住去路，原路返回
                return true;
        return false;
    }

    private bool TileExistsBetweenVertical(int x, int y, int y2)//垂直
    {
        var minY = Mathf.Min(y, y2);
        var maxY = Mathf.Max(y, y2);
        //从最小值右侧的一个平铺开始
        for (int yIndex = minY + 1; yIndex < maxY; yIndex++)
            if (tiles[x, yIndex] != null)//一块磁贴挡住去路，原路返回
                return true;
        return false;
    }

    private void TryMoveRight()
    {
        for (var y = 0; y < GridSize; y++)//从顶层开始
            for (var x = GridSize - 1; x >= 0; x--)//每一排从右向左
            {
                if (tiles[x, y] == null) continue;//检查：空则继续

                for (var x2 = GridSize - 1; x2 > x; x2--)//有：从这一行最右侧寻找一个空白点继续
                {
                    //修复bug：检查确保两者之间没有瓷砖

                    if (tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))//如果有任何磁贴
                            continue;//防止合并
                        //将磁贴作为进一步的右侧：将x添加到y，并告诉它尝试与尾部合并->可能遇到不同的值无法合并
                        if(tiles[x2, y].Merge(tiles[x, y]))
                        {
                            tiles[x, y] = null;//将它从平铺管理器中删除但仍保留在场景中
                            tilesUpdated = true;//有效的移动
                            //打破x2循环，由于已经移动了x-y
                            break;
                        }
                        continue;//未能合并则继续：磁贴将尝试在左侧找到一个空白点
                    }

                    tilesUpdated = true;
                    tiles[x2, y] = tiles[x, y];
                    tiles[x, y] = null;
                    break;//成功移动，推出函数
                }
            }
    }

    private void TryMoveLeft()
    {
        for (var y = 0; y < GridSize; y++)//从顶层开始
            for (var x = 0; x < GridSize; x++)//每一排从左向右
            {
                if (tiles[x, y] == null) continue;//检查：空则继续
                for (int x2 = 0; x2 < x; x2++)//有：从这一行最左侧寻找一个空白点继续
                {
                    if (tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))//如果有任何磁贴
                            continue;//防止合并
                        //将磁贴作为进一步的右侧：将x添加到y，并告诉它尝试与尾部合并->可能遇到不同的值无法合并
                        if (tiles[x2, y].Merge(tiles[x, y]))
                        {
                            tiles[x, y] = null;//将它从平铺管理器中删除但仍保留在场景中
                            tilesUpdated = true;//有效的移动
                            //打破x2循环，由于已经移动了x-y
                            break;
                        }
                        continue;//未能合并则继续：磁贴将尝试在左侧找到一个空白点
                    }

                    tilesUpdated = true;
                    tiles[x2, y] = tiles[x, y];
                    tiles[x, y] = null;
                    break;//成功移动，推出函数
                }
            }
    }

    private void TryMoveDown()
    {
        for (var x = 0; x < GridSize; x++)//从第一列开始
            for (var y = GridSize - 1; y >= 0; y--)//每一层从下向上
            {
                if (tiles[x, y] == null) continue;//检查：空则继续
                for (var y2 = GridSize - 1; y2 > y; y2--)//有：从这一列最下方寻找一个空白点继续
                {
                    if (tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))//如果有任何磁贴
                            continue;//防止合并
                        //将磁贴作为进一步的右侧：将x添加到y，并告诉它尝试与尾部合并->可能遇到不同的值无法合并
                        if (tiles[x, y2].Merge(tiles[x, y]))
                        {
                            tiles[x, y] = null;//将它从平铺管理器中删除但仍保留在场景中
                            tilesUpdated = true;//有效的移动
                            //打破x2循环，由于已经移动了x-y
                            break;
                        }
                        continue;//未能合并则继续：磁贴将尝试在左侧找到一个空白点
                    }

                    tilesUpdated = true;
                    tiles[x, y2] = tiles[x, y];
                    tiles[x, y] = null;
                    break;//成功移动，推出函数
                }
            }
    }

    private void TryMoveUp()
    {
        for (var x = 0; x < GridSize; x++)//从第一列开始
            for (var y = 0; y < GridSize; y++)//每一层从上向下
            {
                if (tiles[x, y] == null) continue;//检查：空则继续
                for (int y2 = 0; y2 < y; y2++)//有：从这一列最上方寻找一个空白点继续
                {
                    if (tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))//如果有任何磁贴
                            continue;//防止合并
                        //将磁贴作为进一步的右侧：将x添加到y，并告诉它尝试与尾部合并->可能遇到不同的值无法合并
                        if (tiles[x, y2].Merge(tiles[x, y]))
                        {
                            tiles[x, y] = null;//将它从平铺管理器中删除但仍保留在场景中
                            tilesUpdated = true;//有效的移动
                            //打破x2循环，由于已经移动了x-y
                            break;
                        }
                        continue;//未能合并则继续：磁贴将尝试在左侧找到一个空白点
                    }

                    tilesUpdated = true;
                    tiles[x, y2] = tiles[x, y];
                    tiles[x, y] = null;
                    break;//成功移动，推出函数
                }
            }
    }
    
    private bool CanMoveRight()
    {
        for (var y = 0; y < GridSize; y++)//从顶层开始
            for (var x = GridSize - 1; x >= 0; x--)//每一排从右向左
            {
                if (tiles[x, y] == null) continue;//检查：空则继续

                for (var x2 = GridSize - 1; x2 > x; x2--)//有：从这一行最右侧寻找一个空白点继续
                {
                    //修复bug：检查确保两者之间没有瓷砖

                    if (tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))//如果有任何磁贴
                            continue;//防止合并
                        //将磁贴作为进一步的右侧：将x添加到y，并告诉它尝试与尾部合并->可能遇到不同的值无法合并
                        if (tiles[x2, y].CanMerge(tiles[x, y]))
                        {
                            return true;//至少有一个移动
                        }
                        continue;//未能合并则继续：磁贴将尝试在左侧找到一个空白点
                    }

                    return true;
                }
            }
        return false;//无法移动
    }

    private bool CanMoveLeft()
    {
        for (var y = 0; y < GridSize; y++)//从顶层开始
            for (var x = 0; x < GridSize; x++)//每一排从左向右
            {
                if (tiles[x, y] == null) continue;//检查：空则继续
                for (var x2 = 0; x2 < x; x2++)//有：从这一行最左侧寻找一个空白点继续
                {
                    if (tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))//如果有任何磁贴
                            continue;//防止合并
                        //将磁贴作为进一步的右侧：将x添加到y，并告诉它尝试与尾部合并->可能遇到不同的值无法合并
                        if (tiles[x2, y].CanMerge(tiles[x, y]))
                        {
                            return true;
                        }
                        continue;//未能合并则继续：磁贴将尝试在左侧找到一个空白点
                    }

                    return true;
                }
            }
        return false;
    }

    private bool CanMoveDown()
    {
        for (var x = 0; x < GridSize; x++)//从第一列开始
            for (var y = GridSize - 1; y >= 0; y--)//每一层从下向上
            {
                if (tiles[x, y] == null) continue;//检查：空则继续
                for (var y2 = GridSize - 1; y2 > y; y2--)//有：从这一列最下方寻找一个空白点继续
                {
                    if (tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))//如果有任何磁贴
                            continue;//防止合并
                        //将磁贴作为进一步的右侧：将x添加到y，并告诉它尝试与尾部合并->可能遇到不同的值无法合并
                        if (tiles[x, y2].CanMerge(tiles[x, y]))
                        {
                            return true;
                        }
                        continue;//未能合并则继续：磁贴将尝试在左侧找到一个空白点
                    }

                    return true;
                }
            }
        return false;
    }

    private bool CanMoveUp()
    {
        for (var x = 0; x < GridSize; x++)//从第一列开始
            for (var y = 0; y < GridSize; y++)//每一层从上向下
            {
                if (tiles[x, y] == null) continue;//检查：空则继续
                for (var y2 = 0; y2 < y; y2++)//有：从这一列最上方寻找一个空白点继续
                {
                    if (tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))//如果有任何磁贴
                            continue;//防止合并
                        //将磁贴作为进一步的右侧：将x添加到y，并告诉它尝试与尾部合并->可能遇到不同的值无法合并
                        if (tiles[x, y2].CanMerge(tiles[x, y]))
                        {
                            return true;
                        }
                        continue;//未能合并则继续：磁贴将尝试在左侧找到一个空白点
                    }

                    return true;
                }
            }
        return false;
    }

    private void ClearGame()
    {
        foreach (var t in tiles)
        {
            if (t != null)
            {
                DestroyImmediate(t.gameObject);
            }   
        }
        
        curGameType.ClearGameState();
    }
}