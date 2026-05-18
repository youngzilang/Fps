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
            // ³ØºÄ¾¡Ê±¶¯Ì¬À©Õ¹
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

    public void preWarm(GameObject prefab, int count)
    {
        if(prefab == null || count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }
}