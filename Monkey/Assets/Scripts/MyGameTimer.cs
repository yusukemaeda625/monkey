using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameTimer : MonoBehaviour
{
    public int min;
    public float sec;

    // Start is called before the first frame update
    void Start()
    {
        min = 0;
        sec = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        sec += Time.deltaTime;
        if(sec >= 60f){
            min++;
            sec = sec - 60;
        }
    }    
}
