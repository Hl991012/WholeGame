using System;
using System.Collections.Generic;
using System.Linq;
using NMNH.Utility;
using PuzzleGame.Gameplay.Boosters;
using PuzzleGame.Input;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PuzzleGame.Gameplay.Bricks2048
{
    public class X2BlocksGameController : BaseGameController<X2BlocksGameState>
    {
        [Header("Mode fields")]
        [SerializeField] private float speed;
        [SerializeField] private float fallSpeed;
        [SerializeField] private RectTransform fieldBackGround;
        [SerializeField] private Transform nextBrickPoint;
        [SerializeField] private Transform columnsParent;
        [SerializeField] private Brick columnPrefab;
        [SerializeField] private bool spawnColumns;
        [SerializeField] private Button backBtn;
        [SerializeField] private Button pauseBtn;
        [SerializeField] private X2BlockGameOverPanel x2BlockGameOverPanel;
        private NumberedBrick nextBrick;
        private NumberedBrick currentBrick;
        private Vector2Int currentBrickCoords;

        private float timeSinceMoveDown;

        private bool isFalling;

        private class BrickPath
        {
            public NumberedBrick brick;
            public List<Vector2Int> path;
        }

        private bool IsAnimating { get; set; }

        private NumberedBrick CurrentBrick
        {
            get
            {
                if(currentBrick == null)
                    CurrentBrick = SpawnBrick(currentBrickCoords, GetRandomNumber());
            
                return currentBrick;
            }
            set => currentBrick = value;
        }

        private static int GetRandomNumber()
        {
            return Mathf.RoundToInt(Mathf.Pow(2, Random.Range(1, 5)));
        }

        private static int GetColorIndex(int number)
        {
            return Mathf.RoundToInt(Mathf.Log(number, 2) - 1);
        }

        private void Awake()
        {
            backBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
            });
            
            pauseBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                if (Time.timeScale == 1)
                {
                    WXSDKManager.Instance.ShowInterstitialVideo(null);
                }
                
                Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            });
        }

        private void Start()
        {
            if (spawnColumns)
                SpawnColumns();

            InputController.Left += OnLeft;
            InputController.Right += OnRight;
            InputController.Down += OnDown;
            InputController.Move += OnTapMove;
        }

        public void PlayGame()
        {
            IsAnimating = false;
            isFalling = false;

            if (field != null)
            {
                // 清除已经生成的内容
                foreach (var item in field)
                {
                    if (item != null)
                    {
                        DestroyImmediate(item.gameObject);
                    }
                }
            }
            else
            {
                field = new NumberedBrick[bricksCount.x, bricksCount.y];
            }

            if (currentBrick != null)
            {
                DestroyImmediate(currentBrick.gameObject);
            }
            
            if (nextBrick != null)
            {
                DestroyImmediate(nextBrick.gameObject);
            }
            
            gameState = UserProgress.Current.GetGameState<X2BlocksGameState>(name);
            if (gameState == null)
            {
                gameState = new X2BlocksGameState();
                UserProgress.Current.SetGameState(name, gameState);
            }
            
            SpawnNextBrick();
            
            StartGame();
        }

        protected override void StartGame()
        {
            currentBrickCoords = new Vector2Int(bricksCount.x / 2, bricksCount.y - 1);
            gameState.CurrentBrickCoords = currentBrickCoords;
        
            if (LoadGame())
                return;

            gameState.Score = 0;
            gameState.IsGameOver = false;
            nextBrick.Number = GetRandomNumber();
            nextBrick.ColorIndex = GetColorIndex(nextBrick.Number);
            SetStartBoosters();

            SaveGame();
        }

        public override void ReplayGame()
        {
            gameState.IsGameOver = false;
            
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

            IsAnimating = false;
            ClearGameState();
            StartGame();
        }
        
        protected override void ClearGameState()
        {
            base.ClearGameState();
            UserProgress.Current.ClearGameState(name);
        }

        // void SpawnStartingBricks()
        // {
        //     currentBrickCoords = new Vector2Int(bricksCount.x / 2, bricksCount.y - 1);
        //     CurrentBrick = SpawnBrick(currentBrickCoords, GetRandomNumber());
        //
        //     List<int> numbers = new List<int>(bricksCount.x);
        //     for (int i = 1; i <= bricksCount.x; i++)
        //     {
        //         numbers.Add(Mathf.RoundToInt(Mathf.Pow(2, i)));
        //     }
        //
        //     for (int i = 0; i < bricksCount.x; i++)
        //     {
        //         int rand = Random.Range(0, numbers.Count);
        //         var brick = SpawnBrick(new Vector2Int(i, 0), numbers[rand]);
        //         brick.PointerClick += OnHighlightedTargetClick;
        //         field[i, 0] = brick;
        //
        //         numbers.RemoveAt(rand);
        //     }
        // }

        private void SpawnColumns()
        {
            for (var i = 0; i < bricksCount.x; i++)
            {
                var columnBrick = Instantiate(columnPrefab, columnsParent);
                var rect = columnBrick.RectTransform;
                var fieldRect = fieldTransform.rect;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.anchoredPosition = new Vector2(GetBrickPosition(new Vector2(i, 0)).x, fieldRect.height / 2);

                rect.sizeDelta = new Vector2(fieldRect.width / bricksCount.x, fieldRect.height);
                columnBrick.ColorIndex = i % 2;
            }
        }

        private void OnLeft()
        {
            if (!IsAnimating && !isFalling)
                MoveHorizontally(-1);
        }

        private void OnRight()
        {
            if (!IsAnimating && !isFalling)
                MoveHorizontally(1);
        }

        private void OnDown()
        {
            if (isBoosterSelected)
            {
                IsAnimating = true;
                OnHighlightedTargetClick(this);
                return;
            }
        
            if (IsAnimating || isFalling)
                return;

            isFalling = true;
            timeSinceMoveDown = 0f;
            MoveDown();
        }

        private void OnTapMove(int value)
        {
            if (isBoosterSelected)
                return;
        
            if (IsAnimating || isFalling) return;

            var path = 0;
            if (value < currentBrickCoords.x)
            {
                for (var i = currentBrickCoords.x - 1; i >= value; i--)
                {
                    if (field[i, currentBrickCoords.y] != null)
                        break;

                    path++;
                }
            }

            if (value > currentBrickCoords.x)
            {
                for (var i = currentBrickCoords.x + 1; i <= value; i++)
                {
                    if (field[i, currentBrickCoords.y] != null)
                        break;

                    path++;
                }
            }

            int steps = Mathf.Abs(currentBrickCoords.x - value);
            value = path < steps ? currentBrickCoords.x : value;
        
            Move(value);
        }

        void Update()
        {
            if (IsAnimating || isBoosterSelected)
                return;

            timeSinceMoveDown += Time.deltaTime;

            if (isFalling && timeSinceMoveDown >= 1f / fallSpeed)
            {
                timeSinceMoveDown -= 1f / fallSpeed;
                MoveDown();
            }

            if (isFalling || !(timeSinceMoveDown >= 1 / speed)) return;
            timeSinceMoveDown -= 1f / speed;
            MoveDown();
        }

        protected virtual bool LoadGame()
        {
            if (gameState == null || gameState.IsGameOver)
                return false;

            var numbers = gameState.GetField();
            if (numbers == null || numbers.Length != bricksCount.x * bricksCount.y)
                return false;

            for (int x = 0; x < bricksCount.x; x++)
            {
                for (int y = 0; y < bricksCount.y; y++)
                {
                    if (numbers[x * bricksCount.y + y] > 0)
                        field[x, y] = SpawnBrick(new Vector2Int(x, y), numbers[x * bricksCount.y + y]);
                }
            }

            currentBrickCoords = gameState.CurrentBrickCoords;
            CurrentBrick = SpawnBrick(currentBrickCoords, gameState.CurrentBrick);
            nextBrick.Number = gameState.NextBrick;
            nextBrick.ColorIndex = GetColorIndex(nextBrick.Number);

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

            gameState.SetField(numbers);
            gameState.CurrentBrickCoords = currentBrickCoords;
            gameState.CurrentBrick = CurrentBrick.Number;
            gameState.NextBrick = nextBrick.Number;
            UserProgress.Current.SaveGameState(name);
        }

        NumberedBrick SpawnBrick(Vector2Int coords, int number)
        {
            var brick = Instantiate(brickPrefab, fieldTransform);

            brick.transform.SetParent(fieldTransform, false);
            brick.RectTransform.anchorMin = Vector2.zero;
            brick.RectTransform.anchorMax = Vector2.zero;
            brick.RectTransform.anchoredPosition = GetBrickPosition(new Vector2(coords.x, coords.y));

            brick.Number = number;
            brick.ColorIndex = GetColorIndex(number);
            brick.PointerClick += OnHighlightedTargetClick;

            return brick;
        }

        private void SpawnNextBrick()
        {
            nextBrick = Instantiate(brickPrefab, nextBrickPoint);
            nextBrick.RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            nextBrick.RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            nextBrick.RectTransform.anchoredPosition = Vector2.zero;
        }

        private void MoveDown()
        {
            if (currentBrickCoords.y > 0 && field[currentBrickCoords.x, currentBrickCoords.y - 1] == null)
            {
                currentBrickCoords.y--;
                CurrentBrick.RectTransform.anchoredPosition =
                    GetBrickPosition(new Vector2(currentBrickCoords.x, currentBrickCoords.y));

                SaveGame();
            }
            else
            {
                SaveGameState();

                IsAnimating = true;
                AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Drop);
                VibrateHelper.VibrateMedium();
                
                CurrentBrick.DoLandingAnimation(
                    () =>
                    {
                        IsAnimating = false;
                        field[currentBrickCoords.x, currentBrickCoords.y] = CurrentBrick;

                        Merge(
                            new List<Vector2Int> {currentBrickCoords},
                            () =>
                            {
                                isFalling = false;

                                currentBrickCoords = new Vector2Int(bricksCount.x / 2, bricksCount.y - 1);

                                if (field[currentBrickCoords.x, currentBrickCoords.y] != null)
                                {
                                    IsAnimating = true;

                                    gameState.IsGameOver = true;
                                    UserProgress.Current.SaveGameState(name);
                                    
                                    // 展示失败弹窗
                                    x2BlockGameOverPanel.Show();
                                    
                                    OnGameOver();
                                    return;
                                }
                            
                                CurrentBrick = SpawnBrick(currentBrickCoords, nextBrick.Number);
                                nextBrick.Number = GetRandomNumber();
                                nextBrick.ColorIndex = GetColorIndex(nextBrick.Number);

                                SaveGame();
                            }
                        );
                    }
                );
            }
        }

        private void MoveHorizontally(int value)
        {
            int x = currentBrickCoords.x + value;
            Move(x);
        }

        private void Move(int value)
        {
            if (value < 0 || value >= field.GetLength(0) || field[value, currentBrickCoords.y] != null)
                return;
        
            currentBrickCoords.x = value;
            CurrentBrick.RectTransform.anchoredPosition =
                GetBrickPosition(new Vector2(currentBrickCoords.x, currentBrickCoords.y));
        }

        private void Merge(List<Vector2Int> toMerge, Action onComplete)
        {
            IsAnimating = true;

            var newCoords = new List<Vector2Int>();

            var animationsLeft = 0;
            foreach (var coords in toMerge)
            {
                if (field[coords.x, coords.y] == null)
                    continue;

                var brick = field[coords.x, coords.y];
                var area = WaveAlgorithm.GetArea(
                    field,
                    coords,
                    GetAdjacentCoords,
                    b => b != null && b.Number == brick.Number
                );

                if (area.Count < 2)
                    continue;

                newCoords.AddRange(area);

                var paths = new List<BrickPath>();
                foreach (var toMove in area)
                {
                    if (toMove == coords)
                    {
                        continue;
                    }

                    var brickPath = new BrickPath
                    {
                        brick = field[toMove.x, toMove.y],
                        path = WaveAlgorithm.GetPath(
                            field,
                            toMove,
                            coords,
                            GetAdjacentCoords,
                            b => b != null && b.Number == brick.Number
                        )
                    };
                    brickPath.path.RemoveAt(0);
                    paths.Add(brickPath);
                }

                foreach (Vector2Int toMove in area)
                    if (toMove != coords)
                        field[toMove.x, toMove.y] = null;

                animationsLeft++;

                var areaSize = area.Count;
                AnimateMerge(
                    paths,
                    () =>
                    {
                        animationsLeft--;

                        if (animationsLeft > 0)
                            return;

                        AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.PutUpBlock);
                        VibrateHelper.VibrateMedium();

                        brick.Number *= Mathf.ClosestPowerOfTwo(areaSize);
                        brick.ColorIndex = GetColorIndex(brick.Number);
                        brick.DoMergingAnimation(
                            () =>
                            {
                                if (newCoords.Count > 0)
                                    Normalize(
                                        normalized =>
                                        {
                                            newCoords.AddRange(normalized);
                                            Merge(newCoords, onComplete);
                                        }
                                    );
                            }
                        );

                        gameState.Score += brick.Number;
                    }
                );
            }

            if (newCoords.Count > 0)
                return;

            IsAnimating = false;
            onComplete.Invoke();
        }

        private void AnimateMerge(List<BrickPath> brickPaths, Action onComplete)
        {
            brickPaths = brickPaths.OrderByDescending(p => p.path.Count).ToList();

            var pathLength = brickPaths[0].path.Count;

            if (pathLength == 0)
            {
                brickPaths.ForEach(p => Destroy(p.brick.gameObject));
                onComplete.Invoke();
                return;
            }

            var animationsLeft = 0;
            foreach (var brickPath in brickPaths)
            {
                if (brickPath.path.Count < pathLength)
                    break;

                Vector2 position = GetBrickPosition(brickPath.path[0]);

                brickPath.path.RemoveAt(0);

                animationsLeft++;
                brickPath.brick.DoLocalMove(
                    position,
                    () =>
                    {
                        animationsLeft--;
                        if (animationsLeft == 0)
                            AnimateMerge(brickPaths, onComplete);
                    }
                );
            }
        }

        private void Normalize(Action<List<Vector2Int>> onComplete)
        {
            var normalized = new List<Vector2Int>();
            for (var x = 0; x < field.GetLength(0); x++)
            {
                for (var y = 0; y < field.GetLength(1); y++)
                {
                    var brick = field[x, y];

                    if (brick == null)
                        continue;

                    var yEmpty = y;
                    while (yEmpty > 0 && field[x, yEmpty - 1] == null)
                        yEmpty--;

                    if (yEmpty == y)
                        continue;

                    field[x, y] = null;
                    field[x, yEmpty] = brick;
                    var brickCoords = new Vector2Int(x, yEmpty);

                    normalized.Add(brickCoords);

                    var isFirst = normalized.Count == 1;
                    brick.DoLocalMove(
                        GetBrickPosition(brickCoords),
                        () =>
                        {
                            if (isFirst)
                            {
                                brick.DoLandingAnimation(() => onComplete.Invoke(normalized));
                                VibrateHelper.VibrateMedium();
                            }
                            else
                                brick.DoLandingAnimation(null);
                        }
                    );
                }
            }

            if (normalized.Count == 0)
                onComplete.Invoke(normalized);
        }

        public override void LastChance(LastChance lastChance)
        {
            field[currentBrickCoords.x, currentBrickCoords.y] = CurrentBrick;

            switch (lastChance.LastChanceType)
            {
                case LastChanceType.Numbers:
                    ClearNumbers.Execute(field, lastChance.MaxNumber, OnLastChanceCompleted);
                    break;
                case LastChanceType.CrossLines:
                    ClearCrossLines.Execute(field, OnLastChanceCompleted);
                    break;
                case LastChanceType.LinesHorizontal:
                    var coordY = bricksCount.y - lastChance.LinesCount;
                    ClearHorizontalLines.Execute(field, coordY, lastChance.LinesCount, OnLastChanceCompleted);
                    break;
                case LastChanceType.LinesVertical:
                    int coordX = (bricksCount.x - lastChance.LinesCount) / 2;
                    ClearVerticalLines.Execute(field, coordX, lastChance.LinesCount, OnLastChanceCompleted);
                    break;
                case LastChanceType.Explosion:
                    var coords = new Vector2Int(bricksCount.x / 2, bricksCount.y / 2);
                    AnimateDestroy(coords, OnLastChanceCompleted);
                    return;
            }
        }

        protected override void OnLastChanceCompleted()
        {
            Normalize(
                normalized =>
                {
                    Merge(normalized, () =>
                    {
                        currentBrickCoords = new Vector2Int(bricksCount.x / 2, bricksCount.y - 1);
                        gameState.CurrentBrickCoords = currentBrickCoords;

                        currentBrick = SpawnBrick(currentBrickCoords, nextBrick.Number);
                        nextBrick.Number = GetRandomNumber();
                        nextBrick.ColorIndex = GetColorIndex(nextBrick.Number);
        
                        gameState.IsGameOver = false;
                        gameState.ClearSave();
        
                        SaveGame();
                    }); 
                }
            );
        }
    
        public override void HighlightBoosterTarget(BoosterType type, bool active)
        {
            isBoosterSelected = active;
            boosterType = type;

            switch (type)
            {
                case BoosterType.ClearBrick:
                case BoosterType.ClearNumber:
                case BoosterType.Explosion:
                case BoosterType.ClearHorizontalLine:
                case BoosterType.ClearVerticalLine:
                    HighlightBricks(active);
                    break;
                case BoosterType.RemoveFigure:
                    break;
                case BoosterType.Undo:
                    HighlightField(active);
                    break;
            }
        }

        protected override void HighlightField(bool active)
        {
            IsAnimating = active;
        
            SetSortingOrder(fieldBackGround.gameObject, active);
        }

        protected override void HighlightBricks(bool active)
        {
            base.HighlightBricks(active);

            for (int x = 0; x < bricksCount.x; x++)
            {
                for (int y = 0; y < bricksCount.y; y++)
                {
                    if(field[x, y] == null) continue;

                    field[x, y].GetComponentInChildren<Image>().raycastTarget = active;
                }
            }
        }

        protected override void OnClearGame()
        {
            if(currentBrick != null)
                Destroy(currentBrick.gameObject);
        }

        protected override void OnBoostersComplete()
        {
            Normalize(
                normalized =>
                {
                    Merge(normalized, () =>
                    {
                        base.OnBoostersComplete(); 
                    });
                }
            );
        }
        
        private void OnDestroy()
        {
            InputController.Left -= OnLeft;
            InputController.Right -= OnRight;
            InputController.Down -= OnDown;
            InputController.Move -= OnTapMove;
        }
    }
}