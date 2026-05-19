using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public EnemyStateMachine stateMachine;

    public NavMeshAgent agent;

    [Header("检测与追击")]
    public float detectionRange ;
    public float attackRange ;
    public float chaseSpeed ;

    public Transform target;

    protected virtual void Awake()
    {
         animator = GetComponent<Animator>();
         stateMachine = new EnemyStateMachine(); 

         agent = GetComponent<NavMeshAgent>();

        // 允许 NavMeshAgent 控制旋转/位置
        agent.updatePosition = true;
         agent.updateRotation = true;
    }

    protected virtual void Start()
    {
        var player= GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;
    }

    protected virtual void Update()
    {
        stateMachine.currentState.Update();
    }

    // 设置目的地并调整速度
    public void SetDestination(Vector3 destination,float speed)
    {
       if(agent&& agent.isOnNavMesh)
       {
           agent.isStopped = false;
              agent.speed = speed;
            //agent.stoppingDistance = attackRange; // 设置停止距离为攻击范围
            agent.SetDestination(destination);
        }
    }

    // 停止移动并重置路径
    public void StopMoving()
    {
        if(agent&& agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    // 检测玩家是否在范围内
    public bool IsPlayerInDetectionRange()
    {
        if (target == null) return false;
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= detectionRange;
    }

    // 检测玩家是否在攻击范围内
    public bool IsPlayerInAttackRange()
    {
        if (target == null) return false;
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= attackRange;
    }

     private void OnDrawGizmosSelected()
    {
        // 绘制检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        // 绘制攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
