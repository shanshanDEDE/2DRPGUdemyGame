using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        player.setVelocity(0f, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected() && player.IsWallDetected())                             //防止角色在空中貼牆時可以衝刺
        {
            stateMachine.ChangeState(player.wallSlide);
        }

        player.setVelocity(player.dashDir * player.dashSpeed, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);

    }
}
