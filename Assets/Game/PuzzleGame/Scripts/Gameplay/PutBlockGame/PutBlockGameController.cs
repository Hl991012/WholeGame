 using System;
using System.Collections.Generic;
using System.Linq;
using NMNH.Utility;
using PuzzleGame.Gameplay.Boosters;
using PuzzleGame.Themes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PuzzleGame.Gameplay.Puzzle1010
{
    public class PutBlockGameController : BaseGameController<PutBlockGameState>
    {
        [Header("PutBlockGame模式字段")]
        [SerializeField] private PutBlockGameOverPanel putBlockGameOverPanel;
        [SerializeField] private PutBlockGameRevivePanel putBlockGameRevivePanel;
        [SerializeField] private Brick emptyBrickPrefab;
        [SerializeField] private FigureController[] figureControllers;
        [SerializeField] private FigureController extraFigureController;

        private Brick[,] backgroundBricks;
        private int[] figures = Array.Empty<int>();
        private float[] figureRotations = Array.Empty<float>();
        private int extraFigureIndex;
        private float extraFigureRotation;
        private readonly BricksHighlighter bricksHighlighter = new();

        private const int MaxBrickNumber = 7;

        private static int GetRandomBrickNumber() => Random.Range(1, MaxBrickNumber);
        
        private void Awake()
        {
            field = new NumberedBrick[bricksCount.x, bricksCount.y];
            backgroundBricks = new Brick[bricksCount.x, bricksCount.y];

            for (var x = 0; x < bricksCount.x; x++)
            {
                for (var y = 0; y < bricksCount.y; y++)
                {
                    SpawnEmptyBrick(new Vector2Int(x, y));
                }
            }
        
            gameState = UserProgress.Current.GetGameState<PutBlockGameState>(name);
            if (gameState == null)
            {
                gameState = new PutBlockGameState();
                UserProgress.Current.SetGameState(name, gameState);
            }

            foreach (var figureController in figureControllers)
            {
                figureController.PointerUp += FigureOnPointerUp;
                figureController.PointerClick += OnHighlightedTargetClick;
                figureController.PointerDrag += FigureOnPointerDrag;
            }

            extraFigureController.PointerUp += FigureOnPointerUp;
            extraFigureController.PointerClick += OnHighlightedTargetClick;
            extraFigureController.PointerDrag += FigureOnPointerDrag;
        }
        
        private void Start()
        {
            StartGame();
            CheckGameOver();
        }

        protected override void StartGame()
        {
            if (LoadGame())
                return;

            gameState.Score = 0;
            gameState.IsGameOver = false;
            SpawnNewFigures(false);
            // SpawnStartingBricks();
            SetStartBoosters();
            SaveGame();
        }

        public override void ReplayGame()
        {
            ComboManager.Instance.ResetComboState(GameType.PutBlockGame);
            
            gameState.IsGameOver = false;
            gameState.HasRevive = false;
            
            // 清除已经生成的内容
            gameState.Score = 0;
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
                // item.Interactable = true;
            }

            foreach (var item in extraFigureController.bricks)
            {
                Destroy(item.gameObject);
            }
            extraFigureController.bricks.Clear();
            extraFigureController.ResetPosition();
            
            ClearGameState();
            StartGame();
        }

        // 创建游戏开始的随机目标
        // private void SpawnStartingBricks()
        // {
        //     var positions = new List<Vector2Int>();
        //     for (var i = 0; i < bricksCount.x; i++)
        //     {
        //         for (var j = 0; j < bricksCount.y; j++)
        //         {
        //             positions.Add(new Vector2Int(i, j));
        //         }
        //     }
        //
        //     for (int i = 1; i <= 9; i++)
        //     {
        //         int rand = Random.Range(0, positions.Count);
        //         SpawnBrick(positions[rand], GetRandomBrickNumber());
        //         positions.RemoveAt(rand);
        //     }
        // }

        private bool LoadGame()
        {
            if (gameState == null || gameState.IsGameOver)
                return false;

            var numbers = gameState.GetField();
            if (numbers == null || numbers.Length != bricksCount.x * bricksCount.y)
                return false;

            for (var x = 0; x < bricksCount.x; x++)
            {
                for (var y = 0; y < bricksCount.y; y++)
                {
                    if (numbers[x * bricksCount.y + y] > 0)
                        SpawnBrick(new Vector2Int(x, y), numbers[x * bricksCount.y + y]);
                }
            }

            figures = gameState.GetFigures();
            figureRotations = gameState.GetFigureRotations();
            var figureIndexes = gameState.GetFigureIndexes();

            if (figures.Length != figureControllers.Length || figureIndexes.Length != figureControllers.Length)
                return false;

            for (var i = 0; i < figureControllers.Length; i++)
            {
                if (figures[i] >= 0)
                    SpawnFigure(figureControllers[i], figures[i], figureRotations[i], figureIndexes[i]);
            }
            
            // 生成额外的figure内容
            extraFigureIndex = gameState.ExtraFigureIndex;
            extraFigureRotation = gameState.ExtraFigureRotation;
            if (extraFigureIndex > 0)
            {
                SpawnFigure(extraFigureController, extraFigureIndex, extraFigureRotation, 0);
            }

            CheckFigures();
            return true;
        }

        public override void SaveGame()
        {
            var numbers = new int[bricksCount.x * bricksCount.y];
            for (var x = 0; x < bricksCount.x; x++)
            {
                for (var y = 0; y < bricksCount.y; y++)
                {
                    numbers[x * bricksCount.y + y] = field[x, y] != null ? field[x, y].Number : 0;
                }
            }

            var indexes = new int[figures.Length];
            for (var i = 0; i < figures.Length; i++)
            {
                indexes[i] = figureControllers[i].bricks.Count > 0 ? ((NumberedBrick) figureControllers[i].bricks[0]).Number : 0;
            }

            gameState.SetField(numbers);
            gameState.SetFigures(figures, indexes, figureRotations);
            gameState.ExtraFigureIndex = extraFigureIndex;
            gameState.ExtraFigureRotation = extraFigureRotation;
            UserProgress.Current.SaveGameState(name);
        }

        protected override void ClearGameState()
        {
            base.ClearGameState();
            UserProgress.Current.ClearGameState(name);
        }

        private void SpawnBrick(Vector2Int coords, int number)
        {
            var brick = Instantiate(brickPrefab, fieldTransform);

            brick.transform.SetParent(fieldTransform, false);
            brick.RectTransform.anchorMin = Vector2.zero;
            brick.RectTransform.anchorMax = Vector2.zero;
            brick.RectTransform.anchoredPosition = GetBrickPosition(coords);

            SetBrickNumber(brick, number);
            brick.PointerClick += OnHighlightedTargetClick;

            field[coords.x, coords.y] = brick;
        }

        private static void SetBrickNumber(NumberedBrick brick, int number)
        {
            brick.Number = number;
            brick.ColorIndex = number - 1;
        }

        protected virtual Brick SpawnEmptyBrick(Vector2Int coords)
        {
            var brick = Instantiate(emptyBrickPrefab, fieldTransform);

            brick.transform.SetParent(fieldTransform, false);
            brick.RectTransform.anchorMin = Vector2.zero;
            brick.RectTransform.anchorMax = Vector2.zero;
            brick.RectTransform.anchoredPosition = GetBrickPosition(new Vector2(coords.x, coords.y));
            brick.PointerClick += OnHighlightedTargetClick;

            backgroundBricks[coords.x, coords.y] = brick;

            return brick;
        }

        // 是否运用算法
        private bool isEnableAlgorithm = false;

        private void SpawnNewFigures(bool enableAlgorithm)
        {
            figures = new int[figureControllers.Length];
            figureRotations = new float[figureControllers.Length];

            // 计算第几个位置应用算法
            var algorithmIndex = -1;
            // if (enableAlgorithm && !isEnableAlgorithm)
            {
                algorithmIndex = Random.Range(0, figureRotations.Length);
            }
            // isEnableAlgorithm = !isEnableAlgorithm;
            
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

                        SpawnFigure(figureControllers[i], figure, rotation, GetRandomBrickNumber());

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

            // foreach (var item in tempXs)
            // {
            //     Debug.LogError($"x{item}");
            // }
            //
            // foreach (var item in tempYs)
            // {
            //     Debug.LogError($"y{item}");
            // }
            
            //得到其中一个
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
                            Debug.LogError($"x{nullCount}+{tempX}");
                            SpawnFigure(figureController, nullCount - 1, 90, GetRandomBrickNumber());
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
                            Debug.LogError($"y{nullCount}+{tempY}");
                            SpawnFigure(figureController, nullCount - 1, 0, GetRandomBrickNumber());
                            return true;
                        default:
                            return false;
                    }
                }
            }

            return false;
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

                    SetBrickNumber(brick, brickNumber);
                }
            }

            figureController.Rotate(rotation);
        }

        RaycastHit2D[] rayCastHits = new RaycastHit2D[1];
        private void FigureOnPointerUp(FigureController figureController)
        {
            bricksHighlighter.UnhighlightBricks();

            if (!TryGetCoords(figureController.bricks, out var coords))
            {
                bricksHighlighter.UnhighlightNumberedBricks();

                if (gameState.UnlockedPutArea)
                {
                    // 判断是否能放置在额外容器中
                    var screenPos = Camera.main.WorldToScreenPoint(figureController.transform.position);
                    var ray = Camera.main.ScreenPointToRay(screenPos);
                    var size = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, rayCastHits, 10);
                    if (size > 0)
                    {
                        foreach (var item in rayCastHits)
                        {
                            if (item.transform != null && item.transform.name == "ExtraPutArea")
                            {
                                if (extraFigureController.bricks.Count <= 0)
                                {
                                    // extraFigureController.Interactable = true;
                                    for (var i = 0; i < figureController.bricks.Count; i++)
                                    {
                                        extraFigureController.transform.rotation = figureController.transform.rotation;
                                        var brick = figureController.bricks[i];
                                        var rectTransform = brick.RectTransform;
                                        var localPos = brick.transform.localPosition;
                                        rectTransform.SetParent(extraFigureController.transform, false);
                                        rectTransform.localPosition = localPos;
                                        extraFigureController.bricks.Add(brick);
                                    }
                    
                                    var index = Array.IndexOf(figureControllers, figureController);
                                    extraFigureIndex = figures[index];
                                    extraFigureRotation = figureRotations[index];
                                    figures[index] = -1;
                                    figureController.bricks.Clear();
                                    figureController.ResetPosition();
                                    if (figureControllers.All(c => c.bricks.Count == 0))
                                        SpawnNewFigures(true);
                                    SaveGame();
                                }
                                break;
                            }
                        }   
                    }   
                }
                return;
            }

            SaveGameState();

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

                gameState.Score++;
            }

            AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Stab);
            VibrateHelper.VibrateLight();

            if (figureController != extraFigureController)
            {
                var index = Array.IndexOf(figureControllers, figureController);
                figures[index] = -1;
            }

            figureController.bricks.Clear();
            figureController.ResetPosition();

            CheckLines();

            if (figureControllers.All(c => c.bricks.Count == 0))
                SpawnNewFigures(true);

            CheckFigures();

            SaveGame();
            CheckGameOver();
        }

        private void FigureOnPointerDrag(FigureController figureController)
        {
            if (!TryGetCoords(figureController.bricks, out var coords))
            {
                bricksHighlighter.UnhighlightBricks();
                bricksHighlighter.UnhighlightNumberedBricks();
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

            foreach (var c in coords)
                field[c.x, c.y] = null;
        }

        private int completeLineCount;

        protected virtual Vector2Int[] GetCompleteLines()
        {
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
                    continue;
                completeLineCount++;
                for (var y = 0; y < bricksCount.y; y++)
                    linesBricks.Add(new Vector2Int(x, y));
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
                    continue;
                completeLineCount++;
                for (var x = 0; x < bricksCount.x; x++)
                    linesBricks.Add(new Vector2Int(x, y));
            }

            return linesBricks.Distinct().ToArray();
        }

        private void CheckLines()
        {
            var bricksToDestroy = GetCompleteLines();
            
            if (bricksToDestroy.Length > 0)
            {
                AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Win);
                VibrateHelper.VibrateMedium();
            }
            
            var addScore = ComboManager.Instance.AddPlayerOperate(GameType.PutBlockGame, completeLineCount, Vector3.zero);
            gameState.Score += addScore;

            foreach (var c in bricksToDestroy)
            {
                var brick = field[c.x, c.y];
                brick.DoMergingAnimation(() =>
                {
                    bricksHighlighter.Unhighlight(brick);
                    Destroy(brick.gameObject);
                });

                field[c.x, c.y] = null;

                gameState.Score++;
            }

            SaveGame();
        }

        private void CheckGameOver()
        {
            if (figureControllers.Any(figure => figure.bricks.Count > 0 && IsCanPlaceFigure(figure)) 
                || (extraFigureController.bricks.Count > 0 && IsCanPlaceFigure(extraFigureController)))
                return;

            // 判断是否复活过
            if (gameState.HasRevive)
            {
                gameState.IsGameOver = true;
                putBlockGameOverPanel.gameObject.SetActive(true);
                putBlockGameOverPanel.Show();
                UserProgress.Current.ClearGameState(name);
                OnGameOver();
            }
            else
            {
                putBlockGameRevivePanel.Show(OnRevive, () =>
                {
                    gameState.IsGameOver = true;
                    putBlockGameOverPanel.gameObject.SetActive(true);
                    putBlockGameOverPanel.Show();
                    UserProgress.Current.ClearGameState(name);
                    OnGameOver();
                });
            }
        }

        private void CheckFigures()
        {
            foreach (var figureController in figureControllers)
            {
                if (figureController.bricks.Count == 0)
                    continue;

                var canPlaceFigure = IsCanPlaceFigure(figureController);
                // figureController.Interactable = canPlaceFigure;
                foreach (var brick in figureController.bricks.Cast<NumberedBrick>())
                {
                    brick.SetOverrideColorType(canPlaceFigure ? null : ColorType.Inactive);
                }
            }

            if (extraFigureController.bricks.Count > 0)
            {
                var canPlaceFigure = IsCanPlaceFigure(extraFigureController);
                // figureController.Interactable = canPlaceFigure;
                foreach (var brick in extraFigureController.bricks.Cast<NumberedBrick>())
                {
                    brick.SetOverrideColorType(canPlaceFigure ? null : ColorType.Inactive);
                }
            }
        }

        private void OnRevive()
        {
            gameState.HasRevive = true;
            // 生成小单位的格子
            foreach (var item in figureControllers)
            {
                foreach (var brick in item.bricks)
                {
                    Destroy(brick.gameObject);
                }
                
                item.bricks.Clear();
                item.ResetPosition();
                // item.Interactable = true;
            }
            
            figures = new int[figureControllers.Length];
            figureRotations = new float[figureControllers.Length];
            for (var i = 0; i < figureControllers.Length; i++)
            {
                var figureIndex = i == 0 ? 0 : (Random.Range(0, 1) > 0.5f ? 0 : 1);
                SpawnFigure(figureControllers[i], figureIndex, 0, GetRandomBrickNumber());
                figures[i] = figureIndex;
                figureRotations[i] = 0;
            }
            SaveGame();
        }

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
                // item.Interactable = true;
            }
            
            figures = new int[figureControllers.Length];
            figureRotations = new float[figureControllers.Length];
            var tempIndex = Random.Range(0, figureControllers.Length);
            for (var i = 0; i < figureControllers.Length; i++)
            {
                var figureIndex = i == tempIndex ? 0 : Random.Range(1, 5);
                var tempRotation = Random.Range(0, 4) * 90;
                SpawnFigure(figureControllers[i], figureIndex, tempRotation, GetRandomBrickNumber());
                figures[i] = figureIndex;
                figureRotations[i] = tempRotation;
            }
            SaveGame();
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

        protected override void OnLastChanceCompleted()
        {
            gameState.IsGameOver = false;
            gameState.ClearSave();
            
            CheckFigures();

            SaveGame();
            CheckGameOver();
        }

        protected override void OnClearGame()
        {
            foreach (var figureController in figureControllers)
                RemoveFigure.Execute(figureController);
        }

        protected override void HighlightFigures(bool active)
        {
            foreach (var figure in figureControllers)
            {
                // figure.Interactable = !active;

                foreach (var brick in figure.bricks)
                    brick.GetComponentInChildren<Image>().raycastTarget = !active;

                SetSortingOrder(figure.gameObject, active);
            }
        }

        protected override void OnFigureRemoved(FigureController figure)
        {
            var index = Array.IndexOf(figureControllers, figure);
            figures[index] = -1;

            if (figureControllers.All(c => c.bricks.Count == 0))
                SpawnNewFigures(true);
            
            CheckFigures();
            CheckGameOver();
        }
    }
}