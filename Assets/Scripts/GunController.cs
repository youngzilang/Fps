using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab; //子弹预制体
    public Transform firePoint; //枪口位置
    public GameObject firePrefab;//开火特效预制体

    private PlayerController player;
    private ForceController forceController;

    private float timer; //计时器
    public float fireRate ; //开火速率

    private void Start()
    {
        player = GetComponent<PlayerController>();
        forceController = GetComponent<ForceController>();
    }

    private void Update()
    {
        timer+= Time.deltaTime;
        if(Input.GetMouseButton(0) && timer >= fireRate)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); //实例化子弹

            forceController.Fire(); //增加后坐力

            Destroy(Instantiate(firePrefab, firePoint.position, firePoint.rotation),.1f); //0.1秒后销毁开火特效
            timer = 0f; //重置计时器
        }
    }
}
