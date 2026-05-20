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
            EnemyController enemyControl = collision.gameObject.GetComponent<EnemyController>();
            if (enemyControl != null)
            {
                enemyControl.BeHurt(10); //假设子弹造成10点伤害
            }

            //计算击退力的方向和大小
            Vector3 knockDir=transform.forward; //击退方向与子弹飞行方向相同
            float knockStrength=15f; //击退力的大小，可以根据需要调整
            Vector3 knockForce=knockDir* knockStrength; //最终的击退力向量

            var enemy=collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ApplyKnockback(knockForce, 0.4f); //假设击退持续0.4秒
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
    