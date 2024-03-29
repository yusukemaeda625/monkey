﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BarCon : MonoBehaviour
{
    [SerializeField] GameObject lifeTimeBar;
    [SerializeField] GameObject TimeText;
    private float scaleParam = 1f;
    [SerializeField] float limitTime = 180;



    [SerializeField] float candleChangeTime = 1f;
    private float candleTimer = 0;
    [SerializeField] Sprite candle1;
    [SerializeField] Sprite candle2;
    [SerializeField] Sprite candle3;


    [SerializeField] List<Sprite> nFont;
    private float remainingTime;
    // Start is called before the first frame update
    void Start()
    {
        remainingTime = limitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(remainingTime <= 0){
            GetComponent<GameSceneManager>().GameClear();
        }
        var time = this.GetComponent<MyGameTimer>().totalTime;        
        remainingTime -= Time.deltaTime;
        scaleParam = remainingTime / limitTime;         
        if(scaleParam >= 0f && scaleParam <= 1f){
            var tr = lifeTimeBar.GetComponent<RectTransform>();            
            tr.localScale = new Vector2(scaleParam,1f);          
            var t = (int)remainingTime;
            TimeText.GetComponent<Text>().text = t.ToString();            
        }else{
            TimeText.GetComponent<Text>().text = "0";
        }


        //ロウソクの絵の切り替え
        candleTimer += Time.deltaTime;
        var bar = GameObject.Find("LifeTimeBar").GetComponent<Image>();

        if(candleTimer >= candleChangeTime){
            if(candleTimer >= candleChangeTime * 2){
                bar.sprite = candle3;
                candleTimer = 0f;
            }else{
                bar.sprite = candle2;
            }            
        }else{
            bar.sprite = candle1;
        }    
    }

    public void ShortenLifeTime(float time){
        remainingTime -= time;
    }
}
