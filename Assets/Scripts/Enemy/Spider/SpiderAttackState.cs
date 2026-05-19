using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAttackState : EnemyState
{
    protected Spider spider;


    public SpiderAttackState(Enemy _enemy, EnemyStateMachine _stateMachine, string _aniName,Spider _spider) : base(_enemy, _stateMachine, _aniName)
    {
        this.spider = _spider;
    }

    override public void Enter()
    {
        base.Enter();
        //在攻击状态进入时，停止移动
        spider.StopMoving();
     
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
      
    }

    public override void Exit() 
    { 
        base.Exit(); 
    }

}
