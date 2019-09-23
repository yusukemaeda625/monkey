using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    enum AnimationStates
    {
        Idle,
        Dash,
        BackStep,
        Guard
    }
    
    #region EffectVariables
    
    [SerializeField] private GameObject effectPosition = null;
    [SerializeField] private GameObject guardEffectPosition = null;
    [SerializeField] private Sprite[] dashEffectSprites = null;
    [SerializeField] private Sprite[] guardEffectSprites = null;
    [SerializeField] private Sprite[] justGuardEffectSprites = null;
    [SerializeField] private Sprite clearImage = null;
    [SerializeField] private GameObject effectObject = null;
    private Image effect = null;
    
    #endregion

    #region PlayerMovementVariables
    
    [SerializeField] private int justGuardDuration = 8;
    [SerializeField] private int guardStopDuration = 20;
    [SerializeField] private int chargeDuration = 10;
    [SerializeField] private float knockBackVelocity = 0.8f;
    [SerializeField] private float rifleKnockBackVelocity = 1f;
    
    private bool isGuarding = false;
    private bool isJustGuard = false;
    private bool hit = false;
    private bool isGuardStop = false;
    private bool tsubameGaeshi = false;
    private bool inMistfiner = false;
    private bool playerIsDead = false;
    private int timer = 0;
    private int stopTimer = 0;
    private float movementVelocity = 2;
    private float mistfinerVelocity = 6;
    private float duration = 0.3f;
    private float targetPosition;
    
    #endregion

    #region AnimationVariables
    
    private Animator animator = null;
    private SimpleAnimation simpleAnim = null;
    private int mistFinerDuration = 50;
    
    #endregion


    void Start()
    {
        animator = GetComponent<Animator>();
        simpleAnim = GetComponent<SimpleAnimation>();
        simpleAnim.wrapMode = WrapMode.Once;
        Instantiate(effectObject);
        effect = GameObject.FindGameObjectWithTag("Effect").GetComponent<Image>();

        this.OnTriggerEnterAsObservable()
            .Subscribe(x =>
            {
                hit = true;
                bool dead = !isGuarding && !isJustGuard && !inMistfiner && !tsubameGaeshi;
                
                if (dead)
                {
                    playerIsDead = true;
                }

                if (inMistfiner)
                {
                    
                }
                
                if (tsubameGaeshi)
                {
                    // 反射を使う
                }
                
                if (isJustGuard)
                {
                    StartCoroutine(JustGuardOnHit());
                    StartCoroutine(JustGuardEffectController());
                }
                if (isGuarding)
                {
                    StartCoroutine(GuardOnHit());
                    targetPosition -= knockBackVelocity;
                    //animator.SetTrigger(guardStop);
                    simpleAnim.Play("GuardStop");
                    StartCoroutine(GuardEffectController());
                }

                Destroy(x.gameObject);
            });
    }

    void Update()
    {
        var tempVec = transform.position;
        var tempPosition = transform.position.x;

        if (Input.GetButton("Guard") && !isGuardStop)
        {
            isGuarding = true;
            simpleAnim.CrossFade("Guard", 0.1f);
        }

        if (Input.GetButtonUp("Guard") && !isGuardStop)
        {
            StartCoroutine(JustGuard());
        }

        if (Input.GetButtonDown("D-Right")&& !isGuardStop && !isGuarding)
        {
            targetPosition = tempPosition + movementVelocity;
            StartCoroutine(DashEffectController());
            simpleAnim.Play("Dash");
        }

        if (Input.GetButtonDown("D-Left") && !isGuardStop && !isGuarding)
        {
            targetPosition = tempPosition - movementVelocity;
            simpleAnim.Play("BackStep");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            TsubameGaeshi();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(Mistfiner());
        }

        if (!simpleAnim.isPlaying)
        {
            simpleAnim.CrossFade("Default", 0.1f);
        }

        if (playerIsDead)
        {
            OnDead();
        }

        tempVec.x = Mathf.Lerp(tempPosition, targetPosition, duration);
        transform.position = tempVec;
    }

    void OnDead()
    {
        Debug.Log("Player is Dead");
        // 死亡時のスキル取得処理
    }

    void TsubameGaeshi()
    {
        tsubameGaeshi = true;
        simpleAnim.Play("Swallow");
    }

    IEnumerator Mistfiner()
    {
        inMistfiner = true;
        simpleAnim.Play("Mistfiner");
        for (int animTimer = 0; animTimer < mistFinerDuration; animTimer++)
        {
            yield return null;
        }

        targetPosition = transform.position.x + mistfinerVelocity;
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
        var tempVec = effectPosition.transform.position;
        
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

            effectPosition.transform.position = tempVec;
            yield return null;
        }
        
        tempVec.x = transform.position.x;
        effectPosition.transform.position = tempVec;
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

        simpleAnim.CrossFade("Default", 0.1f);
        // animator.SetBool(guard, false);
        isJustGuard = false;
    }

    IEnumerator JustGuardOnHit()
    {
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
        
        isGuarding = false;
        isGuardStop = false;
    }

    public void KnockBackPlayer(float velocity)
    {
        targetPosition += velocity;
    }
}
