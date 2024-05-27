using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : IFSMState
{
    private Enemy curEnemy;

    public EnemyDeathState(Enemy enemy)
    {
        curEnemy = enemy;
    }
    
    public void Enter()
    {
        
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
