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
    private bool hit = false;
    private int justGuardDuration = 10;
    private int timer = 0;
    private float movementVelocity = 2;
    private float duration = 0.3f;
    private float targetPosition;
    private Animator animator = null;
    private int dash = Animator.StringToHash("dashTrigger");
    private int guard = Animator.StringToHash("guard");

    void Start()
    {
        animator = GetComponent<Animator>();

        this.OnTriggerEnterAsObservable()
            .Subscribe(x =>
            {
                hit = true;
                if (isJustGuard)
                {
                    Debug.Log("Just Guard");
                }
                if (isGuarding)
                {
                    Debug.Log("Guard");
                }

                if (!isGuarding && !isJustGuard)
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

        if (Input.GetButton("Guard"))
        {
            isGuarding = true;
            animator.SetBool(guard, true);
        }

        if (Input.GetButtonUp("Guard"))
        {
            StartCoroutine(JustGuard());
        }

        if (Input.GetButtonDown("D-Right")&& !isGuarding)
        {
            targetPosition = tempPosition - movementVelocity;
            animator.SetTrigger(dash);
        }

        if (Input.GetButtonDown("D-Left") && !isGuarding)
        {
            targetPosition = tempPosition + movementVelocity;
        }

        tempVec.x = Mathf.Lerp(tempPosition, targetPosition, duration);
        transform.position = tempVec;
    }

    IEnumerator JustGuard()
    {
        isJustGuard = true;
        for (timer = 0; timer < justGuardDuration; timer++)
        {
            if (hit)
            {
                yield return null;
                animator.SetBool(guard, false);
                isJustGuard = false;
                isGuarding = false;
                yield break;
            }
            yield return null;
        }

        animator.SetBool(guard, false);
        isJustGuard = false;
        isGuarding = false;
    }
}
