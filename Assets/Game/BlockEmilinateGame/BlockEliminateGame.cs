 using System;
using System.Collections.Generic;
using System.Linq;
using NMNH.Utility;
using PuzzleGame.Gameplay;
using PuzzleGame.Gameplay.Puzzle1010;
using PuzzleGame.Themes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BlockEliminateGame
{
    public class BlockEliminateGame : BaseGameController<PutBlockGameState>
    {
        [Header("PutBlockGame模式字段")] 
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GraphicRaycaster figureGraphicRaycaster;
        [SerializeField] private Transform allFigureParent;
        [SerializeField] private Transform emptyFieldParent;
        [SerializeField] private BlockEliminateGameOverPanel blockEliminateGameOverPanel;
        [SerializeField] private BlockEliminateGameRevivePanel blockEliminateGameRevivePanel;
        [SerializeField] private Brick emptyBrickPrefab;
        [SerializeField] private FigureController[] figureControllers;
        
        private Brick[,] backgroundBricks;
        private int[] figures = Array.Empty<int>();
        private float[] figureRotations = Array.Empty<float>();
        private readonly BricksHighlighter bricksHighlighter = new();

        private static int GetRandomColorIndex() => Random.Range(1, 6);

        public BlockEliminateModel GameDataModel { get; set; } = new BlockEliminateModel();

        private void Awake()
        {
            closeUseBoosterHintBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                useBoosterHintObj.SetActive(false);
                allFigureParent.gameObject.SetActive(true);
                curBoosterType = BoosterType.None;
            });

            foreach (var item in horizontalBtn)
            {
                item.onClick.AddListener(() =>
                {
                    BaseUtilities.PlayCommonClick();
                    if(curBoosterType == BoosterType.None) return;
                    
                    RemoveLine(curBoosterType == BoosterType.RemoveHorizontal, item.transform.GetSiblingIndex());
                    BlockEliminateStageManager.Instance.ConsumeBooster(curBoosterType);
                    curBoosterType = BoosterType.None;
                    useBoosterHintObj.SetActive(false);
                    allFigureParent.gameObject.SetActive(true);
                });
            }
            
            foreach (var item in verticalBtn)
            {
                item.onClick.AddListener(() =>
                {
                    BaseUtilities.PlayCommonClick();
                    if(curBoosterType == BoosterType.None) return;
                    
                    RemoveLine(curBoosterType == BoosterType.RemoveHorizontal, item.transform.GetSiblingIndex());
                    BlockEliminateStageManager.Instance.ConsumeBooster(curBoosterType);
                    curBoosterType = BoosterType.None;
                    useBoosterHintObj.SetActive(false);
                    allFigureParent.gameObject.SetActive(true);
                });
            }
            
            field = new NumberedBrick[bricksCount.x, bricksCount.y];
            backgroundBricks = new Brick[bricksCount.x, bricksCount.y];

            for (var x = 0; x < bricksCount.x; x++)
            {
                for (var y = 0; y < bricksCount.y; y++)
                {
                    SpawnEmptyBrick(new Vector2Int(x, y));
                }
            }

            foreach (var figureController in figureControllers)
            {
                figureController.PointerUp += FigureOnPointerUp;
                figureController.PointerClick += OnHighlightedTargetClick;
                figureController.PointerDrag += FigureOnPointerDrag;
            }
        }

        public void StartNewGame(StageConfigSO stageConfig)
        {
            ComboManager.Instance.ResetComboState(GameType.PutBlockGame);
            
            // 清除已经生成的内容
            foreach (var item in field)
            {
                if (item != null)
                {
                    item.Number = 0;
                    Destroy(item.gameObject);
                }
            }
            foreach (var item in figureControllers)
            {
                foreach (var brick in item.bricks)
                {
                    Destroy(brick.gameObject);
                }
                
                item.bricks.Clear();
                item.ResetPosition();
                item.Interactable = true;
            }

            allFigureParent.gameObject.SetActive(true);
            
            GameDataModel.RevivedCount = 0;
            GameDataModel.Score = 0;
            collectBarrierRecord.Clear();
            // 生成新的放置内容
            SpawnNewFigures();
            // SpawnStartingBricks();
        }

        #region 使用道具相关

        [SerializeField] private Button closeUseBoosterHintBtn;
        [SerializeField] private GameObject useBoosterHintObj;
        [SerializeField] private TextMeshProUGUI useBoosterHintTmp;
        [SerializeField] private GameObject horizontalBtnsObj;
        [SerializeField] private GameObject verticalBtnsObj;
        [SerializeField] private List<Button> horizontalBtn;
        [SerializeField] private List<Button> verticalBtn;

        private BoosterType curBoosterType;

        public void ShowUseBoosterHint(BoosterType type)
        {
            switch (type)
            {
                case BoosterType.RemoveHorizontal:
                    useBoosterHintTmp.text = "选择一行方块，将它们全部消除";
                    curBoosterType = type;
                    break;
                case BoosterType.RemoveVertical:
                    useBoosterHintTmp.text = "选择一列方块，将它们全部消除";
                    curBoosterType = type;
                    break;
            }
        }
        
        public bool UseBooster(BoosterType type)
        {
            switch (type)
            {
                case BoosterType.Refresh:
                    RefreshAllFigure();
                    curBoosterType = BoosterType.None;
                    break;
                case BoosterType.RemoveHorizontal:
                    horizontalBtnsObj.SetActive(true);
                    verticalBtnsObj.SetActive(false);
                    useBoosterHintObj.SetActive(true);
                    curBoosterType = type;
                    allFigureParent.gameObject.SetActive(false);
                    break;
                case BoosterType.RemoveVertical:
                    horizontalBtnsObj.SetActive(false);
                    verticalBtnsObj.SetActive(true);
                    useBoosterHintObj.SetActive(true);
                    curBoosterType = type;
                    allFigureParent.gameObject.SetActive(false);
                    break;
            }
            
            return true;
        }
        
        private void RemoveLine(bool isHorizontal, int lineNum)
        {
            var bricksToDestroy = new List<Vector2Int>();
            if (isHorizontal)
            {
                for (var i = 0; i < bricksCount.x; i++)
                {
                    bricksToDestroy.Add(new Vector2Int(i, lineNum));
                }
            }
            else
            {
                for (var i = 0; i < bricksCount.y; i++)
                {
                    bricksToDestroy.Add(new Vector2Int(lineNum, i));
                }
            }
            
            if (bricksToDestroy.Count > 0)
            {
                AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Win);
            }

            if (bricksToDestroy.Count == 10)
            {
                VibrateHelper.VibrateMedium();
            }
            else if(bricksToDestroy.Count > 10)
            {
                VibrateHelper.VibrateHeavy();
            }
            
            var addScore = ComboManager.Instance.AddPlayerOperate(GameType.PutBlockGame, completeLineCount, Vector3.zero);
            GameDataModel.Score += addScore;

            foreach (var c in bricksToDestroy)
            {
                var brick = field[c.x, c.y];
                if(brick == null) continue;
                if (brick.Barrier != null && brick.Barrier.BarrierType != GameModel.BarrierType.None)
                {
                    if (brick.Barrier.BeAttack())
                    {
                        brick.DoMergingAnimation(() =>
                        {
                            bricksHighlighter.Unhighlight(brick);
                            Destroy(brick.gameObject);
                        });

                        field[c.x, c.y] = null;
                        CollectBarrier(brick.Barrier.BarrierType, brick.transform.position);
                    }
                    else
                    {
                        brick.Highlight(false);
                    }
                }
                else
                {
                    brick.DoMergingAnimation(() =>
                    {
                        bricksHighlighter.Unhighlight(brick);
                        Destroy(brick.gameObject);
                    });
                    
                    field[c.x, c.y] = null;
                }
                
                GameDataModel.Score++; 
            }
        }
        
        #endregion

        #region 目标相关

        private Dictionary<GameModel.BarrierType, int> collectBarrierRecord = new ();

        public Action<GameModel.BarrierType, Vector3> OnCollectBarrier;

        private List<GameModel.BarrierType> tempBarrierTypes = new ();

        public int GetCollectBarrierCount(GameModel.BarrierType barrierType)
        {
            return collectBarrierRecord.GetValueOrDefault(barrierType, 0);
        }

        private void GenBarrier(FigureController figureController)
        {
            // 根据概率判断是否需要生成障碍物
            var needGenBarrier = Random.Range(0f, 1f) > 0.6f;
            
            if (needGenBarrier)
            {
                tempBarrierTypes.Clear();
                
                foreach (var itemModel in BlockEliminateStageManager.Instance.CurStageConfig.Targets)
                {
                    if (collectBarrierRecord.TryGetValue(itemModel.Key, out var value))
                    {
                        if (value < itemModel.Value.TargetCount)
                        {
                            tempBarrierTypes.Add(itemModel.Key);
                        }
                    }
                    else
                    {
                        tempBarrierTypes.Add(itemModel.Key);
                    }
                }
                
                tempBarrierTypes.AddRange(BlockEliminateStageManager.Instance.CurStageConfig.extraBarriers);

                if (tempBarrierTypes.Count > 0)
                {
                    var barrierType = tempBarrierTypes[Random.Range(0, tempBarrierTypes.Count)];
                    var genIndex = Random.Range(0, figureController.bricks.Count);
                    var tempTrans = Instantiate(ResourcesCenter.Instance.GetBarrierByType(barrierType), figureController.bricks[genIndex].transform, true).transform;
                    tempTrans.localPosition = Vector3.zero;
                    tempTrans.localScale = Vector3.one;
                    figureController.bricks[genIndex].SetBarrier(tempTrans.GetComponent<Barrier>());
                }
            }
        }

        private void CollectBarrier(GameModel.BarrierType barrierType, Vector3 genPos)
        {
            if(BlockEliminateStageManager.Instance.CurStageConfig.Targets is not { Count: > 0 }) return;

            if (BlockEliminateStageManager.Instance.CurStageConfig.Targets.ContainsKey(barrierType))
            {
                if (!collectBarrierRecord.TryAdd(barrierType, 1))
                {
                    if (collectBarrierRecord[barrierType] < BlockEliminateStageManager.Instance.CurStageConfig.Targets[barrierType].TargetCount)
                    {
                        collectBarrierRecord[barrierType]++;
                        OnCollectBarrier?.Invoke(barrierType, genPos);
                    }
                }
                else
                {
                    OnCollectBarrier?.Invoke(barrierType, genPos);
                }
            }
        }

        #endregion

        #region 生成方块相关内容

        private void SpawnBrick(Vector2Int coords, int number)
        {
            var brick = Instantiate(brickPrefab, fieldTransform);

            brick.transform.SetParent(fieldTransform, false);
            brick.RectTransform.anchorMin = Vector2.zero;
            brick.RectTransform.anchorMax = Vector2.zero;
            brick.RectTransform.anchoredPosition = GetBrickPosition(coords);

            SetBrickColor(brick, number);
            brick.PointerClick += OnHighlightedTargetClick;

            field[coords.x, coords.y] = brick;
        }

        private static void SetBrickColor(NumberedBrick brick, int number)
        {
            brick.ColorIndex = number - 1;
        }

        protected virtual Brick SpawnEmptyBrick(Vector2Int coords)
        {
            var brick = Instantiate(emptyBrickPrefab, emptyFieldParent);
            brick.transform.SetParent(emptyFieldParent, false);
            brick.RectTransform.anchorMin = Vector2.zero;
            brick.RectTransform.anchorMax = Vector2.zero;
            brick.RectTransform.anchoredPosition = GetBrickPosition(new Vector2(coords.x, coords.y));
            brick.PointerClick += OnHighlightedTargetClick;
            backgroundBricks[coords.x, coords.y] = brick;
            return brick;
        }

        private void SpawnNewFigures()
        {
            figures = new int[figureControllers.Length];
            figureRotations = new float[figureControllers.Length];

            // 计算第几个位置应用算法
            var algorithmIndex = Random.Range(0, figureRotations.Length);
            
            for (var i = 0; i < figureControllers.Length; i++)
            {
                if (algorithmIndex == i)
                {
                    // 应用算法
                    if (SpawnFigureByAlgorithm(figureControllers[i]))
                    {
                        continue;
                    }
                }
                
                var randomNum = Random.Range(0, 107);
                for (var j = 0; j < PutBlockConfig.FiguresProbability.Length; j++)
                {
                    if (randomNum <= PutBlockConfig.FiguresProbability[j])
                    {
                        var figure = j;
                        var rotation = Random.Range(0, 4) * 90f;

                        SpawnFigure(figureControllers[i], figure, rotation, GetRandomColorIndex());

                        figures[i] = figure;
                        figureRotations[i] = rotation;
                        break;
                    }

                    randomNum -= PutBlockConfig.FiguresProbability[j];
                }
            }
        }

        private bool SpawnFigureByAlgorithm(FigureController figureController)
        {
            // 算法：
            // 第一步：计算只有一个断隔的横排和竖排的元素
            // 第二步：在两个横排和竖排的元素中选择一个
            // 第三步：计算断隔的长度，根据长度去生成对应的元素
            
            // 计算横排只有一个断隔的
            var tempXs = new List<int>();
            for (var i = 0; i < field.GetLength(0); i++)
            {
                var lastNullIndex = -1;
                var isConform = true;
                for (var j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == null)
                    {
                        if (lastNullIndex == -1 || j - lastNullIndex <= 1)
                        {
                            lastNullIndex = j;
                        }
                        else
                        {
                            isConform = false;
                            break;
                        }
                    }
                }

                if (isConform)
                {
                    tempXs.Add(i);
                }
            }
            
            // 计算竖排只有一个断隔的
            var tempYs = new List<int>();
            for (var i = 0; i < field.GetLength(0); i++)
            {
                var lastNullIndex = -1;
                var isConform = true;
                for (var j = 0; j < field.GetLength(1); j++)
                {
                    if (field[j, i] == null)
                    {
                        if (lastNullIndex == -1 || j - lastNullIndex <= 1)
                        {
                            lastNullIndex = j;
                        }
                        else
                        {
                            isConform = false;
                            break;
                        }
                    }
                }

                if (isConform)
                {
                    tempYs.Add(i);
                }
            }
            
            // 得到其中一个
            if (Random.Range(0, 1f) > 0.5f)
            {
                if (tempXs.Count <= 0)
                {
                    return false;
                }
                else
                {
                    var tempX = tempXs[Random.Range(0, tempXs.Count)];
                    var nullStartIndex = -1;
                    var nullEndIndex = 8;
                    for (var i = 0; i < field.GetLength(1); i++)
                    {
                        if (field[tempX, i] == null && nullStartIndex == -1)
                        {
                            nullStartIndex = i;
                        }

                        if (nullStartIndex != -1 && field[tempX, i] != null)
                        {
                            nullEndIndex = i;
                            break;
                        }
                    }

                    var nullCount = nullEndIndex - nullStartIndex;
                    switch (nullCount)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            SpawnFigure(figureController, nullCount - 1, 90, GetRandomColorIndex());
                            return true;
                        default:
                            return false;
                    }
                }
            }
            else
            {
                if (tempYs.Count <= 0)
                {
                    return false;
                }
                else
                {
                    var tempY = tempYs[Random.Range(0, tempYs.Count)];
                    var nullStartIndex = -1;
                    var nullEndIndex = 8;
                    for (var i = 0; i < field.GetLength(0); i++)
                    {
                        if (field[i, tempY] == null && nullStartIndex == -1)
                        {
                            nullStartIndex = i;
                        }

                        if (nullStartIndex != -1 && field[i, tempY] != null)
                        {
                            nullEndIndex = i;
                            break;
                        }
                    }

                    var nullCount = nullEndIndex - nullStartIndex;
                    switch (nullCount)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            SpawnFigure(figureController, nullCount - 1, 0, GetRandomColorIndex());
                            return true;
                        default:
                            return false;
                    }
                }
            }
        }

        private void SpawnFigure(FigureController figureController, int figureIndex, float rotation, int brickNumber)
        {
            figureController.transform.localRotation = Quaternion.identity;

            var figure = PutBlockConfig.Figures[figureIndex];
            
            for (var i = 0; i < figure.GetLength(0); i++)
            {
                for (var j = 0; j < figure.GetLength(1); j++)
                {
                    if (figure[figure.GetLength(0) - i - 1, j] == 0)
                        continue;

                    var brick = Instantiate(brickPrefab, figureController.transform);
                    figureController.bricks.Add(brick);

                    var brickRectTransform = brick.RectTransform;

                    brickRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    brickRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                    var rect = figureController.GetComponent<RectTransform>().rect;
                    var brickSize = new Vector2
                    {
                        x = rect.width / 4,
                        y = rect.height / 4
                    };

                    var coords = new Vector2(j - figure.GetLength(1) / 2f, i - figure.GetLength(0) / 2f);
                    var brickPosition = Vector2.Scale(coords, brickSize);
                    brickPosition += Vector2.Scale(brickSize, brickRectTransform.pivot);
                    brick.RectTransform.anchoredPosition = brickPosition;

                    SetBrickColor(brick, brickNumber);
                }
            }

            if (BlockEliminateStageManager.Instance.CurStageConfig != null &&
                BlockEliminateStageManager.Instance.CurStageConfig.TargetType == GameModel.TargetType.Barrier)
            {
                GenBarrier(figureController);
            }

            figureController.Rotate(rotation);
        }

        #endregion

        private int completeLineCount;

        protected virtual Vector2Int[] GetCompleteLines()
        {
            ResetEffectState();
            completeLineCount = 0;
            var linesBricks = new List<Vector2Int>();

            for (var x = 0; x < bricksCount.x; x++)
            {
                var line = true;

                for (var y = 0; y < bricksCount.y; y++)
                {
                    if (field[x, y] != null)
                        continue;

                    line = false;
                    break;
                }

                if (!line)
                {
                    completeX[x] = 0;
                    continue;
                }
                    
                completeLineCount++;
                for (var y = 0; y < bricksCount.y; y++)
                    linesBricks.Add(new Vector2Int(x, y));
                completeX[x] = 1;
            }

            for (var y = 0; y < bricksCount.y; y++)
            {
                var line = true;

                for (var x = 0; x < bricksCount.x; x++)
                {
                    if (field[x, y] != null)
                        continue;

                    line = false;
                    break;
                }

                if (!line)
                {
                    completeY[y] = 0;
                    continue;
                }
                    
                completeLineCount++;
                for (var x = 0; x < bricksCount.x; x++)
                    linesBricks.Add(new Vector2Int(x, y));
                completeY[y] = 1;
            }

            return linesBricks.Distinct().ToArray();
        }

        #region 游戏流程复活结束相关
        
        private void CheckGameOver()
        {
            // 判断是否完成乐所有目标
            switch (BlockEliminateStageManager.Instance.CurStageConfig.TargetType)
            {
                case GameModel.TargetType.Score:
                    if (GameDataModel.Score >= BlockEliminateStageManager.Instance.CurStageConfig
                            .Targets[GameModel.BarrierType.Score].TargetCount)
                    {
                        OnGameOver(true);
                    }
                    break;
                case GameModel.TargetType.Barrier:
                    if (BlockEliminateStageManager.Instance.CurStageConfig.Targets is { Count: > 0 })
                    {
                        var completeAll = true;
                        foreach (var itemModel in BlockEliminateStageManager.Instance.CurStageConfig.Targets)
                        {
                            if (collectBarrierRecord.TryGetValue(itemModel.Key, out var value))
                            {
                                if (value < itemModel.Value.TargetCount)
                                {
                                    completeAll = false;
                                    break;
                                }
                            }
                            else
                            {
                                completeAll = false;
                                break;
                            }
                        }

                        if (completeAll)
                        {
                            OnGameOver(true);
                        }
                    }
                    break;
            }
            
            if (figureControllers.Any(figure => figure.bricks.Count > 0 && IsCanPlaceFigure(figure)))
                return;

            // 判断是否复活过
            blockEliminateGameRevivePanel.Init(false, OnRevive).Show();
            // if (GameDataModel.RevivedCount > 0)
            // {
            //     OnGameOver(false);
            // }
            // else
            // {
            //     
            // }
        }


        /// <summary>
        /// 游戏结束
        /// </summary>
        private void OnGameOver(bool win)
        {
            if (win)
            {
                blockEliminateGameOverPanel.Show();
                BlockEliminateStageManager.Instance.StagePass();
            }
            else
            {
                blockEliminateGameRevivePanel.Init(true, null).Show();
            }
        }

        /// <summary>
        /// 游戏复活
        /// </summary>
        private void OnRevive()
        {
            GameDataModel.RevivedCount++;
            // 生成小单位的格子
            foreach (var item in figureControllers)
            {
                foreach (var brick in item.bricks)
                {
                    Destroy(brick.gameObject);
                }
                
                item.bricks.Clear();
                item.ResetPosition();
                item.Interactable = true;
            }
            
            figures = new int[figureControllers.Length];
            figureRotations = new float[figureControllers.Length];
            for (var i = 0; i < figureControllers.Length; i++)
            {
                var figureIndex = i == 0 ? 0 : (Random.Range(0, 1) > 0.5f ? 0 : 1);
                SpawnFigure(figureControllers[i], figureIndex, 0, GetRandomColorIndex());
                figures[i] = figureIndex;
                figureRotations[i] = 0;
            }
        }        

        #endregion

        #region 判断相关内容
        
        private void CheckLines()
        {
            var bricksToDestroy = GetCompleteLines();
            
            if (bricksToDestroy.Length > 0)
            {
                AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Win);
            }

            if (bricksToDestroy.Length == 10)
            {
                VibrateHelper.VibrateMedium();
            }
            else if(bricksToDestroy.Length > 10)
            {
                VibrateHelper.VibrateHeavy();
            }
            
            var addScore = ComboManager.Instance.AddPlayerOperate(GameType.PutBlockGame, completeLineCount, Vector3.zero);
            GameDataModel.Score += addScore;

            foreach (var c in bricksToDestroy)
            {
                var brick = field[c.x, c.y];

                if (brick.Barrier != null && brick.Barrier.BarrierType != GameModel.BarrierType.None)
                {
                    if (brick.Barrier.BeAttack())
                    {
                        brick.DoMergingAnimation(() =>
                        {
                            bricksHighlighter.Unhighlight(brick);
                            Destroy(brick.gameObject);
                        });

                        field[c.x, c.y] = null;
                        CollectBarrier(brick.Barrier.BarrierType, brick.transform.position);
                    }
                    else
                    {
                        brick.Highlight(false);
                    }
                }
                else
                {
                    brick.DoMergingAnimation(() =>
                    {
                        bricksHighlighter.Unhighlight(brick);
                        Destroy(brick.gameObject);
                    });
                    
                    field[c.x, c.y] = null;
                }
                
                GameDataModel.Score++; 
            }
        }
        private void CheckFigures()
        {
            foreach (var figureController in figureControllers)
            {
                if (figureController.bricks.Count == 0)
                    continue;

                var canPlaceFigure = IsCanPlaceFigure(figureController);
                figureController.Interactable = canPlaceFigure;
                foreach (var brick in figureController.bricks.Cast<NumberedBrick>())
                {
                    brick.SetOverrideColorType(canPlaceFigure ? null : ColorType.Inactive);
                }
            }
        }

        private bool IsCanPlaceFigure(FigureController figureController)
        {
            for (var x = 0; x < bricksCount.x; x++)
            {
                for (var y = 0; y < bricksCount.y; y++)
                {
                    if (IsCanPlaceFigure(x, y, figureController))
                        return true;
                }
            }

            return false;
        }

        private bool IsCanPlaceFigure(int x, int y, FigureController figureController)
        {
            var rotation = figureController.transform.localRotation;

            var minPosition = new Vector2(float.MaxValue, float.MaxValue);
            foreach (var brick in figureController.bricks)
            {
                Vector2 localPosition = rotation * brick.RectTransform.anchoredPosition;

                minPosition.x = Mathf.Min(minPosition.x, localPosition.x);
                minPosition.y = Mathf.Min(minPosition.y, localPosition.y);
            }

            foreach (var brick in figureController.bricks)
            {
                var rectTransform = brick.RectTransform;

                Vector2 position = rotation * rectTransform.anchoredPosition;
                position -= minPosition;

                var coords = Vector2Int.RoundToInt(position / rectTransform.rect.size);
                coords.x += x;
                coords.y += y;

                if (coords.x < 0 || coords.y < 0 || coords.x >= bricksCount.x || coords.y >= bricksCount.y ||
                    field[coords.x, coords.y] != null)
                    return false;
            }

            return true;
        }

        #endregion

        #region 手指操作相关内容
        
        private void FigureOnPointerUp(FigureController figureController)
        {
            bricksHighlighter.UnhighlightBricks();
            HideAllHighLightEffect();

            if (!TryGetCoords(figureController.bricks, out var coords))
            {
                bricksHighlighter.UnhighlightNumberedBricks();
                return;
            }

            for (var i = 0; i < figureController.bricks.Count; i++)
            {
                var brick = figureController.bricks[i];

                brick.transform.localRotation = Quaternion.identity;
                var rectTransform = brick.RectTransform;

                rectTransform.SetParent(fieldTransform, false);
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.anchoredPosition = GetBrickPosition(coords[i]);

                field[coords[i].x, coords[i].y] = brick as NumberedBrick;
                brick.PointerClick += OnHighlightedTargetClick;

                GameDataModel.Score++;
            }
            
            ShowDestroyEffect(figureController.bricks[0].ColorIndex);

            AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Stab);
            VibrateHelper.VibrateLight();

            var index = Array.IndexOf(figureControllers, figureController);
            figures[index] = -1;

            figureController.bricks.Clear();
            figureController.ResetPosition();

            CheckLines();

            if (figureControllers.All(c => c.bricks.Count == 0))
                SpawnNewFigures();

            CheckFigures();

            CheckGameOver();
        }

        private void FigureOnPointerDrag(FigureController figureController)
        {
            if (!TryGetCoords(figureController.bricks, out var coords))
            {
                bricksHighlighter.UnhighlightBricks();
                bricksHighlighter.UnhighlightNumberedBricks();
                HideAllHighLightEffect();
                return;
            }

            for (var i = 0; i < coords.Length; i++)
            {
                var c = coords[i];
                field[c.x, c.y] = figureController.bricks[i] as NumberedBrick;
            }

            var linesBricks = GetCompleteLines();

            var colorIndex = field[coords[0].x, coords[0].y].ColorIndex;
            bricksHighlighter.SetHighlight(linesBricks.Select(c => field[c.x, c.y]).ToArray(), colorIndex);
            bricksHighlighter.SetHighlight(coords.Select(c => backgroundBricks[c.x, c.y]).ToArray());
            
            ShowHighLightEffect(figureController.bricks[0].ColorIndex);

            foreach (var c in coords)
                field[c.x, c.y] = null;
        }

        protected override void OnFigureRemoved(FigureController figure)
        {
            var index = Array.IndexOf(figureControllers, figure);
            figures[index] = -1;

            if (figureControllers.All(c => c.bricks.Count == 0))
                SpawnNewFigures();
            
            CheckFigures();
            CheckGameOver();
        }

        #endregion

        #region 特效相关

        [SerializeField] public DestroyEffect[] destroyEffectsX;
        [SerializeField] public DestroyEffect[] destroyEffectsY;
        
        private int[] completeX = new int[8];
        private int[] completeY = new int[8];

        private void ResetEffectState()
        {
            for (var i = 0; i < completeX.Length; i++)
            {
                completeX[i] = 0;
            }
            
            for (var i = 0; i < completeY.Length; i++)
            {
                completeY[i] = 0;
            }
        }
        
        private void HideAllHighLightEffect()
        {
            foreach (var t in destroyEffectsX)
            {
                t.SetHighLightEffectVisible(false);
            }
            
            foreach (var t in destroyEffectsY)
            {
                t.SetHighLightEffectVisible(false);
            }
        }

        private void ShowHighLightEffect(int colorIndex)
        {
            for (var i = 0; i < completeX.Length; i++)
            {
                if (completeX[i] == 1)
                {
                    destroyEffectsY[i].SetHighLightEffectVisible(true, colorIndex);
                }
                else
                {
                    destroyEffectsY[i].SetHighLightEffectVisible(false);
                }
            }
            
            for (var i = 0; i < completeY.Length; i++)
            {
                if (completeY[i] == 1)
                {
                    destroyEffectsX[i].SetHighLightEffectVisible(true, colorIndex);
                }
                else
                {
                    destroyEffectsX[i].SetHighLightEffectVisible(false);
                }
            }
        }
        
        private void ShowDestroyEffect(int colorIndex)
        {
            for (var i = 0; i < completeX.Length; i++)
            {
                if (completeX[i] == 1)
                {
                    destroyEffectsY[i].ShowDestroyEffect(colorIndex);   
                }
            }
            
            for (var i = 0; i < completeY.Length; i++)
            {
                if (completeY[i] == 1)
                {
                    destroyEffectsX[i].ShowDestroyEffect(colorIndex);
                }
            }
        }
    
        #endregion

        #region Booster相关内容
        
        /// <summary>
        /// 刷新所有的放置内容
        /// </summary>
        public void RefreshAllFigure()
        {
            // 生成小单位的格子
            foreach (var item in figureControllers)
            {
                foreach (var brick in item.bricks)
                {
                    Destroy(brick.gameObject);
                }
                
                item.bricks.Clear();
                item.ResetPosition();
                item.Interactable = true;
            }
            
            figures = new int[figureControllers.Length];
            figureRotations = new float[figureControllers.Length];
            var tempIndex = Random.Range(0, figureControllers.Length);
            for (var i = 0; i < figureControllers.Length; i++)
            {
                var figureIndex = i == tempIndex ? 0 : Random.Range(1, 5);
                var tempRotation = Random.Range(0, 4) * 90;
                SpawnFigure(figureControllers[i], figureIndex, tempRotation, GetRandomColorIndex());
                figures[i] = figureIndex;
                figureRotations[i] = tempRotation;
            }
        }

        #endregion
        
        public override void SaveGame() { }
        
        protected override void StartGame() { }

        public void SetInputEnable(bool enable)
        {
            canvasGroup.blocksRaycasts = enable;
            figureGraphicRaycaster.enabled = enable;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                RemoveLine(true, 0);
            }
            
            if (Input.GetKey(KeyCode.B))
            {
                RemoveLine(false, 0);
            }
        }
    }

    public class BlockEliminateModel
    {
        public Action OnScoreChanged;

        private int score;
        
        public int Score
        {
            get => score;
            set
            {
                score = value;
                OnScoreChanged?.Invoke();
            }
        }

        public int RevivedCount { get; set; }
    }
}