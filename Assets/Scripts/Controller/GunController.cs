using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab; //子弹预制体
    public Transform firePoint; //枪口位置
    public GameObject firePrefab;//开火特效预制体

    private BulletPool bulletPool;//子弹对象池
    private EffectPool effectPool;//特效对象池

    private PlayerController player;
    private ForceController forceController;

    private float timer; //计时器
    public float fireRate; //开火速率

    private void Start()
    {
        player = GetComponent<PlayerController>();
        forceController = GetComponent<ForceController>();
        bulletPool=BulletPool.Instance; //获取子弹对象池实例
        effectPool=EffectPool.Instance; //获取特效对象池实例
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButton(0) && timer >= fireRate && !player.isRunning)
        {
            bulletPool.Spawn(firePoint.position, firePoint.rotation); //从对象池中获取子弹实例

            //Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); //实例化子弹

            forceController.Fire(); //增加后坐力

            effectPool.Spawn(firePrefab, firePoint.position, firePoint.rotation,.1f); //从对象池中获取开火特效实例

            AudioManager.instance.PlaySFX(0); //播放开火音效

            timer = 0f; //重置计时器
        }
    }
}
