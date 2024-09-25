using System.Collections.Generic;
using GameFrame;
using UnityEngine;

public class StageLoadPresenter : MonoSingleton<StageLoadPresenter>
{
    public List<Ghost> activityGhosts = new ();
    
    public void LoadStagePrefab(int stageLevel)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        var stageConfig = StageConfigManager.Instance.GetStageConfig(stageLevel);
        if (stageConfig != null)
        {
            Instantiate(stageConfig.StagePrefab, transform);
            activityGhosts.Clear();
        }
    }

    public void AddActivityGhost(Ghost ghost)
    {
        activityGhosts.Add(ghost);
    }

    public void OnGameEnd()
    {
        foreach (var ghost in activityGhosts)
        {
            ghost.Stop();
        }
        activityGhosts.Clear();
    }

    public void OnRevive()
    {
        foreach (var ghost in activityGhosts)
        {
            ghost.ResetState();
        }
        activityGhosts.Clear();
    }
}
