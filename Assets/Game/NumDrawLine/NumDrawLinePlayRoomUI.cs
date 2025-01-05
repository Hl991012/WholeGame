using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NumDrawLine
{
    public class NumDrawLinePlayRoomUI : MonoBehaviour
    {
        [SerializeField] private GameObject userBoosterHintObj;
        [SerializeField] private TextMeshProUGUI useBoosterHintTmp;
        [SerializeField] private NumDrawLinePlayRoom numDrawLinePlayRoom;
        [SerializeField] private TextMeshProUGUI curScoreTmp;
        [SerializeField] private TextMeshProUGUI topScoreTmp;
        [SerializeField] private Button backBtn;
        
        [SerializeField] private Button destroyBoosterBtn;
        [SerializeField] private TextMeshProUGUI destroyBoosterTmp;
        [SerializeField] private TextMeshProUGUI adAdddestroyBoosterTmp;
        [SerializeField] private GameObject destroyBoosterWatchAdObj;
        [SerializeField] private GameObject destroyBoosterRemainShowObj;

        private void Awake()
        {
            destroyBoosterBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();

                if (BoosterManager.Instance.GetBoosterCount(GameType.NumDrawLine, BoosterType.Destroy) > 0)
                {
                    numDrawLinePlayRoom.UserBooster(BoosterType.Destroy);
                }
                else
                {
                    WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
                    {
                        if (isSuccess)
                        {
                            BoosterManager.Instance.BuyBooster(GameType.NumDrawLine, BoosterType.Destroy);
                        }
                    });
                }
            });
            
            backBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
            });

            numDrawLinePlayRoom.OnBoosterStateChanged += OnBoosterStateChanged;
            NumDrawLineGameManager.Instance.OnScoreUpdate += RefreshCurScore;
        }

        private void OnEnable()
        {
            RefreshView();
        }

        private void OnBoosterStateChanged(BoosterType boosterType)
        {
            userBoosterHintObj.SetActive(boosterType != BoosterType.None);
            switch (boosterType)
            {
                case BoosterType.Destroy:
                    useBoosterHintTmp.text = "选择一个元素点击后销毁";
                    break;
            }

            RefreshBooster();
        }
        
        private void RefreshBooster()
        {
            var destroyBoosterCount = BoosterManager.Instance.GetBoosterCount(GameType.NumDrawLine, BoosterType.Destroy);
            destroyBoosterWatchAdObj.SetActive(destroyBoosterCount <= 0);
            destroyBoosterRemainShowObj.SetActive(destroyBoosterCount > 0);
            if (destroyBoosterCount > 0)
            {
                destroyBoosterTmp.text = $"{destroyBoosterCount}";
            }
            else
            {
                var tempBoosterConfig = AllBoosterConfigManager.Instance.GetBoosterConfig(BoosterType.Undo);
                var tempAddCount = tempBoosterConfig != null ? tempBoosterConfig.CountToBuy : 1;
                adAdddestroyBoosterTmp.text = $"+{tempAddCount}";
            }
        }

        private void RefreshView()
        {
            userBoosterHintObj.SetActive(false);
            RefreshCurScore();
            RefreshBooster();
        }

        private void RefreshCurScore()
        {
            curScoreTmp.text = NumDrawLineGameManager.Instance.Data.CurScore.ToString();
            topScoreTmp.text = NumDrawLineGameManager.Instance.Data.TopScore.ToString();
        }
    }
}
