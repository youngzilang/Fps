using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简单的特效对象池，按预制体分队列复用，自动在粒子播放完毕后回收。
/// </summary>
public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance { get; private set; }

    // 为每个特效预制体维护一个队列
    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    // 存放回收对象的父对象
    private Transform poolParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        poolParent = transform;
    }

    /// <summary>
    /// 从池中获取一个指定预制体的实例（若池空则新建），并在指定位置激活播放，随后会自动回收。
    /// lifeOverride: 如果大于 0，则使用该值作为回收时间，覆盖对粒子系统的自动计算。
    /// </summary>
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float lifeOverride = -1f)
    {
        if (prefab == null) return null;

        if (!pools.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            pools[prefab] = queue;
        }

        GameObject go;
        if (queue.Count > 0)
        {
            go = queue.Dequeue();
            go.transform.SetParent(null);
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(true);
        }
        else
        {
            go = Instantiate(prefab, position, rotation);
            // 标记该实例对应的原始预制体，便于回收时放回正确队列
            var member = go.GetComponent<PooledEffect>();
            if (member == null) member = go.AddComponent<PooledEffect>();
            member.prefab = prefab;
        }

        // 播放粒子（若有）
        PlayParticleSystems(go);

        // 计算大致生命周期并自动回收（允许外部覆盖）
        float life = lifeOverride > 0f ? lifeOverride : CalculateLifeTime(go);
        StartCoroutine(AutoReturnAfter(go, life));

        return go;
    }

    private IEnumerator AutoReturnAfter(GameObject go, float seconds)
    {
        if (seconds > 0f)
            yield return new WaitForSeconds(seconds);
        else
            yield return new WaitForSeconds(1f);

        ReturnToPool(go);
    }

    /// <summary>
    /// 将实例回收到对应的预制体队列
    /// </summary>
    public void ReturnToPool(GameObject go)
    {
        if (go == null) return;

        var member = go.GetComponent<PooledEffect>();
        if (member == null || member.prefab == null)
        {
            // 如果没有标记原始预制体，直接销毁以避免泄露
            Destroy(go);
            return;
        }

        var prefab = member.prefab;

        if (!pools.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            pools[prefab] = queue;
        }

        // 停止所有粒子并复位状态
        StopParticleSystems(go);

        go.SetActive(false);
        go.transform.SetParent(poolParent);
        queue.Enqueue(go);
    }

    private void PlayParticleSystems(GameObject go)
    {
        var systems = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }
    }

    private void StopParticleSystems(GameObject go)
    {
        var systems = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            ps.Clear(true);
        }
    }

    /// <summary>
    /// 根据包含的粒子系统估算一个回收时间（取所有系统的最大值），如果没有粒子系统则返回 1 秒。
    /// </summary>
    private float CalculateLifeTime(GameObject go)
    {
        float maxLife = 0f;
        var systems = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            var main = ps.main;
            float dur = main.duration;

            // 处理 startLifetime 的可能模式，取最大可能值
            float startLifetime = 0f;
            try
            {
                startLifetime = main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants
                    ? main.startLifetime.constantMax
                    : main.startLifetime.constant;
            }
            catch
            {
                // 保险回退
                startLifetime = 0.5f;
            }

            float candidate = dur + startLifetime;
            if (candidate > maxLife) maxLife = candidate;
        }

        if (maxLife <= 0f) maxLife = 1f;
        return maxLife;
    }

    public void preWarm(GameObject prefab, int count)
    {
        if (prefab == null || count <= 0) return;
        if (!pools.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            pools[prefab] = queue;
        }
        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(prefab, poolParent);
            go.SetActive(false);
            var member = go.GetComponent<PooledEffect>();
            if (member == null) member = go.AddComponent<PooledEffect>();
            member.prefab = prefab;
            queue.Enqueue(go);
        }
    }

    // 简单组件：标记实例对应的原始预制体
    private class PooledEffect : MonoBehaviour
    {
        public GameObject prefab;
    }
}