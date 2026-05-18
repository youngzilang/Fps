using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;
    private BulletPool bulletPool;
    private EffectPool effectPool;

    //子弹击中目标的特效预制体
    public GameObject hitEffectPrefab;

    //子弹飞行速度
    public float speed ;


    // Start is called before the first frame update
    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        bulletPool=BulletPool.Instance;
        effectPool=EffectPool.Instance;
        //给子弹一个初始力，使其向前飞行
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);

       // Destroy(gameObject, 1f); //1秒后销毁子弹，防止无限存在占用资源

        Invoke(nameof(ReturnBullet), 1f); //1秒后把子弹返回对象池
    }

    private void OnCollisionEnter(Collision collision)
    {
        //从对象池中获取子弹击中目标的特效,让特效朝向碰撞点的法线方向
        GameObject hitEffect = effectPool.Spawn(hitEffectPrefab, transform.position, Quaternion.LookRotation(collision.contacts[0].normal));

        if(collision.gameObject.CompareTag("Enemy"))
        {
            //如果碰撞到敌人，调用敌人的受伤方法
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.BeHurt(10); //假设子弹造成10点伤害
            }
        }

        // Destroy(gameObject); //销毁子弹

        //把子弹返回对象池而不是销毁
        ReturnBullet();
    }

    private void OnDisable()
    {
        if (rb)
        {
            rb.velocity = Vector3.zero; //重置速度，防止下次启用时受到之前的速度影响
            rb.angularVelocity = Vector3.zero; //重置旋转速度
        }
    }

    private void ReturnBullet()
    {
        CancelInvoke(); // 防止重复调用
        bulletPool.ReturnToPool(gameObject);
    }
}
    