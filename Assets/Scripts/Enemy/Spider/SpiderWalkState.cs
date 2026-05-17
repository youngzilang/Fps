using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWalkState : EnemyState
{
    public SpiderWalkState(Enemy _enemy, EnemyStateMachine _stateMachine, string _aniName) : base(_enemy, _stateMachine, _aniName)
    {
    }
}
