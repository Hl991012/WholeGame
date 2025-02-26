using DG.Tweening;
using PutBlockGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockEliminateGame
{
    public class BlockEliminatePlayRoomUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelTmp;
        [SerializeField] private Button backBtn;
        [SerializeField] private SingleTargetItem[] barrierTargets;

        private void Awake()
        {
            backBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
                WXSDKManager.Instance.ShowInterstitialVideo(null);
            });
        }

        private void Start()
        {
            PlayRoomPresenter.Instance.BlockEliminateGame.GameDataModel.OnScoreChanged += RefreshCurScore;
        }

        public void RefreshView(StageConfigSO stageConfigSo)
        {
            if(stageConfigSo == null) return;
            scoreTargetObj.SetActive(stageConfigSo.TargetType == GameModel.TargetType.Score);
            collectTargetObj.SetActive(stageConfigSo.TargetType == GameModel.TargetType.Barrier);

            levelTmp.text = $"第{BlockEliminateStageManager.Instance.CurrentLevel}关";
            
            switch (stageConfigSo.TargetType)
            {
                case GameModel.TargetType.Score:
                    RefreshCurScore();
                    break;
                case GameModel.TargetType.Barrier:
                    var i = 0;
                    foreach (var itemModel in stageConfigSo.Targets)
                    {
                        barrierTargets[i].gameObject.SetActive(i < stageConfigSo.Targets.Values.Count);
                        if (i < stageConfigSo.Targets.Values.Count)
                        {
                            barrierTargets[i].Init(itemModel.Value).RefreshView();
                        }
                        i++;
                    }

                    for (var j = stageConfigSo.Targets.Values.Count; j < barrierTargets.Length; j++)
                    {
                        barrierTargets[j].gameObject.SetActive(false);
                    }
                    break;
            }
        }
    
        #region 分数目标相关

        [SerializeField] private GameObject scoreTargetObj;
        [SerializeField] private TextMeshProUGUI curScoreTmp;
        [SerializeField] private TextMeshProUGUI targetScoreTmp;
        [SerializeField] private Slider scoreSlider;
        [SerializeField] private RectTransform curScoreBgRectTransform;

        private Tween scoreTween;
        
        private void RefreshCurScore()
        {
            if(BlockEliminateStageManager.Instance.CurStageConfig.TargetType != GameModel.TargetType.Score) return;
            var targetScore = BlockEliminateStageManager.Instance.CurStageConfig.Targets[GameModel.BarrierType.Score].TargetCount;
            curScoreTmp.text = PlayRoomPresenter.Instance.BlockEliminateGame.GameDataModel.Score.ToString();
            targetScoreTmp.text = targetScore.ToString();

            var endValue = (float)PlayRoomPresenter.Instance.BlockEliminateGame.GameDataModel.Score / targetScore;
            endValue = Mathf.Clamp(endValue, 0, 1);
            scoreTween?.Kill();
            scoreTween = DOTween.To(val =>
            {
                scoreSlider.value = val;
                curScoreBgRectTransform.anchoredPosition = new Vector2(40 + val * 453.35f, 0);
            }, scoreSlider.value, endValue, 0.5f);
        }

        #endregion

        #region 收集目标相关内容

        [SerializeField] private GameObject collectTargetObj;

        #endregion
        
    }   
}
