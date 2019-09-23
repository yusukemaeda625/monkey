using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCannon : MonoBehaviour
{
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
        transform.Translate(speed, 0, 0);      
    }

    public void Refrect(){
        GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        GetComponent<Rigidbody>().angularVelocity =  new Vector3(0,0,0);
        speed = Mathf.Abs(speed);
    }
}
