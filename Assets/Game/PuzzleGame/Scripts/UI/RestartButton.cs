
using PuzzleGame.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PuzzleGame.UI
{
    [RequireComponent(typeof(Button))]
    public class RestartButton : MonoBehaviour
    {
        public float interval = 30f;

        static float lastAdsTime;
    
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            UserProgress.Current.GetGameState<GameStateBaseModel>(UserProgress.Current.CurrentGameId).Reset();
            UserProgress.Current.SaveGameState(UserProgress.Current.CurrentGameId);
            UserProgress.Current.Save();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
            if (UserProgress.Current.IsItemPurchased("no_ads"))
                return;
        }
    }
}