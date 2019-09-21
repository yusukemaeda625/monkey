using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{    
    void Start(){
        var canvas = GameObject.Find("FadeCanvas");
        canvas.GetComponent<Fade>().FadeIn();
    }
    void Update()
    {        
        if (Input.GetKey(KeyCode.Space)) {
            var canvas = GameObject.Find("FadeCanvas");                         
            canvas.GetComponent<Fade>().FadeOut();
            Invoke("SceneTransition",canvas.GetComponent<Fade>().speed);
        }
    }
    void SceneTransition(){
        SceneManager.LoadScene("Game");
    }
}
