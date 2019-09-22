using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] GameObject panel;   
    private Image fadeImage;
    public Color fadeColor;
    public bool isFadeIn = false;
    public bool isFadeOut = false;
    public float speed = 1;
    private float alpha = 1.0f;

    void Start()
    {
        fadeImage = panel.GetComponent<Image>();
        fadeColor = new Color(0,0,0,0);
    }
    
    void Update()
    {
        if(isFadeIn){
            alpha -= speed * Time.deltaTime;
            if(alpha <= 0.0f){
                isFadeIn = false;
            }            
        }

        if(isFadeOut){
            alpha += speed * Time.deltaTime;
            if(alpha >= 1.0){
                isFadeOut = false;
            }            
        }
        fadeImage.color = new Color (fadeColor.r,fadeColor.g,fadeColor.b,alpha);
    }

    public void FadeOut(){
        isFadeIn = false;
        isFadeOut = true;
    }

    public void  FadeIn(){
        isFadeIn = true;
        isFadeOut = false;
    }
}
