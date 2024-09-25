 using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleGame.Gameplay.Boosters;
using PuzzleGame.Themes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PuzzleGame.Gameplay.Puzzle1010
{
    public class GameController1010 : BaseGameController<GameState1010>
    {
        [Header("Mode fields")]
        public Brick emptyBrickPrefab;
        public FigureController[] figureControllers;

        private Brick[,] backgroundBricks;
        private int[] figures = Array.Empty<int>();
        private float[] figureRotations = Array.Empty<float>();

        private readonly BricksHighlighter bricksHighlighter = new();

        private const int MaxBrickNumber = 7;

        private static int GetRandomBrickNumber() => Random.Range(1, MaxBrickNumber);

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
        
            gameState = UserProgress.Current.GetGameState<GameState1010>(name);
            if (gameState == null)
            {
                gameState = new GameState1010();
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
        }

        protected override void StartGame()
        {
            if (LoadGame())
                return;

            gameState.Score = 0;
            gameState.IsGameOver = false;
            SpawnNewFigures();
            SpawnStartingBricks();
            SetStartBoosters();

            SaveGame();
        }

        private void SpawnStartingBricks()
        {
            var positions = new List<Vector2Int>();
            for (var i = 0; i < bricksCount.x; i++)
            {
                for (var j = 0; j < bricksCount.y; j++)
                {
                    positions.Add(new Vector2Int(i, j));
                }
            }

            for (int i = 1; i <= 9; i++)
            {
                int rand = Random.Range(0, positions.Count);
                SpawnBrick(positions[rand], GetRandomBrickNumber());
                positions.RemoveAt(rand);
            }
        }

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
                var figure = Random.Range(0, Figures1010.Figures.Length);
                var rotation = Random.Range(0, 4) * 90f;

                SpawnFigure(figureControllers[i], figure, rotation, GetRandomBrickNumber());

                figures[i] = figure;
                figureRotations[i] = rotation;
            }
        }

        private void SpawnFigure(FigureController figureController, int figureIndex, float rotation, int brickNumber)
        {
            figureController.transform.localRotation = Quaternion.identity;

            var figure = Figures1010.Figures[figureIndex];
            for (var i = 0; i < figure.GetLength(0); i++)
            {
                for (int j = 0; j < figure.GetLength(1); j++)
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

            // if (Random.Range(0f, 1f) < coinProbability)
            // {
            //     UserProgress.Current.Coins++;
            //
            //     GameObject vfx = Resources.Load<GameObject>("CoinVFX");
            //     vfx = Instantiate(vfx, fieldTransform.parent);
            //
            //     vfx.transform.position = figureController.transform.position;
            //
            //     Destroy(vfx, 1.5f);
            // }

            // soundCollection.GetSfx(SoundId.Landing).Play();

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

            // if (bricksToDestroy.Length > 0)
                // soundCollection.GetSfx(SoundId.Merging).Play();

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

            gameState.IsGameOver = true;
            UserProgress.Current.SaveGameState(name);
            Debug.LogError("游戏结束");
            OnGameOver();
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