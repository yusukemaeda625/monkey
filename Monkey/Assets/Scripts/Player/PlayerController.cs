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
    private bool isGuardStop = false;
    private int justGuardDuration = 10;
    private int guardStopDuration = 30;
    private int timer = 0;
    private int stopTimer = 0;
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
                if (!isGuarding && !isJustGuard)
                {
                    Debug.Log("Dead");
                }
                
                if (isJustGuard)
                {
                    StartCoroutine(JustGuardOnHit());
                    Debug.Log("Just Guard");
                }
                if (isGuarding)
                {
                    StartCoroutine(GuardOnHit());
                    Debug.Log("Guard");
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

        if (Input.GetButtonUp("Guard") && !isGuardStop)
        {
            StartCoroutine(JustGuard());
        }

        if (Input.GetButtonDown("D-Right")&& !isGuardStop)
        {
            targetPosition = tempPosition - movementVelocity;
            animator.SetTrigger(dash);
        }

        if (Input.GetButtonDown("D-Left") && !isGuardStop)
        {
            targetPosition = tempPosition + movementVelocity;
        }

        tempVec.x = Mathf.Lerp(tempPosition, targetPosition, duration);
        transform.position = tempVec;
    }

    IEnumerator JustGuard()
    {
        isGuarding = false;
        isJustGuard = true;
        for (timer = 0; timer < justGuardDuration; timer++)
        {
            yield return null;
        }

        animator.SetBool(guard, false);
        isJustGuard = false;
    }

    IEnumerator JustGuardOnHit()
    {
        animator.SetBool(guard, false);
        isJustGuard = false;
        yield break;
    }

    IEnumerator GuardOnHit()
    {
        isGuarding = true;
        isGuardStop = true;
        for (stopTimer = 0; stopTimer < guardStopDuration; stopTimer++)
        {
            isJustGuard = false;
            yield return null;
        }

        animator.SetBool(guard, false);
        isGuarding = false;
        isGuardStop = false;
    }
}
