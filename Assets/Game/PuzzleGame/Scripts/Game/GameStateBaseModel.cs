using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleGame.Gameplay
{
    [Serializable]
    public class GameStateBaseModel
    {
        public Action StateUpdate;

        [SerializeField] private int score;
        [SerializeField] private int topScore;

        [SerializeField] private int[] field = Array.Empty<int>();

        [SerializeField] private int[] nextBricks = Array.Empty<int>();

        [SerializeField] public List<GameSave> fieldSaves = new List<GameSave>();
    
        [SerializeField] private bool isGameOver;

        [SerializeField] private string themeId;

        public int Score
        {
            get => score;
            set
            {
                score = value;

                if (score > topScore)
                    topScore = score;

                StateUpdate?.Invoke();
            }
        }

        public int TopScore => topScore;

        public bool IsGameOver
        {
            get => isGameOver;
            set => isGameOver = value;
        }

        public string ThemeId
        {
            get => themeId;
            set => themeId = value;
        }

        public int[] GetField()
        { 
            return (int[]) field.Clone();
        }

        public void SetField(int[] value)
        {
            field = (int[]) value.Clone();
        }

        public int[] GetNextBricks()
        {
            return (int[]) nextBricks.Clone();
        }

        public void SetNextBricks(int[] value)
        {
            nextBricks = (int[]) value.Clone();
        }

        public virtual void SaveGameState()
        {
            GameSave save = new GameSave
            {
                score = Score,
                field = GetField(),
                nextBricks = GetNextBricks()
            };

            fieldSaves.Add(save);
        }
    
        public virtual bool UndoGameState()
        {
            if (fieldSaves.Count == 0) return false;

            GameSave save = fieldSaves[^1];
            fieldSaves.RemoveAt(fieldSaves.Count - 1);

            score = save.score;
            field = save.field;
            nextBricks = save.nextBricks;
            isGameOver = false;
        
            return true;
        }
    
        public virtual void Reset()
        {
            isGameOver = true;
            score = 0;
            ClearSave();
        }

        public virtual void ClearSave()
        {
            fieldSaves.Clear();
        }
    }

    [Serializable]
    public class GameSave
    {
        [SerializeField]
        public int score;
    
        [SerializeField]
        public int[] field = Array.Empty<int>();
    
        [SerializeField]
        public int[] nextBricks = Array.Empty<int>();
    }
}