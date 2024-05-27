public class EnemyWalkState : IFSMState
{
    private Enemy curEnemy;
    
    public EnemyWalkState(Enemy enemy)
    {
        curEnemy = enemy;
    }
    
    public void Enter()
    {
        curEnemy.skeletonAnimation.AnimationState.SetAnimation(0, "Walk", true);
    }

    public void Update()
    {
        curEnemy.Move();
    }

    public void Exit()
    {
        
    }
}
