using GameFrame;
using Spine.Unity;
using UnityEngine;

public class Enemy : MonoBehaviour, IRecyclable
{
    [SerializeField] public SkeletonAnimation skeletonAnimation;

    private FSMStateMachine fsmStateMachine;

    public EnemyModel EnemyModel { get; private set; }

    private float nextAttackTime;

    private void Awake()
    {
        var enemyIdleState = new EnemyIdleState(this);
        var enemyWalkState = new EnemyWalkState(this);
        var enemyAttackState = new EnemyAttackState(this);
        var enemyDeathState = new EnemyDeathState(this);
        fsmStateMachine = new FSMStateMachine();
        fsmStateMachine.AddSingleTransition(enemyDeathState, () => EnemyModel?.Hp <= 0);
        fsmStateMachine.AddStateTransition(enemyWalkState, enemyIdleState, () => InAttackRange);
        fsmStateMachine.AddStateTransition(enemyIdleState, enemyAttackState, () => Time.time > nextAttackTime && InAttackRange);
        fsmStateMachine.AddStateTransition(enemyAttackState, enemyIdleState, () => enemyAttackState.AttackFinished && InAttackRange);
        fsmStateMachine.ChangeStage(enemyWalkState);
    }

    public void Init(EnemyModel enemyModel)
    {
        EnemyModel = enemyModel;
    }
    
    private void Update()
    {
        fsmStateMachine?.Update();
    }

    private bool InAttackRange => Mathf.Abs(-5 - transform.position.y) <= EnemyModel.AttackRange;

    #region 行为

    public void Move()
    {
        if (EnemyModel != null)
        {
            transform.Translate(Vector3.down * (Time.deltaTime * EnemyModel.MoveSpeed));
        }
    }

    public void Attack()
    {
        nextAttackTime = Time.time + EnemyModel.AttackInterval;
    }
    
    public void BeAttacked(int damage)
    {
        
    }

    #endregion
    
    public RecyclableType RecyclableType => RecyclableType.Enemy;
    
    public void OnGet()
    {
        gameObject.SetActive(true);
    }

    public void OnReturn()
    {
        gameObject.SetActive(false);
    }
}
