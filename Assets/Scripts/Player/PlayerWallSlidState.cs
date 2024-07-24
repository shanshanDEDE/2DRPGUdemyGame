using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlidState : PlayerState
{
    public PlayerWallSlidState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJump);
            return;
        }

        if (xInput != 0 && player.facingDir != xInput)
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (yInput < 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y * .7f);

        //我自己加的!player.IsGroundDetected() && !player.IsWallDetected()判斷式，用來防止角色在空中貼牆後沒牆壁時還會保持貼牆下滑動作
        if (!player.IsGroundDetected() && !player.IsWallDetected())
        {
            stateMachine.ChangeState(player.airState);
        }
        else
        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);
    }
}
