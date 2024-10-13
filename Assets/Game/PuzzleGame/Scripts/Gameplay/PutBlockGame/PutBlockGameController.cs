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
        [SerializeField] private Button backHomeBtn;
        [SerializeField] private PutBlockGameOverPanel putBlockGameOverPanel;
        [SerializeField] private PutBlockGameRevivePanel putBlockGameRevivePanel;
        [SerializeField] private Brick emptyBrickPrefab;
        [SerializeField] private FigureController[] figureControllers;

        private Brick[,] backgroundBricks;
        private int[] figures = Array.Empty<int>();
        private float[] figureRotations = Array.Empty<float>();

        private readonly BricksHighlighter bricksHighlighter = new();

        private const int MaxBrickNumber = 7;

        private static int GetRandomBrickNumber() => Random.Range(1, MaxBrickNumber);

        private void Awake()
        {
            backHomeBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                SaveGame();
                GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
            });
        }

        private void Start()
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

            UserProgress.Current.CurrentGameId = name;

            foreach (var figureController in figureControllers)
            {
                figureController.PointerUp += FigureOnPointerUp;
                figureController.PointerClick += OnHighlightedTargetClick;
                figureController.PointerDrag += FigureOnPointerDrag;
            }

            StartGame();
            CheckGameOver();
        }

        protected override void StartGame()
        {
            if (LoadGame())
                return;

            gameState.Score = 0;
            gameState.IsGameOver = false;
            SpawnNewFigures();
            // SpawnStartingBricks();
            SetStartBoosters();
            SaveGame();
        }

        public override void ReplayGame()
        {
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
                item.Interactable = true;
            }
            
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

            CheckFigures();
            return true;
        }

        protected override void SaveGame()
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

        private void SpawnNewFigures()
        {
            figures = new int[figureControllers.Length];
            figureRotations = new float[figureControllers.Length];
            for (var i = 0; i < figureControllers.Length; i++)
            {
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

        private void FigureOnPointerUp(FigureController figureController)
        {
            bricksHighlighter.UnhighlightBricks();

            if (!TryGetCoords(figureController.bricks, out var coords))
            {
                bricksHighlighter.UnhighlightNumberedBricks();
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

            var index = Array.IndexOf(figureControllers, figureController);
            figures[index] = -1;

            figureController.bricks.Clear();
            figureController.ResetPosition();

            CheckLines();

            if (figureControllers.All(c => c.bricks.Count == 0))
                SpawnNewFigures();

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

        protected virtual Vector2Int[] GetCompleteLines()
        {
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
            if (figureControllers.Any(figure => figure.bricks.Count > 0 && IsCanPlaceFigure(figure)))
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
                figureController.Interactable = canPlaceFigure;
                foreach (var brick in figureController.bricks.Cast<NumberedBrick>())
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
                item.Interactable = true;
            }
            
            figures = new int[figureControllers.Length];
            figureRotations = new float[figureControllers.Length];
            for (var i = 0; i < figureControllers.Length; i++)
            {
                var figureIndex = i == 0 ? 0 : (Random.Range(0, 1) > 0.5f ? 0 : 1);
                SpawnFigure(figureControllers[i], figureIndex, 0, GetRandomBrickNumber());
                figures[i] = 0;
                figureRotations[i] = 0;
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
                figure.Interactable = !active;

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
                SpawnNewFigures();
            
            CheckFigures();
            CheckGameOver();
        }
    }
}