using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    [SerializeField] private float returnSpeed = 12;
    private Animator animator;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;

    [Header("Bounce info")]
    [SerializeField] private float bounceSpeed;
    private bool isBouncing;                        //判斷是否為彈跳效果的劍
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player)
    {
        player = _player;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        if (pierceAmount <= 0)
            animator.SetBool("Rotation", true);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounce)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;

        //由於private List<Transform> enemyTarget;設為private的話unity會因為private而不會創建,因此這邊需要先實力化
        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
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

        collision.GetComponent<Enemy>()?.Damage();

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

        StuckInto(collision);                                               //碰撞後的處理
    }

    //碰撞後的處理
    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)        //如果碰撞的是敵人並且還有穿透力
        {
            pierceAmount--;
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
