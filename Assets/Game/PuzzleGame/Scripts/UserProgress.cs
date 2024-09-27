using System;
using System.Collections.Generic;
using PuzzleGame.Gameplay;
using UnityEngine;

namespace PuzzleGame
{
    [Serializable]
    public class UserProgress
    {
        [Serializable]
        struct PurchaseProgress
        {
            public string item;
            public int value;

            public PurchaseProgress(string item, int value)
            {
                this.item = item;
                this.value = value;
            }
        }

        static UserProgress current;

        public event Action ProgressUpdate = delegate { };

        Dictionary<string, GameStateModel> gameStates = new();

        [SerializeField] private int coins;

        [SerializeField] private List<string> purchasedItems = new();

        [SerializeField] private List<PurchaseProgress> purchaseInProgress = new();

        [SerializeField] private string currentGameId;

        public static UserProgress Current
        {
            get
            {
                if (current != null)
                    return current;

                var progressJson = PlayerPrefs.GetString("UserProgress", "{}");
                Debug.Log("UserProgress : " + progressJson);
                current = JsonUtility.FromJson<UserProgress>(progressJson);

                return current;
            }
        }

        public int Coins
        {
            get => coins;
            set
            {
                coins = value;

                Save();

                ProgressUpdate.Invoke();
            }
        }

        public string CurrentGameId
        {
            get => currentGameId;
            set
            {
                currentGameId = value;

                Save();

                ProgressUpdate.Invoke();
            }
        }

        public string CurrentThemeId
        {
            get => GetGameState<GameStateModel>(currentGameId)?.ThemeId;
            set
            {
                GameStateModel gameStateModel = GetGameState<GameStateModel>(currentGameId);
                gameStateModel.ThemeId = value;
                SetGameState(currentGameId, gameStateModel);
                SaveGameState(currentGameId);
                Save();

                ProgressUpdate.Invoke();
            }
        }

        public bool IsItemPurchased(string item)
        {
            return purchasedItems.Contains(item);
        }

        public int GetItemsPurchasedCount(string item)
        {
            return purchasedItems.FindAll((i) => i.Equals(item)).Count;
        }

        public void RemoveItemPurchase(string item)
        {
            if (purchasedItems.Contains(item))
                purchasedItems.Remove(item);
        }
    
        public void RemoveAllItemPurchase(string item)
        {
            if (purchasedItems.Contains(item))
                purchasedItems.RemoveAll(i => i.Equals(item));
        }

        public void OnItemPurchased(string item)
        {
            purchasedItems.Add(item);

            Save();

            ProgressUpdate.Invoke();
        }

        public int GetItemPurchaseProgress(string item)
        {
            PurchaseProgress purchaseProgress = purchaseInProgress.Find(p => p.item == item);
            return purchaseProgress.value;
        }

        public void SetItemPurchaseProgress(string item, int value)
        {
            purchaseInProgress.RemoveAll(p => p.item == item);
            purchaseInProgress.Add(new PurchaseProgress(item, value));

            Save();

            ProgressUpdate.Invoke();
        }

        public T GetGameState<T>(string id) where T : GameStateModel
        {
            if (string.IsNullOrEmpty(id))
                return null;

            if (gameStates.ContainsKey(id) && gameStates[id] is T)
                return (T) gameStates[id];

            if (!PlayerPrefs.HasKey(id))
                return null;

            if (gameStates.ContainsKey(id))
                gameStates.Remove(id);

            GameStateModel gameStateModel = JsonUtility.FromJson<T>(PlayerPrefs.GetString(id));
            gameStates.Add(id, gameStateModel);

            return (T) gameStateModel;
        }

        public void SetGameState<T>(string id, T state) where T : GameStateModel
        {
            gameStates[id] = state;
        }

        public void SaveGameState(string id)
        {
            if (gameStates.TryGetValue(id, out var state))
                PlayerPrefs.SetString(id, JsonUtility.ToJson(state));
        }

        public void Save()
        {
            string progressJson = JsonUtility.ToJson(this, true);
            PlayerPrefs.SetString("UserProgress", progressJson);
        }
    }
}