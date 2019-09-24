using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asigarus : MonoBehaviour
{
    public enum AsigaruType{
        Asigaru_A,
        Asigaru_B,
        Asigaru_C
    }
    [SerializeField] AsigaruType myType;

    private float myTimer = 0f;
    public GameObject bulletPrefab;
    public GameObject rifleBulletPrefab;

    private bool isDead = false;
    [SerializeField] Vector3 shotPos = Vector3.zero;

    [SerializeField] float fireRate = 0.15f;
    public float interval = 1.5f;

    public float shotField = 12f;

    private GameObject player;

    private float killDistance = 3f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
            return;
            
        var ve = player.transform.position - this.transform.position;
        
        if(ve.magnitude > shotField || ve.magnitude <= killDistance){
            return;
        }
        myTimer += Time.deltaTime;
        if(myTimer >= interval){
        switch(myType){
            case AsigaruType.Asigaru_A :
            {                
                ShotNormalBullet();
            }
            break;
            case AsigaruType.Asigaru_B :
            {
                float t = 0f;
                for(int i = 0; i < 2; i++){
                    if(isDead)
                        break;
                    Invoke("ShotNormalBullet",t);
                    t += fireRate;
                }
            }
            break;
            case AsigaruType.Asigaru_C :
            {
                float t = 0.7f;
                ShotRifleBullet();
                for(int i = 0; i < 3; i++){
                    if(isDead)
                        break;
                    Invoke("ShotNormalBullet",t);
                    t += fireRate;
                }
            }
            break;
        }
            myTimer = 0f;
        }
    }

    void ShotNormalBullet(){        
        Instantiate(bulletPrefab, transform.position + shotPos, Quaternion.identity);             
        GetComponent<Animator>().SetTrigger("Shot");
    }

    void ShotRifleBullet(){
        Instantiate(rifleBulletPrefab, transform.position + shotPos, Quaternion.identity);     
    }

    public void Deth(){
        GetComponent<Animator>().SetBool("Dead",true);
        isDead = true;
    }
}
