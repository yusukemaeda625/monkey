using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using UnityEngine.Experimental.Animations;
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
    
    #region SkillVariables

    private bool canSwallowBlade = true;
    private bool swallowBladePlus = false;
    private bool swallowBladePlusPlus = false;
    private bool canMistfiner = true;
    private bool mistfinerPlus = false;
    private bool mistfinerPlusPlus = false;
    private bool advancedDash = false;
    private bool advancedGuard = false;
    private bool advancedStep = false;
    
    #endregion
    
    #region EffectVariables
    
    [SerializeField] private GameObject effectPosition = null;
    [SerializeField] private GameObject guardEffectPosition = null;
    [SerializeField] private Sprite[] dashEffectSprites = null;
    [SerializeField] private Sprite[] guardEffectSprites = null;
    [SerializeField] private Sprite[] justGuardEffectSprites = null;
    [SerializeField] private Sprite[] swallowEffectSprites = null;
    [SerializeField] private Sprite clearImage = null;
    [SerializeField] private GameObject effectObject = null;
    private Image effect = null;
    
    #endregion

    #region PlayerMovementVariables
    
    [SerializeField] private int justGuardDuration = 6;
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
    private bool canFinish = false;
    private bool freezing = false;
    private int timer = 0;
    private int stopTimer = 0;
    private float movementVelocity = 2;
    private float mistfinerVelocity = 4;
    private float duration = 0.3f;
    private float targetPosition;
    private int deathCounts = 0;
    
    #endregion

    #region AnimationVariables
    
    private Animator animator = null;
    private SimpleAnimation simpleAnim = null;
    private int mistFinerDuration = 50;
    private int guardHash = Animator.StringToHash("isGuard");
    private int dashHash = Animator.StringToHash("dashTrigger");
    private int backStepHash = Animator.StringToHash("backStep");
    private int swallowHash = Animator.StringToHash("swallowTrigger");
    private int mistfinerHash = Animator.StringToHash("mistfinerTrigger");
    private int finishHash = Animator.StringToHash("finishTrigger");
    private int isBossHash = Animator.StringToHash("isBoss");
    private int deadHash = Animator.StringToHash("deadTrigger");

    #endregion

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Start()
    {
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

                if (inMistfiner && mistfinerPlusPlus)
                {
                    freezing = false;
                }
                
                if (tsubameGaeshi)
                {
                    x.GetComponent<BulletAttr>().Refrect();
                    if (swallowBladePlusPlus)
                    {
                        freezing = false;
                    }
                }
                
                if (isJustGuard)
                {
                    isJustGuard = false;
                    StartCoroutine(JustGuardEffectController());
                }
                if (isGuarding)
                {
                    StartCoroutine(GuardOnHit());
                    if (x.CompareTag("RifleBullet"))
                    {
                        targetPosition -= rifleKnockBackVelocity;
                    }

                    if (x.CompareTag("Bullet"))
                    {
                        targetPosition -= knockBackVelocity;
                    }
                    
                    animator.ResetTrigger("guardStop");
                    animator.SetTrigger("guardStop");
                    StartCoroutine(GuardEffectController());
                }

                Destroy(x.gameObject);
            });
    }

    void Update()
    {
        var tempVec = transform.position;
        var tempPosition = transform.position.x;

        if (!freezing)
        {
            if (Input.GetButton("Guard") && !isGuardStop)
            {
                isGuarding = true;
                animator.SetBool(guardHash, true);
            }

            if (Input.GetButtonUp("Guard") && !isGuardStop)
            {
                StartCoroutine(JustGuard());
            }

            if (Input.GetButtonDown("D-Right") && !isGuardStop && !isGuarding)
            {
                targetPosition = tempPosition + movementVelocity;
                StartCoroutine(DashEffectController());
                animator.ResetTrigger(dashHash);
                animator.SetTrigger(dashHash);
            }

            if (Input.GetButtonDown("D-Left") && !isGuardStop && !isGuarding)
            {
                targetPosition = tempPosition - movementVelocity;
                animator.ResetTrigger(backStepHash);
                animator.SetTrigger(backStepHash);
            }
            
            if (canSwallowBlade && Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(SwallowBlade());
            }

            if (canMistfiner && Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(Mistfiner());
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.ResetTrigger(finishHash);
                animator.SetTrigger(finishHash);
            }
        }

        if (playerIsDead || Input.GetKeyDown(KeyCode.D))
        {
            OnDead();
        }

        tempVec.x = Mathf.Lerp(tempPosition, targetPosition, duration);
        transform.position = tempVec;
    }

    void OnDead()
    {
        animator.ResetTrigger(deadHash);
        animator.SetTrigger(deadHash);
        targetPosition -= 1;
        Debug.Log("Player is Dead");
        // 死亡時のスキル取得処理
    }

    IEnumerator SwallowBlade()
    {
        effect.transform.position = Camera.main.WorldToScreenPoint(guardEffectPosition.transform.position);

        freezing = true;
        tsubameGaeshi = true;
        var tsubameDuration = 4;
        if (swallowBladePlus)
        {
            tsubameDuration = 8;
        }
        animator.ResetTrigger(swallowHash);
        animator.SetTrigger(swallowHash);
        
        for (int animTimer = 0; animTimer < 11; animTimer++)
        {
            if (animTimer > tsubameDuration)
            {
                tsubameGaeshi = false;
            }
            effect.sprite = swallowEffectSprites[10 - animTimer];
            yield return null;
        }

        freezing = false;
        effect.sprite = clearImage;
    }

    IEnumerator Mistfiner()
    {
        freezing = true;
        animator.ResetTrigger(mistfinerHash);
        animator.SetTrigger(mistfinerHash);
        for (int animTimer = 0; animTimer < mistFinerDuration; animTimer++)
        {
            yield return null;
        }

        inMistfiner = true;
        if (mistfinerPlus)
        {
            mistfinerVelocity = 6;
        }
        targetPosition = transform.position.x + mistfinerVelocity;
        freezing = false;
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
            if (animationTimer > 2 && advancedDash)
            {
                isGuarding = true;
            }
            
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

        isGuarding = false;
        tempVec.x = transform.position.x;
        effectPosition.transform.position = tempVec;
        effect.sprite = clearImage;
    }

    IEnumerator JustGuard()
    {
        isGuarding = false;
        isJustGuard = true;
        if (advancedGuard)
        {
            justGuardDuration = 10;
        }
        for (timer = 0; timer < justGuardDuration; timer++)
        {
            yield return null;
        }
        
        animator.SetBool(guardHash, false);
        justGuardDuration = 6;
        isJustGuard = false;
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
