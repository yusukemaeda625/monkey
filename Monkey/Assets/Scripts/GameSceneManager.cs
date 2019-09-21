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
        var canvas = GameObject.Find("FadeCanvas");
        var fadeScript = canvas.GetComponent<Fade>();
        fadeScript.fadeColor = new Color(0.5f,0.5f,0.5f,1.0f);
        fadeScript.FadeOut();        
        Invoke("ActiveResultCanvas",canvas.GetComponent<Fade>().speed);
    }

    public void ToTitle(){
        var canvas = GameObject.Find("FadeCanvas");                                 
        var fadeScript = canvas.GetComponent<Fade>();
        //fadeScript.fadeColor = new Color(0f,0f,0f,1.0f);
        fadeScript.FadeOut(); 
        Invoke("SceneTransition",canvas.GetComponent<Fade>().speed);
    }

    void ActiveResultCanvas(){
        var rc = GameObject.Find("ResultCanvas");               
        rc.GetComponent<Canvas>().enabled = true;
        var timetext = GameObject.Find("TimeText");
        var gametimer = this.GetComponent<MyGameTimer>();     
        timetext.GetComponent<Text>().text = gametimer.min.ToString("00") + ":" + gametimer.sec.ToString("F2");
    }

    void SceneTransition(){
        SceneManager.LoadScene("Title");
    }
}
