using UnityEngine;

namespace PuzzleGame.Gameplay.Boosters
{
    [CreateAssetMenu(fileName = nameof(SingleBoosterConfig), menuName = "Booster Preset")]
    public class SingleBoosterConfig : ScriptableObject
    {
        public BoosterType Type;
        public Sprite Icon;
        public string Description;
        public int CountToBuy = 1;
        public int startCount = 1;
    }
}
