using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameFrame
{
    public class AssetManager : Singleton<AssetManager>
    {
        public async UniTask<T> LoadAssetAsync<T>(string name) where T : Component
        {
            var obj = await Addressables.InstantiateAsync(name);
            return obj.TryGetComponent<T>(out var temp) ? temp : null;
        }

        public T LoadAsset<T>(string name) where T : Component
        {
            var obj = Addressables.InstantiateAsync(name).WaitForCompletion();
            return obj.TryGetComponent<T>(out var temp) ? temp : null;
        }
    }
}
