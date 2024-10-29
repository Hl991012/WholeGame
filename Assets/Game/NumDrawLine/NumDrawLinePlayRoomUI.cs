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

        private void Awake()
        {
            destroyBoosterBtn.onClick.AddListener(() =>
            {
                numDrawLinePlayRoom.UserBooster(BoosterType.Destroy);
            });

            numDrawLinePlayRoom.OnBoosterStateChanged += OnBoosterStateChanged;
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
        }
    }
}
