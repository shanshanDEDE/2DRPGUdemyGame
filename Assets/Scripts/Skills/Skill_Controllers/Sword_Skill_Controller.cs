using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    private float returnSpeed = 12;

    [Header("Pierce info")]
    private int pierceAmount;

    [Header("Bounce info")]
    private float bounceSpeed;
    private bool isBouncing;                        //判斷是否為彈跳效果的劍
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float springDirection;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        returnSpeed = _returnSpeed;

        if (pierceAmount <= 0)
            animator.SetBool("Rotation", true);

        springDirection = Math.Clamp(rb.velocity.x, -1f, 1f);//透過初始速度決定方向,並且防止超出範圍

        Invoke("DestroyMe", 7);             //一段時間後刪除自己
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounce, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;
        bounceSpeed = _bounceSpeed;

        //由於private List<Transform> enemyTarget;設為private的話unity會因為private而不會創建,因此這邊需要先實力化
        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    //劍返需要改回來的一些設定
    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;          //鎖定位置跟旋轉,透過動畫繞他看起來動就好
        //rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1f)
                player.CatchTheSword();
        }

        BounceLogic(); //彈跳劍的邏輯,裡面會判斷是否為彈跳劍

        SpinLogic();   //旋轉劍的邏輯,裡面會判斷是否為旋轉劍
    }

    //旋轉劍的邏輯,裡面會判斷是否為旋轉劍
    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                //旋轉到指定位置停止後 下面這行可以讓他漸漸往前(這邊老師只有做x軸前後)一點
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + springDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer <= 0)
                {
                    isSpinning = false;
                    isReturning = true;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);        //將附近的敵人放進陣列

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    //彈跳劍的邏輯,裡面會判斷是否為彈跳劍
    private void BounceLogic()
    {
        //如果劍有設定彈跳,並且附近的敵人至少有一個
        if (isBouncing && enemyTarget.Count > 0)   //設定enemyTarget.Count為0的話就算敵人只有一個也會自動把刀飛回來
        {
            //彈跳敵人
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            //判斷是否到達敵人
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());  //對敵人用飛劍技能造成傷害及效果

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;                 //讓劍飛回來
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    //用來讓劍插在物件身上
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)                                 //防止劍飛回來時會攻擊到敵人(可依遊戲性判斷要不要加)
            return;



        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);

        }

        //collision.GetComponent<Enemy>()?.Damage();

        SetUpTargetsForBounce(collision);

        StuckInto(collision);                                               //碰撞後的處理
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        enemy.Damage();
        enemy.StartCoroutine("FreezeTimeFor", freezeTimeDuration);
    }

    private void SetUpTargetsForBounce(Collider2D collision)
    {
        //如果碰撞的是敵人
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)                       //如果有設定彈跳且還沒有將附近的敵人放進陣列
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);        //將附近的敵人放進陣列

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);
                    }
                }
            }
        }
    }

    //碰撞後的處理
    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)        //如果碰撞的是敵人並且還有穿透力
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;                              //將圓形碰撞器關掉,這樣的話劍返回就不會再次觸發碰撞

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)                //設定enemyTarget.Count為0的話就算敵人只有一個也會自動把刀飛回來
            return;

        animator.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
