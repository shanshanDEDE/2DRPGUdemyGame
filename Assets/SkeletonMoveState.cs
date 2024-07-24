using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMoveState : EnemyState
{
    private Enemy_Skeleton enemy;

    //就我目前的理解 _enemyBase是基本Enemy,_enemy則是針對目前自己本身的敵人是什麼 07/24
    public SkeletonMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemy, _stateMachine, _animBoolName)   
    {
        enemy = _enemy;
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

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, enemy.rb.velocity.y);

        if(enemy.IsWallDetected() || !enemy.IsGroundDetected()){
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
    }   
}
