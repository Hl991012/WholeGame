using System;
using DG.Tweening;
using PuzzleGame;
using PuzzleGame.Gameplay;
using PuzzleGame.Gameplay.Bricks2048;
using PuzzleGame.Gameplay.Puzzle1010;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class X2BlockGameOverPanel : MonoBehaviour
{
    [SerializeField] private GameObject infoObj;
    [SerializeField] private TextMeshProUGUI curScoreTmp;
    [SerializeField] private TextMeshProUGUI hintTmp;
    [SerializeField] private Button onceAgainBtn;
    [SerializeField] private CanvasGroup canvasGroup;

    private Sequence showAnimSeq;

    private void OnEnable()
    {
        infoObj.SetActive(false);
    }

    private void Awake()
    {
        onceAgainBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            X2BlocksGameController.Instance.ReplayGame();
            canvasGroup.alpha = 1;
            showAnimSeq?.Kill();
            showAnimSeq = DOTween.Sequence()
                .Append(canvasGroup.DOFade(0, 0.5f))
                .SetLink(gameObject)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    infoObj.SetActive(false);
                });
        });
    }

    public void Show()
    {
        infoObj.SetActive(true);
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

        var gameStata = UserProgress.Current.GetGameState<GameStateBaseModel>(UserProgress.Current.CurrentGameId);
        hintTmp.text = gameStata.Score < gameStata.TopScore ? 
            $"好可惜啊，就差{gameStata.TopScore - gameStata.Score}分就能超越历史最高分了，再试一次吧" : 
            "恭喜突破历史最高分,真是太厉害了！";
        
        curScoreTmp.text = gameStata.Score.ToString();
    }
}
