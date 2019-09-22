using System;
using System.Collections;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region EffectVariables
    
    [SerializeField] private GameObject effectPosition = null;
    [SerializeField] private GameObject guardEffectPosition = null;
    [SerializeField] private Sprite[] dashEffectSprites = null;
    [SerializeField] private Sprite[] guardEffectSprites = null;
    [SerializeField] private Sprite[] justGuardEffectSprites = null;
    [SerializeField] private Sprite clearImage = null;
    private Image effect = null;
    
    #endregion

    #region PlayerMovementVariables
    
    private bool isGuarding = false;
    private bool isJustGuard = false;
    private bool hit = false;
    private bool isGuardStop = false;
    private int justGuardDuration = 8;
    private int guardStopDuration = 20;
    private int timer = 0;
    private int stopTimer = 0;
    private float movementVelocity = 2;
    private float duration = 0.3f;
    private float targetPosition;
    
    #endregion

    #region AnimationVariables

    [SerializeField] private AnimationClip dashAnim = null;
    [SerializeField] private AnimationClip backStepAnim = null;
    [SerializeField] private AnimationClip guardAnim = null;
    [SerializeField] private AnimationClip idleAnim = null;
    private Animator animator = null;
    private Animation animation = null;
    private int dash = Animator.StringToHash("dashTrigger");
    private int backStep = Animator.StringToHash("backStepTrigger");
    private int guard = Animator.StringToHash("guard");
    
    #endregion


    void Start()
    {
        animator = GetComponent<Animator>();
        animation = GetComponent<Animation>();
        effect = GameObject.FindGameObjectWithTag("Effect").GetComponent<Image>();

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
                    StartCoroutine(JustGuardEffectController());
                    Debug.Log("Just Guard");
                }
                if (isGuarding)
                {
                    StartCoroutine(GuardOnHit());
                    StartCoroutine(GuardEffectController());
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
            targetPosition = tempPosition + movementVelocity;
            StartCoroutine(DashEffectController());
            animator.SetTrigger(dash);
        }

        if (Input.GetButtonDown("D-Left") && !isGuardStop)
        {
            targetPosition = tempPosition - movementVelocity;
            animator.SetTrigger(backStep);
        }

        tempVec.x = Mathf.Lerp(tempPosition, targetPosition, duration);
        transform.position = tempVec;
    }

    IEnumerator JustGuardEffectController()
    {
        effect.transform.position = Camera.main.WorldToScreenPoint(guardEffectPosition.transform.position);
        for (int animationTimer = 0; animationTimer < 8; animationTimer++)
        {
            if (animationTimer < 8)
            {
                effect.sprite = justGuardEffectSprites[3];
            }

            if (animationTimer < 6)
            {
                effect.sprite = justGuardEffectSprites[2];
            }

            if (animationTimer < 4)
            {
                effect.sprite = justGuardEffectSprites[1];
            }

            if (animationTimer < 2)
            {
                effect.sprite = justGuardEffectSprites[0];
            }

            yield return null;
        }

        effect.sprite = clearImage;
    }
    
    IEnumerator GuardEffectController()
    {
        effect.transform.position = Camera.main.WorldToScreenPoint(guardEffectPosition.transform.position);
        for (int animationTimer = 0; animationTimer < 8; animationTimer++)
        {
            if (animationTimer < 8)
            {
                effect.sprite = guardEffectSprites[3];
            }

            if (animationTimer < 6)
            {
                effect.sprite = guardEffectSprites[2];
            }

            if (animationTimer < 4)
            {
                effect.sprite = guardEffectSprites[1];
            }

            if (animationTimer < 2)
            {
                effect.sprite = guardEffectSprites[0];
            }

            yield return null;
        }

        effect.sprite = clearImage;
    }

    IEnumerator DashEffectController()
    {
        effect.transform.position = Camera.main.WorldToScreenPoint(effectPosition.transform.position);
        for (int animationTimer = 0; animationTimer < 12; animationTimer++)
        {
            if (animationTimer < 12)
            {
                effect.sprite = dashEffectSprites[3];
            }
            
            if (animationTimer < 8)
            {
                effect.sprite = dashEffectSprites[2];
            }
            
            if (animationTimer < 4)
            {
                effect.sprite = dashEffectSprites[1];
            }
            
            if (animationTimer < 2)
            {
                effect.sprite = dashEffectSprites[0];
            }

            yield return null;
        }

        effect.sprite = clearImage;
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
