using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{    

    void Start()
    {
        var canvas = GameObject.Find("FadeCanvas");
        canvas.GetComponent<Fade>().FadeIn();        
    }
    
    void Update()
    {

        if (Input.GetKey(KeyCode.Space)) {            
            GameClear();
        }
    }

    public void GameOver(){
        var canvas = GameObject.Find("FadeCanvas");
        var fadeScript = canvas.GetComponent<Fade>();
        fadeScript.fadeColor = new Color(0.5f,0.5f,0.5f,1.0f);
        fadeScript.FadeOut();        
        Invoke("ActiveResultCanvas",canvas.GetComponent<Fade>().speed);
    }

    public void GameClear(){
        var gametimer = this.GetComponent<MyGameTimer>();     
        PlayerPrefs.SetInt("PlayerMin",gametimer.min);
        PlayerPrefs.SetFloat("PlayerSec",gametimer.sec);

        var canvas = GameObject.Find("FadeCanvas");
        var fadeScript = canvas.GetComponent<Fade>();
        fadeScript.fadeColor = new Color(0.5f,0.5f,0.5f,1.0f);
        fadeScript.FadeOut();        
        Invoke("ToResultScene",canvas.GetComponent<Fade>().speed);
    }

    void ToResultScene(){
        SceneManager.LoadScene("Result");
    }
}
