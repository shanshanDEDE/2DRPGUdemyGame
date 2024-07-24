using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        stateMachine = new EnemyStateMachine();
    }
    
    private void Update()
    {
        stateMachine.currentState.Update();
    }
}
