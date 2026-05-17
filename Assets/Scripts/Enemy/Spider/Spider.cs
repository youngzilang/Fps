using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
{
    public SpiderIdleState idleState;
    public SpiderWalkState walkState;
    public SpiderAttackState attackState;

    protected override void Awake()
    {
         base.Awake();
         idleState = new SpiderIdleState(this, stateMachine, "Idle");
         walkState = new SpiderWalkState(this, stateMachine, "Walk");  
         attackState = new SpiderAttackState(this, stateMachine, "Attack");  
    }

    protected override void Start()
    {
         base.Start();
         stateMachine.Initialized(idleState);
    }
     protected override void Update()
    {
         base.Update();
    }
}
