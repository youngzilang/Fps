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
            Instantiate(deadEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }   
}
