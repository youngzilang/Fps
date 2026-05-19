using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAttackState : EnemyState
{
    protected Spider spider;

    private float attackTimer;
    private float attackCD=1f;

    public SpiderAttackState(Enemy _enemy, EnemyStateMachine _stateMachine, string _aniName,Spider _spider) : base(_enemy, _stateMachine, _aniName)
    {
        this.spider = _spider;
    }

    override public void Enter()
    {
        base.Enter();
        //在攻击状态进入时，停止移动
        spider.StopMoving();
        attackTimer=attackCD; //重置攻击计时器
    }

    public override void Update()
    {
        base.Update();
        //如果玩家不在攻击范围内，切换回追逐状态
        if (!spider.IsPlayerInAttackRange())
        {
            stateMachine.ChangeState(spider.walkState);
            return;
        }
        //如果玩家在攻击范围内，继续攻击
        attackTimer+=Time.deltaTime;
        if(attackTimer>=attackCD)
        {
            attackTimer=0f; //重置攻击计时器
            ReAttack();//再次攻击
        }
    }

    private void ReAttack()
    {
        
    }

    public override void Exit() 
    { 
        base.Exit(); 
    }

}
