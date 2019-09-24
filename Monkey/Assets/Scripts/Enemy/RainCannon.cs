using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCannon : MonoBehaviour
{
    [SerializeField] AudioSource  explSE;
    [SerializeField] float Pos1 = 20;
    [SerializeField] float Pos2 = 30;
    [SerializeField] float Pos3  = 40;

    [SerializeField] float UpPower = 35f;
    [SerializeField] Vector3 force;        
    // Start is called before the first frame update
    void Start()
    {        
        var r = Random.Range(0f,10f);
        if(r <= 3f){
            force = new Vector3(-Pos1,UpPower,0);
        }else if(r <= 6f){
            force = new Vector3(-Pos2,UpPower,0);
        }else{
            force = new Vector3(-Pos3,UpPower,0);
        }                
        GetComponent<Rigidbody>().AddForce(force);
    }

    void OnDestroy(){                    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col){
        if(col.tag != "Perry" && col.tag != "Bullet"){     
            AudioSource.PlayClipAtPoint( explSE.clip, transform.position);           
            Destroy(this.gameObject);
        }
    }
}
