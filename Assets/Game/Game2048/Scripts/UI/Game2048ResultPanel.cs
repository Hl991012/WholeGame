using DG.Tweening;
using PuzzleGame;
using PuzzleGame.Gameplay;
using PuzzleGame.Gameplay.Puzzle1010;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Game2048ResultPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curScoreTmp;
    [SerializeField] private Button onceAgainBtn;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TileManager tileManager;
    
    private Sequence showAnimSeq;
    
    private void Awake()
    {
        onceAgainBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            tileManager.RestartGame();
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
        gameObject.SetActive(true);
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
                WXSDKManager.Instance.ShowInterstitialVideo(null);
            });
        
        curScoreTmp.text = UserProgress.Current.GetGameState<GameStateBaseModel>(UserProgress.Current.CurrentGameId).Score
            .ToString();
    }
}