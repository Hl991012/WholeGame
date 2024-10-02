using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.Gameplay.Make10
{
    public class NumberCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI highlightedLabel;
        [SerializeField] private Animator animator;

        private int value;
        private static readonly int On = Animator.StringToHash("On");
        
        private void Awake()
        {
            label.text = "0";
            highlightedLabel.gameObject.SetActive(false);
        }

        public void UpdateValue(int value, int maxValue)
        {
            if(this.value == value) return;

            this.value = value;
            label.text = value.ToString();
            highlightedLabel.text = value.ToString();

            animator.SetTrigger(On);

            highlightedLabel.gameObject.SetActive(value > maxValue);
            label.gameObject.SetActive(value <= maxValue);
        }
    }
}
