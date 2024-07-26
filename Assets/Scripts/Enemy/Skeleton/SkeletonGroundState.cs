using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundState : EnemyState
{

    protected Enemy_Skeleton enemy;
    protected Transform player;

    //就我目前的理解 _enemyBase是基本Enemy,_enemy則是針對目前自己本身的敵人是什麼 07/24
    public SkeletonGroundState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        //player = GameObject.Find("Player").transform;
        player = PlayerManager.instance.player.transform;
        //透過PlayerManager並使用單例模式來尋找會比起GameObject.Find 有效率更多,因為GameObject.Find會搜尋全部物件來找到目標
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.position) < 2)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
