using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttr : MonoBehaviour
{
    public int durable = 1;
    [SerializeField] float speed = -0.2f;
    [SerializeField] float yspeed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.sleepThreshold = -1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed, yspeed, 0);   
        if(durable <= 0){
            Destroy(this.gameObject);
        }
    }

    public void Refrect(){
        GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        GetComponent<Rigidbody>().angularVelocity =  new Vector3(0,0,0);
        speed = Mathf.Abs(speed * 2);
        yspeed = 0f;
    }

    void OnTriggerEnter(Collider col){
        if(col.tag == "Bullet" || col.tag == "BigCannon"){
            var ba = col.gameObject.GetComponent<BulletAttr>();
            if(ba != null){
                int md = durable;
                durable -= ba.durable;                                
                ba.durable -= md;
            }
        }
        if(col.tag == "Enemy"){
            col.gameObject.GetComponent<Asigarus>().Deth();
            col.gameObject.GetComponent<BoxCollider>().enabled = false;            
            Destroy(this.gameObject);
        }
        if(col.tag == "Perry"){
            Destroy(this.gameObject);
        }
    }
}
