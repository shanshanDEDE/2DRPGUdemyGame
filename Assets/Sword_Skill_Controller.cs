using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();

    }

    public void SetupSword(Vector2 _dir, float _gravityScale)
    {
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
    }
}