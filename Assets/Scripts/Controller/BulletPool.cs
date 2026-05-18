using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    public GameObject bulletPrefab;
    public int initialSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 预创建
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(bulletPrefab, transform);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject go;
        if (pool.Count > 0)
        {
            go = pool.Dequeue();
        }
        else
        {
            // 池耗尽时动态扩展
            go = Instantiate(bulletPrefab, transform);
        }

        go.transform.SetParent(null);
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.SetActive(true);
        return go;
    }

    public void ReturnToPool(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(transform);
        pool.Enqueue(go);
    }
}