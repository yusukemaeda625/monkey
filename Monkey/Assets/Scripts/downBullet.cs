using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class downBullet : MonoBehaviour
{
    [SerializeField] int durable = 1;
    [SerializeField] float speed = -0.2f;

    // Start is called before the first frame update
    void Start()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.sleepThreshold = -1;    
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed, -speed, 0);   
        if(durable == 0){
            Destroy(this.gameObject);
        }
    }

        public void Refrect(){
        GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        GetComponent<Rigidbody>().angularVelocity =  new Vector3(0,0,0);
        speed = Mathf.Abs(speed);
    }

        void OnTriggerEnter(Collider col){
        if(col.tag == "Bullet"){
            durable--;
            Debug.Log(durable);
            Destroy(col.gameObject);                            
        }
    }
}
