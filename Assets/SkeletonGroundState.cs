using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundState : EnemyState
{

    protected Enemy_Skeleton enemy;

    //就我目前的理解 _enemyBase是基本Enemy,_enemy則是針對目前自己本身的敵人是什麼 07/24

    public SkeletonGroundState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
