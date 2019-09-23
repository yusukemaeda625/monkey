using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerryController : MonoBehaviour
{    
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject cannonPrefab;
    [SerializeField] GameObject BigCannonPrefab;
    [SerializeField] GameObject rainCannonPrefab;
    [SerializeField] GameObject downCannonPrefab;
    public int hp = 3;
    private float timer = 0f;
    private float addForceParam = 0f;
    public float SkillInterval = 5f;

    [SerializeField] float battleDistance = 10f;
    [SerializeField] float rainRate = 0.3f;
    GameObject player;

    [SerializeField] float kickField = 4f;
    [SerializeField] float kickPower = 3f;
    [SerializeField] float kickLag = 1f;

    [SerializeField] int numCannonRash = 4;
    [SerializeField] float cannonRashInterval = 0.4f;

    [SerializeField] Vector3 shotPos = Vector3.zero;
    [SerializeField] float shotStopDist = 3f;
    public bool isBattle = false;
    //isStan == true (can call Damage)
    public bool isStan = false;
    public float stanTime = 0f;
    private float stanTimer = 0f;

    public bool isYoroke = false;
    private float yorokeTimer = 0f;
    private float yorokeTime = 5f;

    public bool isDead = false;

    private int oldHp;
    
    bool iskicked = false;
     void Start()
    {        
        oldHp = hp;
        player = GameObject.Find("Player");
        timer = SkillInterval - 1;
    }
    
    void Update()
    {   
        if(isDead)
            return;
        if(hp == 0){
            //死ぬ処理
            //TODO : 死ぬアニメーションを再生
            GetComponent<Animator>().SetBool("Dead",true);
            isDead = true;            
            //Destroy(this.gameObject);
        }

        if(isStan){
            Debug.Log("Stan Now");
            if(hp != oldHp){
                isStan = false;
                oldHp = hp;
            }
            GetComponent<Animator>().SetBool("Piyo",true);            
            return;
        }else{
            GetComponent<Animator>().SetBool("Piyo",false);            
        }        

        if(isYoroke){            
            yorokeTimer += Time.deltaTime;            
            if(yorokeTimer >= yorokeTime){
                isYoroke = false;
                yorokeTime = 0f;
                Debug.Log("clear yoroke");
                GetComponent<Animator>().SetBool("Yoroke",false);
                //TODO : ガードアニメーション再生
            }
        }


        var ve = player.transform.position - this.transform.position;

        if(ve.magnitude <= kickField && hp == 1 && !isYoroke && !iskicked){
            GetComponent<Animator>().SetTrigger("Kick");
            Invoke("Kick",kickLag);            
            iskicked = true;
            return;
        }

        if(ve.magnitude <= battleDistance){
            isBattle = true;            


        if(timer >= SkillInterval && ve.magnitude >= shotStopDist && !isYoroke){
            timer = 0f;            
            switch(hp){
            case 1 :
            {
                //第三形態                
                var r = Random.Range(0f,12f);
                Debug.Log(r);
                if(r >= 5f){
                    cannonRashInterval = Random.Range(0.2f,0.5f);
                    RandomCannonRash();
                }else{
                    CannonRain();
                }    
            }
            break;
            case 2 :
            {
                //第二形態                
                var r = Random.Range(0f,10f);
                Debug.Log(r);
                if(r >= 5f){
                    RandomCannonRash();                    
                }else{
                    CannonRain();
                }               
            }
            break;
            case 3 :
            {
                //初期状態                
                Cannon();
            }
            break;
        }        
        }
        }
        timer += Time.deltaTime;
    }

    void Piyo(){
        isStan = true;        
        //TODO : PlayPiyoAnimation
    }

    void Cannon(){
        Invoke("ShotCannon",0f);
        Invoke("ShotCannon",0.5f);
        Invoke("ShotBigCannon",1f);
    }

    void RandomCannonRash(){
        var t = 0f;
        for(int i = 0; i < numCannonRash; i++){
            var r = Random.Range(0f,10f);
            if(r >= 3f){
                Invoke("ShotCannon",t);
            }else{
                Invoke("ShotBigCannon",t);
            }         
            t += cannonRashInterval;   
        }
    }

    void NormalCannonRash(){
        var t = 0f;
        for(int i = 0; i < numCannonRash; i++){
            Invoke("ShotCannon",t);
            t += cannonRashInterval;
        }
    }
    
    void CannonRain(){
        float shotRate = rainRate;        
        float t = 0f;    
        for(int i = 0; i < 10; i++){            
            Invoke("ShotRainCannon",t);
            t += shotRate;            
        }
    }
    
    void Kick(){                
        Debug.Log("Perrys Kick");        
        var gs = player.GetComponent<PlayerController>().GetGuardState();        
        if(!gs){
            player.GetComponent<PlayerController>().KnockBackPlayer(-kickPower);
        }
        if(gs){
            Debug.Log("Kick Guard!!!");
            yorokeTimer = 0f;
            isYoroke = true;
            Invoke("ShotCannon",2);
            GetComponent<Animator>().SetBool("Yoroke",true);
            Debug.Log("YOROKE");
        }
        iskicked = false;
    }

    private void ShotBullet(){
        Instantiate(bulletPrefab, transform.position + shotPos, Quaternion.identity);    
        GetComponent<Animator>().SetTrigger("Shot");
    }
    private void ShotCannon(){
        Instantiate(cannonPrefab, transform.position + shotPos, Quaternion.identity);     
        GetComponent<Animator>().SetTrigger("Shot");
    }

    private void ShotBigCannon(){
        Instantiate(BigCannonPrefab, transform.position + shotPos, Quaternion.identity);     
        GetComponent<Animator>().SetTrigger("Shot");
    }

    private void ShotRainCannon(){  
        Instantiate(rainCannonPrefab, transform.position + shotPos, Quaternion.identity);  
        GetComponent<Animator>().SetTrigger("Shot");                                       
    }

    private void ShotDownCannon(){
        Instantiate(downCannonPrefab,transform.position + shotPos, Quaternion.identity);
    }

    void OnTriggerEnter(Collider col){        
        if(col.tag == "BigCannon"){
            if(hp > 1){
                Piyo();
                Debug.Log("Perry Collide Big Cannon");
            }
            if(hp == 1){
                if(isYoroke){
                    Piyo();
                }
            }    
        }
    }

    //ペリーが攻撃を受けた時の処理
    public void Damage(){
        if(isStan)
            hp--;
    }

}
