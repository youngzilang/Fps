using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;

    //子弹击中目标的特效预制体
    public GameObject hitEffectPrefab;

    //子弹飞行速度
    public float speed ;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //给子弹一个初始力，使其向前飞行
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);

        Destroy(gameObject, 1f); //1秒后销毁子弹，防止无限存在占用资源
    }

    private void OnCollisionEnter(Collision collision)
    {
        //实例化子弹击中目标的特效,让特效朝向碰撞点的法线方向
        GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.LookRotation(collision.contacts[0].normal));

        if(collision.gameObject.CompareTag("Enemy"))
        {
            //如果碰撞到敌人，调用敌人的受伤方法
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.BeHurt(10); //假设子弹造成10点伤害
            }
        }

        Destroy(gameObject); //销毁子弹
        Destroy(hitEffect,1f);//销毁特效
    }
}
    