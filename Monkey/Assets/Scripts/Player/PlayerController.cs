using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

public class PlayerController : MonoBehaviour
{
    private bool isGuarding = false;
    private bool isJustGuard = false;
    private float movementVelocity = 2;
    private float duration = 0.3f;
    private float targetPosition;

    void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Subscribe(x =>
            {
                if (isJustGuard)
                {
                    Debug.Log("Just Guard");
                }
                if (isGuarding)
                {
                    Debug.Log("Guard");
                }

                if (!isGuarding || !isJustGuard)
                {
                    Debug.Log("Dead");
                }
                
                Destroy(x.gameObject);
            });
    }

    void Update()
    {
        var tempVec = transform.position;
        var tempPosition = transform.position.x;

        if (Input.GetKey(KeyCode.Space))
        {
            isGuarding = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            targetPosition = tempPosition + movementVelocity;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            targetPosition = tempPosition - movementVelocity;
        }

        tempVec.x = Mathf.Lerp(tempPosition, targetPosition, duration);
        transform.position = tempVec;
    }
}
