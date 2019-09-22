using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var canvas = GameObject.Find("FadeCanvas");                                 
        var fadeScript = canvas.GetComponent<Fade>();
        fadeScript.fadeColor = new Color(0f,0f,0f,1f);
        fadeScript.FadeIn();
        Invoke("ActiveResultCanvas",1);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ActiveResultCanvas(){
        var rc = GameObject.Find("ResultCanvas");               
        var timetext = GameObject.Find("TimeText");               
        timetext.GetComponent<Text>().text = PlayerPrefs.GetInt("PlayerMin").ToString("00") + ":" + PlayerPrefs.GetFloat("PlayerSec").ToString("F2");
        rc.GetComponent<Canvas>().enabled = true;
        
    } 

    public void ToTitle(){
        //var canvas = GameObject.Find("FadeCanvas");                                 
        //var fadeScript = canvas.GetComponent<Fade>();
        //fadeScript.fadeColor = new Color(0f,0f,0f,1.0f);
        //fadeScript.FadeOut(); 
        Invoke("SceneTransition",1);
    }  
    void SceneTransition(){
        
        SceneManager.LoadScene("Title");
    } 
}
