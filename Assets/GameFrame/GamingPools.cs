using System.Collections.Generic;
using Game.Scripts.ScriptableObject;
using UnityEngine;

namespace GameFrame
{
    public class GamingPools : MonoSingleton<GamingPools>
    {
        private readonly Dictionary<RecyclableType, List<GameObject>> pools = new();
        
        public T GetFromPool<T>(RecyclableType recyclableType) where T : Component, IRecyclable
        {
            if (pools.TryGetValue(recyclableType, out var subPool))
            {
                if (subPool.Count > 0)
                {
                    var temp = subPool[0].GetComponent<T>();
                    subPool.RemoveAt(0);
                    temp.OnGet();
                    return temp;
                }
            }

            var obj = Instantiate(SoCenter.Instance.prefabSoCenter.GetPrefabByRecyclableType(recyclableType), transform);
            var tempT = obj.AddComponent<T>();
            return tempT;
        }

        public void ReturnToPool<T>(GameObject obj) where T : Component, IRecyclable
        {
            var temp = obj.GetComponent<T>();
            if (pools.TryGetValue(temp.RecyclableType, out var pool))
            {
                pool.Add(obj);
            }
            else
            {
                pools[temp.RecyclableType] = new List<GameObject>()
                {
                    obj,
                };
            }
            temp.OnReturn();
        }
    }
}
