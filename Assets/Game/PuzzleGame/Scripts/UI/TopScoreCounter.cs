﻿namespace PuzzleGame.UI
{
    public class TopScoreCounter : ScoreCounter
    {
        protected override int Value => currentGameStateModel.TopScore;
    }
}