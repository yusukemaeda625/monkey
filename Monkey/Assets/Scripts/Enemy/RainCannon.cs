using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCannon : MonoBehaviour
{
    [SerializeField] float xmin = -50f;
    [SerializeField] float xmax = -30f;

    [SerializeField] float ymin = 30f;
    [SerializeField] float ymax = 30f;

    private Vector3 force;
    // Start is called before the first frame update
    void Start()
    {
        force = new Vector3(Random.Range(xmin,xmax),Random.Range(ymin,ymax),0);
        GetComponent<Rigidbody>().AddForce(force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col){
        if(col.tag != "Enemy"){
            Destroy(this.gameObject);
        }
    }
}
