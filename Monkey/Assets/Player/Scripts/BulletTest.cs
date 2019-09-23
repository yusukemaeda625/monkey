using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
    void Start()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.sleepThreshold = -1;
    }

    private void Update()
    {
        transform.Translate(-0.2f, 0, 0);
    }
}
