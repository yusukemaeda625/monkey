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
    [SerializeField] float rainRate = 0.3f;
    [SerializeField] GameObject player;

    [SerializeField] float kickField = 1f;
    [SerializeField] float kickPower = 3f;
    void Start()
    {        
        player = GameObject.Find("Player");
    }
    
    void Update()
    {                
        if(hp == 0){
            //死ぬ処理
            
        }
        var ve = player.transform.position - this.transform.position;

        if(ve.magnitude <= kickField){
            Kick();
            return;
        }

        if(timer >= SkillInterval){
            timer = 0f;

            switch(hp){
            case 1 :
            {
                //第三形態
            }
            break;
            case 2 :
            {
                //第二形態
                CannonRain();
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
        timer += Time.deltaTime;
    }

    //キャノンラッシュ
    void Cannon(){
        Invoke("ShotCannon",0f);
        Invoke("ShotCannon",0.5f);
        Invoke("ShotBigCannon",1f);
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
        player.GetComponent<PlayerController>().KnockBackPlayer(-kickPower);
    }

    void Gatling(){

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
