using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int hp;//敌人生命值
    public GameObject deadEffect;//死亡特效

    public void BeHurt(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deadEffect != null)
        {
            GameObject effect = Instantiate(deadEffect, transform.position, transform.rotation);
            Destroy(effect, 1f); //1秒后销毁死亡特效对象，确保特效播放完毕
        }
        AudioManager.instance.PlaySFX(3); //播放敌人死亡音效
        //销毁敌人对象
        Destroy(gameObject);
        
    }   
}
