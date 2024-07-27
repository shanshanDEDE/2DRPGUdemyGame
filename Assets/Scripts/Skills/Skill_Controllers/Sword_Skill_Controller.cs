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

        animator.SetBool("Rotation", true);
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
    }

    //用來讓劍插在物件身上
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)                                 //防止劍飛回來時會攻擊到敵人(可依遊戲性判斷要不要加)
            return;

        animator.SetBool("Rotation", false);

        canRotate = false;
        cd.enabled = false;                              //將圓形碰撞器關掉,這樣的話劍返回就不會再次觸發碰撞

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        transform.parent = collision.transform;
    }
}
