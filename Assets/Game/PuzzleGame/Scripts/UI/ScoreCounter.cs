using PuzzleGame.Gameplay;
using TMPro;
using UnityEngine;

namespace PuzzleGame.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ScoreCounter : MonoBehaviour
    {
        TextMeshProUGUI labelTmp;

        protected GameStateModel currentGameStateModel;

        protected virtual int Value => currentGameStateModel.Score;

        private void Start()
        {
            labelTmp = GetComponent<TextMeshProUGUI>();

            OnProgressUpdate();
            UserProgress.Current.ProgressUpdate += OnProgressUpdate;
        }

        private void OnDestroy()
        {
            UserProgress.Current.ProgressUpdate -= OnProgressUpdate;

            if (currentGameStateModel != null)
                currentGameStateModel.StateUpdate -= OnStateUpdate;
        }

        private void OnProgressUpdate()
        {
            var gameState = UserProgress.Current.GetGameState<GameStateModel>(UserProgress.Current.CurrentGameId);

            if (currentGameStateModel != null)
                currentGameStateModel.StateUpdate -= OnStateUpdate;

            currentGameStateModel = gameState;

            if (gameState == null)
                return;

            OnStateUpdate();
            gameState.StateUpdate += OnStateUpdate;
        }

        private void OnStateUpdate()
        {
            labelTmp.text = Value.ToString();
        }
    }
}