using System.Collections.Generic;
using BlockEliminateGame;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [field: SerializeField] public GameModel.BarrierType BarrierType { get; set; }
    [SerializeField] private int subLevel;
    [SerializeField] private List<GameObject> levelObjs;

    private int curLevel = 1;

    private void OnEnable()
    {
        RefreshStatus();
    }

    public bool BeAttack()
    {
        curLevel++;
        RefreshStatus();

        if (curLevel > subLevel)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    private void RefreshStatus()
    {
        for (var i = 0; i < levelObjs.Count; i++)
        {
            levelObjs[i].SetActive(i + 1 == curLevel);
        }
    }
}
