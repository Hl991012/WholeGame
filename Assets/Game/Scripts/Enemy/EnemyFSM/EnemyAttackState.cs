public class EnemyAttackState : IFSMState
{
    private Enemy curEnemy;

    public bool AttackFinished { get; private set; }

    public EnemyAttackState(Enemy enemy)
    {
        curEnemy = enemy;
    }
    
    public void Enter()
    {
        AttackFinished = false;
        curEnemy.skeletonAnimation.AnimationState.SetAnimation(0, "Attack", false).Complete += _ =>
        {
            AttackFinished = true;
        };
        curEnemy.Attack();
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
