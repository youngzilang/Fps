using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderWalkState : EnemyState
{
    protected Spider spider;
    public SpiderWalkState(Enemy _enemy, EnemyStateMachine _stateMachine, string _aniName,Spider _spider) : base(_enemy, _stateMachine, _aniName)
    {
        this.spider = _spider;
    }

    override public void Enter()
    {
        base.Enter();
        if(spider.agent&&spider.agent.isOnNavMesh)
        {
           spider.agent.isStopped = false;
            spider.agent.speed = spider.chaseSpeed;
        }
    }

    public override void Update()
    {
        //如果玩家进入攻击范围，切换到攻击状态
        if (spider.IsPlayerInAttackRange())
        {
            spider.StopMoving();
            stateMachine.ChangeState(spider.attackState);
            return;
        }

        //如果玩家在检测范围内，继续追逐
        if (spider.IsPlayerInDetectionRange()&&spider.target)
        {
            spider.SetDestination(spider.target.position, spider.chaseSpeed);
            return;
        }

        //如果玩家不在检测范围内，切换回闲置状态
        spider.StopMoving();
        stateMachine.ChangeState(spider.idleState);

    }

    override public void Exit()
    {
        base.Exit();
        spider.StopMoving();
    }
}
