using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public EnemyStateMachine stateMachine;

    protected virtual void Awake()
    {
         animator = GetComponent<Animator>();
         stateMachine = new EnemyStateMachine(); 
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        stateMachine.currentState.Update();
    }
}
