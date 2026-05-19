using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState 
{
    protected EnemyStateMachine stateMachine;
    protected Enemy enemy;
    protected string aniName;

    public EnemyState(Enemy _enemy,EnemyStateMachine _stateMachine,string _aniName)
    {
        enemy = _enemy;
        stateMachine = _stateMachine;
        aniName = _aniName;
    }

    public virtual void Enter()
    {
        enemy.animator.SetBool(aniName, true);
    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {
        enemy.animator.SetBool(aniName, false);
    }
}
