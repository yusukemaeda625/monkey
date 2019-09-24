using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bul : MonoBehaviour
{
    
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
            transform.Translate(-0.2f, 0, 0);
            timer = 0;
        }
    }
}
