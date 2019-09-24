using System.Collections;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region SkillVariables

    private bool canSwallowBlade = false;
    private bool swallowBladePlus =  false;
    private bool swallowBladePlusPlus = false;
    private bool canMistfiner = false;
    private bool mistfinerPlus = false;
    private bool mistfinerPlusPlus = false;
    private bool advancedDash = false;
    private bool advancedGuard = false;
    private bool advancedStep = false;
    
    #endregion
    
    #region EffectVariables
    
    [SerializeField] private GameObject effectPosition = null;
    [SerializeField] private GameObject guardEffectPosition = null;
    [SerializeField] private GameObject mistfinerEffectPos = null;
    [SerializeField] private GameObject assistCanvas = null;
    [SerializeField] private Sprite[] dashEffectSprites = null;
    [SerializeField] private Sprite[] guardEffectSprites = null;
    [SerializeField] private Sprite[] justGuardEffectSprites = null;
    [SerializeField] private Sprite[] swallowEffectSprites = null;
    [SerializeField] private Sprite[] damageEffectSprites = null;
    [SerializeField] private Sprite[] mistfinerEffectSprites = null;
    [SerializeField] private Sprite[] skillImages = null;
    [SerializeField] private Sprite clearImage = null;
    [SerializeField] private GameObject effectObject = null;
    [SerializeField] private GameObject skillCanvas = null;
    private GameObject canvas = null;
    private Image effect = null;
    
    #endregion

    #region PlayerMovementVariables
    
    [SerializeField] private int justGuardDuration = 6;
    [SerializeField] private int guardStopDuration = 20;
    [SerializeField] private int chargeDuration = 10;
    [SerializeField] private float knockBackVelocity = 0.8f;
    [SerializeField] private float rifleKnockBackVelocity = 1f;
    private GameObject[] enemies = null;
    private GameObject perry = null;
    
    private bool isGuarding = false;
    private bool isJustGuard = false;
    private bool hit = false;
    private bool isGuardStop = false;
    private bool tsubameGaeshi = false;
    private bool inMistfiner = false;
    private bool playerIsDead = false;
    private bool canFinish = false;
    private bool freezing = false;
    private bool inSkillSelect = false;
    private bool finishBlow = false;
    private int timer = 0;
    private int stopTimer = 0;
    private float movementVelocity = 3;
    private float mistfinerVelocity = 4;
    private float duration = 0.3f;
    private float targetPosition;
    private int deathCounts = 0;
    
    #endregion

    #region AnimationVariables
    
    private Animator animator = null;
    private SimpleAnimation simpleAnim = null;
    private int mistFinerDuration = 50;
    private bool isBoss = true;
    private int guardHash = Animator.StringToHash("isGuard");
    private int guardStopHash = Animator.StringToHash("guardStop");
    private int dashHash = Animator.StringToHash("dashTrigger");
    private int backStepHash = Animator.StringToHash("backStep");
    private int swallowHash = Animator.StringToHash("swallowTrigger");
    private int mistfinerHash = Animator.StringToHash("mistfinerTrigger");
    private int finishHash = Animator.StringToHash("finishTrigger");
    private int deadHash = Animator.StringToHash("deadTrigger");
    private int bossHash = Animator.StringToHash("bossTrigger");
    private int mobHash = Animator.StringToHash("mobTrigger");
    private int selfKillHash = Animator.StringToHash("selfKillTrigger");

    #endregion

    private bool skillSelectInit = false;
    private int enemySouls = 0;
    
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        animator = gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        Instantiate(effectObject);
        effect = GameObject.FindGameObjectWithTag("Effect").GetComponent<Image>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        perry = GameObject.FindGameObjectWithTag("Perry");

        StartCoroutine(OnStartDeath());
        
        this.OnTriggerEnterAsObservable()
            .Subscribe(x =>
            {
                hit = true;
                bool dead = !isGuarding && !isJustGuard && !inMistfiner && !tsubameGaeshi;

                if (dead && (x.CompareTag("Bullet") || x.CompareTag("RifleBullet")))
                {
                    playerIsDead = true;
                }

                if (dead)
                {
                    if (!x.CompareTag("Perry"))
                    {
                        playerIsDead = true;
                    }
                }

                if (inMistfiner && x.CompareTag("Enemy"))
                {
                    StartCoroutine(NinjaExecution(x.gameObject));
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
                    if (x.CompareTag("RifleBullet") || x.CompareTag("Bullet"))
                    {
                        Destroy(x.gameObject);
                    }

                    StartCoroutine(JustGuardEffectController());
                }

                if (isGuarding)
                {
                        StartCoroutine(GuardOnHit());
                        if (x.CompareTag("RifleBullet"))
                        {
                            targetPosition -= rifleKnockBackVelocity;
                            Destroy(x.gameObject);
                        }

                        if (x.CompareTag("Bullet"))
                        {
                            targetPosition -= knockBackVelocity;
                            Destroy(x.gameObject);
                        }

                        animator.ResetTrigger(guardStopHash);
                        animator.SetTrigger(guardStopHash);
                        StartCoroutine(GuardEffectController());
                }
            });
    }

    void Update()
    {
        var tempVec = transform.position;
        var tempPosition = transform.position.x;
        float distance = 0f;
        float distToPerry = 0f;
        int enemyIndex = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            var tempDis = Vector3.Distance(transform.position, enemies[i].transform.position);
            if (distance < tempDis)
            {
                distance = tempDis;
                enemyIndex = i;
            }
        }

        distToPerry = Vector3.Distance(transform.position, perry.transform.position);

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
                animator.SetBool(guardHash, false);
                animator.ResetTrigger(dashHash);
                animator.SetTrigger(dashHash);
            }

            if (Input.GetButtonDown("D-Left") && !isGuardStop && !isGuarding)
            {
                targetPosition = tempPosition - movementVelocity;
                animator.SetBool(guardHash, false);
                animator.ResetTrigger(backStepHash);
                animator.SetTrigger(backStepHash);
            }
            
            if (canSwallowBlade && Input.GetButtonDown("SwallowBlade"))
            {
                StartCoroutine(SwallowBlade());
            }

            if (canMistfiner && Input.GetButtonDown("Mistfiner"))
            {
                StartCoroutine(Mistfiner());
                StartCoroutine(MistfinerEffect());
            }

            if (Input.GetButtonDown("Execution") && canFinish)
            {
                if (isBoss)
                {
                    StartCoroutine(NinjaExecution(enemies[enemyIndex]));
                }

                if (!isBoss)
                {
                    StartCoroutine(NinjaExecution(perry));
                }
            }

            if (Input.GetButtonDown("SelfKillL") && Input.GetButtonDown("SelfKillR"))
            {
                StartCoroutine(SelfKill());
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(SelfKill());
            }
        }

        if (distance < 3)
        {
            canFinish = true;
            isBoss = false;
        }

        if (distToPerry < 3)
        {
            canFinish = true;
            isBoss = true;
        }

        if (finishBlow)
        {
            if (Input.GetButtonDown("Execution"))
            {
                Time.timeScale = 1;
            }
        }

        if (inSkillSelect)
        {
            SkillSelector();
        }

        if (playerIsDead)
        {
            deathCounts++;
            StartCoroutine(PlayerDead());
            playerIsDead = false;
        }

        tempVec.x = Mathf.Lerp(tempPosition, targetPosition, duration);
        transform.position = tempVec;
    }

    void SkillSelector()
    {
        GameObject obj = null;
        Debug.Log(deathCounts);
        
        if (!skillSelectInit)
        {
            obj = Instantiate(skillCanvas);
            skillSelectInit = true;
        }
        var skillSprite = GameObject.FindGameObjectWithTag("SkillImage");
        var image = skillSprite.GetComponent<Image>();

        switch (deathCounts)
        {
            case 1:
                canSwallowBlade = true;
                image.sprite = skillImages[0];
                break;
            case 2:
                canMistfiner = true;
                image.sprite = skillImages[1];
                break;
            case 3:
                advancedGuard = true;
                image.sprite = skillImages[2];
                break;
            case 4:
                swallowBladePlus = true;
                image.sprite = skillImages[3];
                break;
            case 5:
                mistfinerPlus = true;
                image.sprite = skillImages[4];
                break;
            case 6:
                advancedDash = true;
                image.sprite = skillImages[5];
                break;
            case 7:
                swallowBladePlusPlus = true;
                image.sprite = skillImages[6];
                break;
            case 8:
                mistfinerPlusPlus = true;
                image.sprite = skillImages[7];
                break;
            default:
                image.sprite = clearImage;
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Execution"))
        {
            Time.timeScale = 1;
            inSkillSelect = false;
            image.sprite = clearImage;
        }
    }

    IEnumerator NinjaExecution(GameObject enemy)
    {
        freezing = true;
        enemySouls++;
        targetPosition = enemy.transform.position.x - 1.5f;
        var assistImage = Instantiate(assistCanvas).GetComponentInChildren<Image>();
        var tempPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
        tempPos.y += 200f;
        assistImage.transform.position = tempPos;
        
        animator.ResetTrigger(finishHash);
        animator.SetTrigger(finishHash);
        
        for (int i = 0; i < 120; i++)
        {
            if (i == 12)
            {
                finishBlow = true;
                Time.timeScale = 0;
            }

            if (i == 14)
            {
                if (isBoss)
                {
                    perry.GetComponent<PerryController>().Damage();
                    animator.ResetTrigger(bossHash);
                    animator.SetTrigger(bossHash);
                }

                if (!isBoss)
                {
                    animator.ResetTrigger(mobHash);
                    animator.SetTrigger(mobHash);
                }
            }

            yield return null;
        }

        freezing = false;
    }

    IEnumerator SelfKill()
    {
        freezing = true;
        deathCounts++;
        animator.ResetTrigger(selfKillHash);
        animator.SetTrigger(selfKillHash);
        
        for (int i = 0; i < 90; i++)
        {
            if (i == 39)
            {
                Time.timeScale = 0f;
                inSkillSelect = true;
            }

            yield return null;
        }

        freezing = false;
    }
    
    IEnumerator PlayerDead()
    {
        var bullets = GameObject.FindGameObjectsWithTag("Bullet");
        var rifles = GameObject.FindGameObjectsWithTag("RifleBullet");
        freezing = true;
        
        animator.ResetTrigger(deadHash);
        animator.SetTrigger(deadHash);

        foreach (var bullet in bullets)
        {
            Destroy(bullet);
        }

        foreach (var rifle in rifles)
        {
            Destroy(rifle);
        }
        
        for (int i = 0; i < 60; i++)
        {
            if (i < 31)
            {
                targetPosition -= 0.1f;
            }

            if (i == 31)
            {
                inSkillSelect = true;
                Time.timeScale = 0;
            }
            
            if (i > 31)
            {
                targetPosition -= 0.01f;
            }
            
            yield return null;
        }

        freezing = false;
    }

    IEnumerator SwallowBlade()
    {
        effect.transform.position = Camera.main.WorldToScreenPoint(guardEffectPosition.transform.position);

        freezing = true;
        tsubameGaeshi = true;
        var tsubameDuration = 3;
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

    IEnumerator OnStartDeath()
    {
        StartCoroutine(SelfKill());
        yield break;
    }

    IEnumerator MistfinerEffect()
    {
        effect.transform.position = Camera.main.WorldToScreenPoint(mistfinerEffectPos.transform.position);
        
        for (int i = 0; i < 51; i++)
        {
            effect.transform.position = Camera.main.WorldToScreenPoint(mistfinerEffectPos.transform.position);

            if (i < 51)
            {
                effect.sprite = mistfinerEffectSprites[23];
            }

            if (i < 50)
            {
                effect.sprite = mistfinerEffectSprites[22];
            }

            if (i < 49)
            {
                effect.sprite = mistfinerEffectSprites[21];
            }

            if (i < 48)
            {
                effect.sprite = mistfinerEffectSprites[20];
            }

            if (i < 47)
            {
                effect.sprite = mistfinerEffectSprites[19];
            }
            
            if (i < 45)
            {
                effect.sprite = mistfinerEffectSprites[18];
            }
            
            if (i < 43)
            {
                effect.sprite = mistfinerEffectSprites[17];
            }
            
            if (i < 41)
            {
                effect.sprite = mistfinerEffectSprites[16];
            }

            if (i < 38)
            {
                effect.sprite = mistfinerEffectSprites[15];
            }
            
            if (i < 36)
            {
                effect.sprite = mistfinerEffectSprites[14];
            }

            if (i < 33)
            {
                effect.sprite = mistfinerEffectSprites[13];
            }
            
            if (i < 31)
            {
                effect.sprite = mistfinerEffectSprites[12];
            }
            
            if (i < 28)
            {
                effect.sprite = mistfinerEffectSprites[11];
            }
            
            if (i < 26)
            {
                effect.sprite = mistfinerEffectSprites[10];
            }
            
            if (i < 23)
            {
                effect.sprite = mistfinerEffectSprites[9];
            }
            
            if (i < 20)
            {
                effect.sprite = mistfinerEffectSprites[8];
            }
            
            if (i < 18)
            {
                effect.sprite = mistfinerEffectSprites[7];
            }
            
            if (i < 16)
            {
                effect.sprite = mistfinerEffectSprites[6];
            }
            
            if (i < 13)
            {
                effect.sprite = mistfinerEffectSprites[5];
            }
            
            if (i < 10)
            {
                effect.sprite = mistfinerEffectSprites[4];
            }
            
            if (i < 8)
            {
                effect.sprite = mistfinerEffectSprites[3];
            }
            
            if (i < 6)
            {
                effect.sprite = mistfinerEffectSprites[2];
            }
            
            if (i < 4)
            {
                effect.sprite = mistfinerEffectSprites[1];
            }

            if (i < 1)
            {
                effect.sprite = mistfinerEffectSprites[0];
            }

            yield return null;
        }

        effect.sprite = clearImage;
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

    IEnumerator DamageEffect()
    {
        effect.transform.position = Camera.main.WorldToScreenPoint(transform.position);

        for (int i = 0; i < 5; i++)
        {
            if (i < 5)
            {
                effect.sprite = damageEffectSprites[3];
            }

            if (i < 3)
            {
                effect.sprite = damageEffectSprites[2];
            }

            if (i < 2)
            {
                effect.sprite = damageEffectSprites[1];
            }

            if (i < 1)
            {
                effect.sprite = damageEffectSprites[0];
            }
            yield return null;
        }

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

    public bool GetGuardState()
    {
        return isGuarding || isJustGuard;
    }
}
