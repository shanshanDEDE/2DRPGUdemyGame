using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("攻擊細節")]
    public Vector2[] attackMovement;                          //每段主要攻擊的位移大小
    public float counterAttackDuration = 0.2f;

    public bool isBusy { get; private set; }                //用來放在協成中，確認是否正在動作,如果再動作可以用來防止移動

    [Header("移動資料")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;

    [Header("衝刺資料")]
    //[SerializeField] private float dashCoolDown;
    //private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }


    public SkillerManager skill { get; private set; }
    public GameObject sword { get; private set; }


    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlidState wallSlide { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }


    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }

    public PlayerAimSwordState aimSword { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlidState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillerManager.instance;

        stateMachine.Initialize(moveState);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        CheckForDashInput();
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    //這個協成可以用來防止玩家一直在攻擊的期間,中間的空窗期會讓idelstate中的移動問題
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        if (IsWallDetected())    //防止貼牆滑行狀態時可以衝刺
            return;

        //改成用SkillManager的方法來做冷卻及是否攻擊的判斷
        //dashUsageTimer -= Time.deltaTime;
        //if (Input.GetKeyDown(KeyCode.Q) && dashUsageTimer < 0)
        if (Input.GetKeyDown(KeyCode.Q) && SkillerManager.instance.dash.CanUseSkill())
        {
            //dashUsageTimer = dashCoolDown;
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
            {
                dashDir = facingDir;
            }

            stateMachine.ChangeState(dashState);
        }
    }

}
