using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactoryManager : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab = null;
    private float timer = 0;
    private float interval = 1.5f;
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            timer = 0;
        }
    }
}
