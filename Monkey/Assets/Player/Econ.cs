using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Econ : MonoBehaviour
{
    [SerializeField] private GameObject bu = null;
    private float timer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer > 2)
        {
            Instantiate(bu, transform.position, Quaternion.identity);
            timer = 0;
        }
    }
}
