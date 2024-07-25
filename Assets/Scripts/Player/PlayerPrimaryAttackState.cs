using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter;

    private float lastTimeAttacked;
    private float comboWindow = 2;

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();
        xInput = 0;       //用這個來修復攻擊方向不正確的bug

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }

        player.anim.SetInteger("ComboCounter", comboCounter);

        //player.anim.speed = 1.5f;                                   //改變動畫速度(可以用來改變武器攻擊速度)

        /*
            if (xInput != 0 && player.facingDir != xInput)              //我自己加的,目的是連續攻擊期間可以透過方向鍵去改便攻擊方向
           {
               player.Flip();
           }
         player.setVelocity(player.attackMovement[comboCounter].x * player.facingDir, player.attackMovement[comboCounter].y); */


        //下面這幾行是用來連續攻擊中可以改變攻擊方向
        float attackDir = player.facingDir;
        if (xInput != 0)
        {
            attackDir = xInput;
        }

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;                                        //設定狀態機共用的計時器(這邊用來設定攻擊後多久才停止移動,來製作慣性的效果)
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);                   //設置主要攻擊的忙碌時間

        //player.anim.speed = 1f;                                    //離開時記得將動畫速度改回來

        comboCounter++;
        lastTimeAttacked = Time.time;

    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)                                       //當計時器<0時，才停止玩家移動,達到移動攻擊有慣性移動一下的效果
        {
            player.SetZeroVelocity();
        }

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

    }
}
