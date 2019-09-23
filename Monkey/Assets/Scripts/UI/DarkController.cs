using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkController : MonoBehaviour
{
    [SerializeField] GameObject UpperImage;
    [SerializeField] GameObject LowerImage;

    public float slowTime = 3;
    public float slowSpeed = 1;
    public float normalSpeed = 3;
    private float moveSpeed = 1;
    private bool isMove = false;
    private float timer = 0f;
    void Start()
    {
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if(timer <= slowTime){
            moveSpeed = slowSpeed;
        }else{
            moveSpeed = normalSpeed;
        }
        if(isMove){
            UpperImage.transform.Translate(new Vector3(0,moveSpeed,0));
            LowerImage.transform.Translate(new Vector3(0,-moveSpeed,0));
        }
    }

    public void Darkness(){
        timer = 0f;
        isMove =  false;
        UpperImage.transform.position = (new Vector3(-1030,0,0));
        LowerImage.transform.position = (new Vector3(1400,0,0));
    }

    public void Open(){
        timer = 0;
        isMove = true;
    }

}
