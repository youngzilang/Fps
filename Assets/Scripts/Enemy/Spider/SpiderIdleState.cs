using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderIdleState : EnemyState
{
    protected Spider spider;
    public SpiderIdleState(Enemy _enemy, EnemyStateMachine _stateMachine, string _aniName,Spider _spider) : base(_enemy, _stateMachine, _aniName)
   {
        this.spider = _spider;
    }

    override public void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        //Èç¹ûÍæ¼Ò½øÈë¼ì²â·¶Î§£¬ÇÐ»»µ½×·Öð×´Ì¬
        if (spider.IsPlayerInDetectionRange()&&spider.target)
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
