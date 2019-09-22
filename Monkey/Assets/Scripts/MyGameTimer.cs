using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameTimer : MonoBehaviour
{
    public int min;
    public float sec;

    public float totalTime;
    // Start is called before the first frame update
    void Start()
    {
        min = 0;
        sec = 0f;
        totalTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        sec += Time.deltaTime;
        if(sec >= 60f){
            min++;
            sec = sec - 60;
        }
    }    
}
