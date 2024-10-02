using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PuzzleGame;
using PuzzleGame.Gameplay;
using PuzzleGame.Gameplay.Puzzle1010;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PutBlockGameOverPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curScoreTmp;
    [SerializeField] private TextMeshProUGUI bestScoreTmp;
    [SerializeField] private Button onceAgainBtn;
    [SerializeField] private CanvasGroup canvasGroup;

    private Sequence showAnimSeq;
    
    private void Awake()
    {
        onceAgainBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            PutBlockGameController.Instance.ReplayGame();
            canvasGroup.alpha = 1;
            showAnimSeq?.Kill();
            showAnimSeq = DOTween.Sequence()
                .Append(canvasGroup.DOFade(0, 0.5f))
                .SetLink(gameObject)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        });
    }

    public void Show()
    {
        canvasGroup.alpha = 0;
        showAnimSeq?.Kill();
        onceAgainBtn.interactable = false;
        showAnimSeq = DOTween.Sequence()
            .AppendInterval(0.3f)
            .Append(canvasGroup.DOFade(1, 0.7f))
            .SetLink(gameObject)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                onceAgainBtn.interactable = true;
            });
        
        curScoreTmp.text = UserProgress.Current.GetGameState<GameStateBaseModel>(UserProgress.Current.CurrentGameId).Score
            .ToString();
        bestScoreTmp.text = UserProgress.Current.GetGameState<GameStateBaseModel>(UserProgress.Current.CurrentGameId).TopScore
            .ToString();
    }
}
