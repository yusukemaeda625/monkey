using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerryController : MonoBehaviour
{    
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject cannonPrefab;
    [SerializeField] GameObject BigCannonPrefab;
    [SerializeField] GameObject rainCannonPrefab;
    private int hp = 3;
    private float timer = 0f;
    private float addForceParam = 0f;
    public float SkillInterval = 10f;

    [SerializeField] float battleDistance = 10f;
    [SerializeField] float rainRate = 0.3f;
    [SerializeField] GameObject player;

    [SerializeField] float kickField = 1f;
    [SerializeField] float kickPower = 3f;

    [SerializeField] int numCannonRash = 4;
    [SerializeField] float cannonRashInterval = 0.4f;

    public bool isBattle = false;

     void Start()
    {        
        player = GameObject.Find("Player");
        timer = SkillInterval - 1;
    }
    
    void Update()
    {                
        if(hp == 0){
            //死ぬ処理
            Destroy(this.gameObject);
        }        
        var ve = player.transform.position - this.transform.position;

        if(ve.magnitude <= kickField){
            Kick();
            return;
        }

        if(ve.magnitude <= battleDistance){
            isBattle = true;            
        if(timer >= SkillInterval){
            timer = 0f;

            switch(hp){
            case 1 :
            {
                //第三形態
                var r = Random.Range(0f,10f);
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
        Damage();
        Debug.Log("Perrys Kick");
        player.GetComponent<PlayerController>().KnockBackPlayer(-kickPower);
    }

    void Gatling(){
        //float shotRate = 
    }

    private void ShotBullet(){
        Instantiate(bulletPrefab, transform.position, Quaternion.identity);    
    }
    private void ShotCannon(){
        Instantiate(cannonPrefab, transform.position, Quaternion.identity);     
    }

    private void ShotBigCannon(){
        Instantiate(BigCannonPrefab, transform.position, Quaternion.identity);     
    }

    private void ShotRainCannon(){
        Instantiate(rainCannonPrefab, transform.position, Quaternion.identity);                                   
    }

    void OnTriggerEnter(Collider col){
        if(col.tag == "BigCannon"){

        }
    }

    //ペリーが攻撃を受けた時の処理
    public void Damage(){
        hp--;
    }

}
