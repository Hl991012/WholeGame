using UnityEngine;

namespace PuzzleGame.Gameplay.Boosters
{
    [CreateAssetMenu(fileName = "BoosterPreset", menuName = "Booster Preset")]
    public class BoosterPreset : ScriptableObject
    {
        public BoosterType Type;
        public Sprite Icon;
        public string Description;
        public int CountToBuy = 1;
    }
}
