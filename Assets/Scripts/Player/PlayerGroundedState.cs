using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword()) //這邊用HasNoSword()方法來做判斷
            stateMachine.ChangeState(player.aimSword);

        if (Input.GetKeyDown(KeyCode.E))
            stateMachine.ChangeState(player.counterAttack);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);


        //老師很聰明 加上這個判斷式後因為Dash完會回到idle的狀態機上,而idle的狀態機idle的狀態機繼承自PlayerGroundedState
        //，所以這裡要加上判斷是否接觸地面,如果沒有則切到PlayerAirState狀態機
        if (!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }

    }

    private bool HasNoSword()
    {

        if (!player.sword)
            return true;

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();

        return false;
    }
}
