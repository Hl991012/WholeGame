using System;
using System.Collections.Generic;
using GameFrame;
using PuzzleGame;
using PuzzleGame.Gameplay;
using PuzzleGame.Gameplay.Boosters;
using UnityEngine;

public abstract class BaseGameController<TGameState> : BaseGameController where TGameState : GameStateBaseModel
    {
        protected TGameState gameState;
    
        protected override void SaveGameState()
        {
            gameState.SaveGameState();
        }
    }

    public abstract class BaseGameController : MonoSingleton<BaseGameController>
    {
        [Header("Base fields")] 
        public Vector2Int bricksCount;
        public RectTransform fieldTransform;
        public NumberedBrick brickPrefab;

        protected NumberedBrick[,] field;
    
        protected bool isBoosterSelected;
        protected BoosterType boosterType;
        protected string destroyVfx = "DestroyVFX";

        public int HighlightSortingOrder { get; set; }

        protected abstract void StartGame();

        public virtual void ReplayGame() { }
        
        public string ID => name;

        public abstract void SaveGame();

        protected void SetStartBoosters()
        {
            // BoostersController.Instance.SetBoosters();
        }

        /// <summary>
        /// Basic puzzles algorithms
        /// </summary>
        protected virtual List<Vector2Int> GetAdjacentCoords(Vector2Int coords)
        {
            var adjacent = new List<Vector2Int>();

            var up = new Vector2Int(coords.x, coords.y + 1);
            if (up.y < field.GetLength(1))
                adjacent.Add(up);

            var down = new Vector2Int(coords.x, coords.y - 1);
            if (down.y >= 0)
                adjacent.Add(down);

            var left = new Vector2Int(coords.x - 1, coords.y);
            if (left.x >= 0)
                adjacent.Add(left);

            var right = new Vector2Int(coords.x + 1, coords.y);
            if (right.x < field.GetLength(0))
                adjacent.Add(right);

            return adjacent;
        }

        protected virtual Vector2 GetBrickPosition(Vector2 coords)
        {
            var brickSize = GetBrickSize();
            var brickTransform = brickPrefab.GetComponent<RectTransform>();

            var brickPosition = Vector2.Scale(coords, brickSize);
            brickPosition += Vector2.Scale(brickSize, brickTransform.pivot);
            
            return brickPosition;
        }

        protected Vector2 GetBrickSize()
        {
            var rect = fieldTransform.rect;
            var brickSize = new Vector2
            {
                x = rect.width / bricksCount.x,
                y = rect.height / bricksCount.y
            };

            return brickSize;
        }

        private Vector2 GetWorldBrickSize()
        {
            var worldCorners = new Vector3[4];
            fieldTransform.GetWorldCorners(worldCorners);
            var brickSize = new Vector2
            {
                x = (worldCorners[2].x - worldCorners[0].x) / bricksCount.x,
                y = (worldCorners[2].y - worldCorners[0].y) / bricksCount.y
            };

            return brickSize;
        }

        private Vector2Int BrickPositionToCoords(Vector3 position, Vector2 pivot)
        {
            var worldCorners = new Vector3[4];
            fieldTransform.GetWorldCorners(worldCorners);

            var brickSize = GetWorldBrickSize();

            Vector2 localPoint = position - worldCorners[0] - Vector3.Scale(brickSize, pivot);
            var coords = localPoint / brickSize;

            return Vector2Int.RoundToInt(coords);
        }
    
        protected bool TryGetCoords(List<Brick> bricks, out Vector2Int[] coords)
        {
            coords = new Vector2Int[bricks.Count];
            Vector2 minPosition = bricks[0].transform.position;

            foreach (var brick in bricks)
            {
                if (brick.transform.position.x < minPosition.x)
                    minPosition.x = brick.transform.position.x;
            
                if (brick.transform.position.y < minPosition.y)
                    minPosition.y = brick.transform.position.y;
            }

            var pivot = bricks[0].GetComponent<RectTransform>().pivot;
            var minCoords = BrickPositionToCoords(minPosition, pivot);
        
            for (var i = 0; i < bricks.Count; i++)
            {
                var localCoords = ((Vector2)bricks[i].transform.position - minPosition) / GetWorldBrickSize();
                coords[i] = Vector2Int.RoundToInt(localCoords) + minCoords;
            
                if (coords[i].x < 0 || coords[i].y < 0 || coords[i].x >= bricksCount.x || coords[i].y >= bricksCount.y ||
                    field[coords[i].x, coords[i].y] != null)
                    return false;
            }
        
            return true;
        }
        
        protected virtual void HighlightField(bool active)
        {
            SetSortingOrder(fieldTransform.gameObject, active);
        }

        protected virtual void HighlightBricks(bool active)
        {
            for (var x = 0; x < bricksCount.x; x++)
            {
                for (var y = 0; y < bricksCount.y; y++)
                {
                    if(field[x, y] == null) continue;

                    SetSortingOrder(field[x, y].gameObject, active);
                }
            }
        }

        protected void SetSortingOrder(GameObject obj, bool active)
        {
            if (!obj.TryGetComponent(out SortingOrderApplier applier))
                applier = obj.AddComponent<SortingOrderApplier>();
                
            if (active)
                applier.SetSortingOrder(HighlightSortingOrder);
            else
                applier.Hide();
        }

        protected void OnHighlightedTargetClick<T>(T target)
        {
            if (!isBoosterSelected) return;
        }

        protected abstract void SaveGameState();

        protected virtual void ClearGameState() { }

        protected virtual void OnBoostersComplete()
        {
            SaveGame();
        }
    
        protected virtual void OnFigureRemoved(FigureController figure){}
    
        protected virtual void OnClearGame(){}
    
        protected Vector2Int GetCoords(Brick brick)
        {
            var pivot = brick.GetComponent<RectTransform>().pivot;
            var coords = BrickPositionToCoords(brick.transform.position, pivot);

            return coords;
        }
    
        void SpawnDestroyAnimation(Vector2Int coords, Action onComplete)
        {
            var vfx = Resources.Load<GameObject>(destroyVfx);
            vfx = Instantiate(vfx, fieldTransform);
        
            var rectTransform = vfx.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.anchoredPosition = GetBrickPosition(coords);

            var brickSize = GetBrickSize();
            var delta = GetBrickSize() - brickSize;
            brickSize *= 3;
            brickSize += delta * 2;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, brickSize.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, brickSize.y);

            this.DelayedCall(1f, () =>
            {
                Destroy(vfx);
                onComplete?.Invoke();
            });
        }
    }