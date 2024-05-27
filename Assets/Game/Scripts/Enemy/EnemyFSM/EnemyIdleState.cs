using Spine.Unity;

public class EnemyIdleState : IFSMState
{
    private Enemy curEnemy;

    public EnemyIdleState(Enemy enemy)
    {
        curEnemy = enemy;
    }
    
    public void Enter()
    {
        curEnemy.skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
