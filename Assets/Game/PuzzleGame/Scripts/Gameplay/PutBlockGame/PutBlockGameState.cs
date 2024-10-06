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

        public bool HasRevive { get; set; }

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
                HasRevive = HasRevive
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

            return true;
        }

        public override void ClearSave()
        {
            base.ClearSave();
            saves.Clear();
        }
    }

    [Serializable]
    public class PutBlockGameStateSaveInfo
    {
        [SerializeField] public bool HasRevive { get; set; }

        [SerializeField] public int[] figures = Array.Empty<int>();
    
        [SerializeField] public float[] figureRotations = Array.Empty<float>();
    
        [SerializeField] public int[] figureIndexes = Array.Empty<int>();
    }
}