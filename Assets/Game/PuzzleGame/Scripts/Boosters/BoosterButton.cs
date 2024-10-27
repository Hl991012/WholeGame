using System;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.Gameplay.Boosters
{
    public class BoosterButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private GameObject counter;
        [SerializeField] private GameObject counterEmpty;
        [SerializeField] private Text countText;
        [SerializeField] private Text emptyText;

        public event Action<SingleBoosterConfig, bool> Select = delegate { };

        private SingleBoosterConfig config;
        private int highlightSortingOrder;
        private bool canBuy;

        public SingleBoosterConfig Config => config;
    
        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
        }

        public void Init(SingleBoosterConfig config, int highlightSortingOrder, bool canBuy)
        {
            this.config = config;
            this.highlightSortingOrder = highlightSortingOrder;
            this.canBuy = canBuy;
            icon.sprite = config.Icon;
        
            UpdateButton();
        }

        public void SetRaycast(bool value)
        {
            button.interactable = value;
        }

        private void Highlight()
        {
            gameObject.AddComponent<Canvas>();
            gameObject.GetComponent<Canvas>().overrideSorting = true;

            gameObject.GetComponent<Canvas>().sortingOrder = highlightSortingOrder;
        }

        public void Deselect()
        {
            Destroy(gameObject.GetComponent<Canvas>());
            UpdateButton();
        }

        private void UpdateButton()
        {

        }

        void OnClick()
        {
            
        }

        private void OnButtonUpdate(SingleBoosterConfig config)
        {
            if(this.config != config) return;
        
            UpdateButton();
        }
    
        private void OnButtonUpdate(SingleBoosterConfig arg1, bool value)
        {
            UpdateButton();
        }
    }
}
