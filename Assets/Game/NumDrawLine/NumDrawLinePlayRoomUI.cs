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
        [SerializeField] private Button destroyBoosterBtn;
        [SerializeField] private TextMeshProUGUI curScoreTmp;
        [SerializeField] private TextMeshProUGUI topScoreTmp;
        [SerializeField] private Button backBtn;

        private void Awake()
        {
            destroyBoosterBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                numDrawLinePlayRoom.UserBooster(BoosterType.Destroy);
            });
            
            backBtn.onClick.AddListener(() =>
            {
                BaseUtilities.PlayCommonClick();
                
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
        }

        private void RefreshView()
        {
            userBoosterHintObj.SetActive(false);
            RefreshCurScore();
        }

        private void RefreshCurScore()
        {
            curScoreTmp.text = NumDrawLineGameManager.Instance.Data.CurScore.ToString();
            topScoreTmp.text = NumDrawLineGameManager.Instance.Data.TopScore.ToString();
        }
    }
}
