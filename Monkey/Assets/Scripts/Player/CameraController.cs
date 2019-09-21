using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject cameraTarget = null;
    private float duration = 0.6f;
    
    void Start()
    {
        cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget");
    }

    void Update()
    {
        var tempPosition = transform.position;

        transform.position = cameraTarget.transform.position;
    }
}
