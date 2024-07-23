using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("攻擊細節")]
    public Vector2[] attackMovement;                          //每段主要攻擊的位移大小

    public bool isBusy { get; private set; }                //用來放在協成中，確認是否正在動作,如果再動作可以用來防止移動

    [Header("移動資料")]
    public float moveSpeed = 12f;
    public float jumpForce;

    [Header("衝刺資料")]
    [SerializeField] private float dashCoolDown;
    private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

    [Header("碰撞資料")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    public int facingDir { get; private set; } = 1;
    public bool facingRight = true;

    #region Compoments
    public Animator anim { get; private set; }

    public Rigidbody2D rb { get; private set; }
    #endregion

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

    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlidState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine.Initialize(moveState);
    }

    private void Update()
    {
        stateMachine.currentState.Update();

        CheckForDashInput();
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

        dashUsageTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q) && dashUsageTimer < 0)
        {
            dashUsageTimer = dashCoolDown;
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
            {
                dashDir = facingDir;
            }

            stateMachine.ChangeState(dashState);
        }
    }
    #region Velocity
    public void ZeroVelocity() => rb.velocity = new Vector2(0, 0);

    public void setVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

    }
    #endregion 

    #region Flip
    public void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }
    #endregion
}
