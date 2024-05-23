using UnityEngine;

namespace Game.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = nameof(SoCenter), menuName = "SO/" + nameof(SoCenter))]
    public class SoCenter : UnityEngine.ScriptableObject
    {
        private static SoCenter instance;

        private SoCenter() { }

        public static SoCenter Instance
        {
            get
            {
                if(instance == null)
                    instance = (SoCenter)CreateInstance(nameof(SoCenter));
                return instance;
            }
        }

        public PrefabSoCenter prefabSoCenter;
    }
}
