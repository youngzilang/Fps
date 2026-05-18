using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject fireEffect;
    public GameObject hitEffect;

    public int count;

    private void Start()
    {
        BulletPool.Instance.preWarm(bulletPrefab, count);
        EffectPool.Instance.preWarm(fireEffect, count);
        EffectPool.Instance.preWarm(hitEffect, count);
    }
}
