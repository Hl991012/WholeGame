using System;
using DG.Tweening;
using PuzzleGame.Gameplay;
using TMPro;
using UnityEngine;

namespace PuzzleGame.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ScoreCounter : MonoBehaviour
    {
        public bool needIncreaseAnim;
        
        TextMeshProUGUI labelTmp;

        protected GameStateBaseModel currentGameStateBaseModel;

        protected virtual int Value => currentGameStateBaseModel.Score;

        private int curShowValue;

        private void Awake()
        {
            labelTmp = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            isFirstSetValue = true;
            OnProgressUpdate();
            UserProgress.Current.ProgressUpdate += OnProgressUpdate;
        }

        private void OnDisable()
        {
            UserProgress.Current.ProgressUpdate -= OnProgressUpdate;

            if (currentGameStateBaseModel != null)
                currentGameStateBaseModel.StateUpdate -= OnStateUpdate;
        }

        private void OnProgressUpdate()
        {
            var gameState = UserProgress.Current.GetGameState<GameStateBaseModel>(UserProgress.Current.CurrentGameId);

            if (currentGameStateBaseModel != null)
                currentGameStateBaseModel.StateUpdate -= OnStateUpdate;

            currentGameStateBaseModel = gameState;

            if (gameState == null)
                return;

            OnStateUpdate();
            gameState.StateUpdate += OnStateUpdate;
        }

        private Tweener increaseAnim;

        private bool isFirstSetValue;
        
        private void OnStateUpdate()
        {
            if (isFirstSetValue || Value <= 0)
            {
                curShowValue = Value;
                labelTmp.text = Value.ToString();
                isFirstSetValue = false;
                return;
            }
            
            if (Value != curShowValue && needIncreaseAnim)
            {
                increaseAnim?.Kill();
                increaseAnim = DOTween.To(val =>
                    {
                        curShowValue = Mathf.CeilToInt(val);
                        labelTmp.text = curShowValue.ToString();
                    }, curShowValue, Value, 0.5f)
                .SetLink(gameObject)
                .SetUpdate(true);
            }
            else
            {
                curShowValue = Value;
                labelTmp.text = Value.ToString();
            }
        }
    }
}