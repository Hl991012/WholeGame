using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockEliminateGame
{
    public class MainUIPresenter : MonoBehaviour
    {
        [SerializeField] private Button startNewGameBtn;
        [SerializeField] private Button endlessModeBtn;
        [SerializeField] private Button settingBtn;
        [SerializeField] private SettingPanel settingPanel;
        [SerializeField] private Button moreGameBtn;
        [SerializeField] private Button circleGameBtn;
        [SerializeField] private GameObject gameListObj;
        [SerializeField] private TextMeshProUGUI playHintTmp;

        private void Awake()
        {
            settingBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                settingPanel.gameObject.SetActive(true);
            });

            startNewGameBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                BlockEliminateStageManager.Instance.StartNewGame();
            });

            endlessModeBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                GameCenter.Instance.ChangeState(GameCenter.GameState.Game, GameType.PutBlockGame);
            });

            moreGameBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                gameListObj.SetActive(true);
                WXSDKManager.Instance.ShowInterstitialVideo(null);
                WXSDKManager.Instance.CloseCustomAd1();
            });
            
            circleGameBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                GameCenter.Instance.ChangeState(GameCenter.GameState.Game, GameType.CircleGame);
                WXSDKManager.Instance.ShowInterstitialVideo(null);
                WXSDKManager.Instance.CloseCustomAd1();
            });
        }

        public void RefreshView()
        {
            playHintTmp.text = BlockEliminateStageManager.Instance.AllLocalStagePassed() ? "敬请期待" : "开始游戏";
            RefreshStageItems();
            PlayAnim();
        }

        #region 主界面按钮出现动画

        [Header("主界面按钮出现动画")] 
        [SerializeField] private RectTransform topUIRectTrans;
        [SerializeField] private RectTransform downUIRectTrans;
        [SerializeField] private RectTransform leftUIRectTrans;
        [SerializeField] private RectTransform rightUIRectTrans;
        [SerializeField] private CanvasGroup canvasGroup;

        private void ResetDefaultPos(bool visible)
        {
            topUIRectTrans.anchoredPosition = visible ? Vector2.zero : new Vector2(0, 500);
            downUIRectTrans.anchoredPosition = visible ? Vector2.zero : new Vector2(0, -700);
            leftUIRectTrans.anchoredPosition = visible ? Vector2.zero : new Vector2(-200, 0);
            rightUIRectTrans.anchoredPosition = visible ? Vector2.zero : new Vector2(200, 0);
        }

        private Sequence uiShowAnim;

        private void PlayAnim()
        {
            canvasGroup.blocksRaycasts = false;
            uiShowAnim?.Kill();
            uiShowAnim = DOTween.Sequence()
                .SetLink(gameObject)
                .SetLink(gameObject)
                .OnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = true;
                });
            ResetDefaultPos(false);
            uiShowAnim
                .Append(topUIRectTrans.DOAnchorPos(Vector2.zero, 0.5f))
                .Join(downUIRectTrans.DOAnchorPos(Vector2.zero, 0.5f))
                .Join(leftUIRectTrans.DOAnchorPos(Vector2.zero, 0.5f))
                .Join(rightUIRectTrans.DOAnchorPos(Vector2.zero, 0.5f));
        }

        # endregion

        #region 刷新当前关卡列表

        [SerializeField] private SingleStageItem[] stageItems;

        private void RefreshStageItems()
        {
            for (var i = 0; i < stageItems.Length; i++)
            {
                stageItems[i].RefreshView(BlockEliminateStageManager.Instance.CurrentLevel + i);
            }
        }

        #endregion

        private void RefreshPlayBtn()
        {
            
        }
    }
}