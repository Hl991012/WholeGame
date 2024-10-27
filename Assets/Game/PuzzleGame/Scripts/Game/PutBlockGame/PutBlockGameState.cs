using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleGame.Gameplay.Puzzle1010
{
    [Serializable]
    public class PutBlockGameState : GameStateBaseModel
    {
        [SerializeField] private List<PutBlockGameStateSaveInfo> saves = new List<PutBlockGameStateSaveInfo>();
        [SerializeField] private int[] figures = Array.Empty<int>();
        [SerializeField] private float[] figureRotations = Array.Empty<float>();
        [SerializeField] private int[] figureIndexes = Array.Empty<int>();
        [SerializeField] private int extraFigureIndex;
        [SerializeField] private float extraFigureRotation;
        [SerializeField] private bool hasRevive;
        [SerializeField] private SerializableDictionary<BoosterType, int> useBoosters = new();
        [SerializeField] private bool unlockedPutArea;
        
        public int ExtraFigureIndex
        {
            get => extraFigureIndex;
            set => extraFigureIndex = value;
        }

        public float ExtraFigureRotation
        {
            get => extraFigureRotation;
            set => extraFigureRotation = value;
        }

        public bool HasRevive {
            get => hasRevive;
            set
            {
                hasRevive = value;
                StateUpdate?.Invoke();
            }
        }

        public bool CanUndo => saves.Count > 0;

        public void AddUseBoosterCount(BoosterType boosterType)
        {
            if (!useBoosters.TryAdd(boosterType, 1))
            {
                useBoosters[boosterType]++;
            }

            StateUpdate?.Invoke();
        }

        public int GetBoosterUseCount(BoosterType boosterType)
        {
            if (useBoosters.TryGetValue(boosterType, out var temp))
            {
                return temp;
            }
            return 0;
        }

        public bool UnlockedPutArea
        {
            get => unlockedPutArea;
            set
            {
                unlockedPutArea = value;
                StateUpdate?.Invoke();
            }
        }

        public void SetFigures(int[] value, int[] indexes, float[] rotations)
        {
            figures = (int[]) value.Clone();
            figureIndexes = (int[]) indexes.Clone();
            figureRotations = (float[]) rotations.Clone();
        }

        public int[] GetFigures()
        {
            return (int[]) figures.Clone();
        }

        public float[] GetFigureRotations()
        {
            return (float[]) figureRotations.Clone();
        }
    
        public int[] GetFigureIndexes()
        {
            return (int[]) figureIndexes.Clone();
        }

        public override void SaveGameState()
        {
            base.SaveGameState();
        
            var save = new PutBlockGameStateSaveInfo
            {
                figures = GetFigures(),
                figureRotations = GetFigureRotations(),
                figureIndexes = GetFigureIndexes(),
                extraFigureIndex = ExtraFigureIndex,
                extraFigureRotation = extraFigureRotation,
            };
    
            saves.Add(save);
        }

        public override bool UndoGameState()
        {
            if (!base.UndoGameState())
                return false;
        
            var save = saves[^1];
            saves.RemoveAt(saves.Count - 1);

            figures = save.figures;
            figureRotations = save.figureRotations;
            figureIndexes = save.figureIndexes;
            ExtraFigureIndex = save.extraFigureIndex;
            ExtraFigureRotation = save.extraFigureRotation;

            return true;
        }

        public override void Reset()
        {
            // 重置当前游戏的游玩进度
            base.Reset();
            HasRevive = false;
            useBoosters.Clear();
            UnlockedPutArea = false;
            ExtraFigureIndex = -1;
        }

        public override void ClearSave()
        {
            // 清除玩家的操作数据
            base.ClearSave();
            saves.Clear();
            
            StateUpdate?.Invoke();
        }
    }

    [Serializable]
    public class PutBlockGameStateSaveInfo
    {
        [SerializeField] public int[] figures = Array.Empty<int>();
    
        [SerializeField] public float[] figureRotations = Array.Empty<float>();
    
        [SerializeField] public int[] figureIndexes = Array.Empty<int>();

        [SerializeField] public int extraFigureIndex = -1;

        [SerializeField] public float extraFigureRotation;
    }
}