using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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

    private Rigidbody rb;
    private Coroutine knockbackCoroutine;

    private Spider spider;

    protected virtual void Awake()
    {
         animator = GetComponent<Animator>();
         stateMachine = new EnemyStateMachine(); 
         spider = GetComponent<Spider>();

        agent = GetComponent<NavMeshAgent>();

        // 允许 NavMeshAgent 控制旋转/位置
        agent.updatePosition = true;
         agent.updateRotation = true;

        rb = GetComponent<Rigidbody>();

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
       if(agent&& agent.isOnNavMesh&&agent.enabled)
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

    public void ApplyKnockback(Vector3 force, float duration = 0.4f)
    {
        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(KnockbackRoutine(force, duration));
    }

    private IEnumerator KnockbackRoutine(Vector3 force, float duration)
    {
        // 禁用 agent（停止寻路）
        bool hadAgent = (agent != null && agent.isOnNavMesh && agent.enabled);
        bool originalIsKinematic = rb.isKinematic; // 保存原来的IsKinematic状态

        if (hadAgent)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        // 切换刚体为受物理控制
        rb.isKinematic = false;

        // 清理之前速度，施加冲量
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);

        // 等待物理击退时间
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // 停止刚体运动并恢复 kinematic
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = originalIsKinematic;


        //启用agent
        agent.enabled = true;

        // 把 NavMeshAgent 同步到当前物理位置并恢复
        if (hadAgent)
        {
            // 确保 agent 能够在 NavMesh 上
            if (!agent.isOnNavMesh)
            {
                NavMeshHit hit;
                // 在小半径内寻找最近的 NavMesh 点
                NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas);
               agent.Warp(hit.position);
            }
            else agent.Warp(transform.position);// agent 在 NavMesh 上，直接同步位置

            agent.ResetPath();
            agent.isStopped = false;

        }

        // 恢复完 agent 后，确保状态机回到合适的寻路/攻击状态
        if (spider != null)
        {
            if (IsPlayerInDetectionRange())
            {
                if (IsPlayerInAttackRange())
                {
                    // 进入攻击态
                    stateMachine.ChangeState(spider.attackState);
                }
                else
                {
                    // 进入追击态
                    stateMachine.ChangeState(spider.walkState);
                }
            }
            else
            {
                // 回 idle
                stateMachine.ChangeState(spider.idleState);
            }
        }


        knockbackCoroutine = null;
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
