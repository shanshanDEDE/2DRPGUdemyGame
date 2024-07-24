using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (xInput != 0)                                                                //我自己加的讓角色一跳起來就可以左右移動,增加控制性
        {
            player.SetVelocity(xInput * player.moveSpeed * .8f, rb.velocity.y);
        }

        if (player.rb.velocity.y < 0f)
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
