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
        Instantiate(bu, transform.position, Quaternion.identity);
    }
}
