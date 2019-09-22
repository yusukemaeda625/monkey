using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{    
    private Dictionary<Material,Shader> dictionaryMaterialShader = new Dictionary<Material, Shader>();
    private List<Material> changedMaterials = new List<Material>();
    private Material defaultSkyBoxMaterial;
    private Material blackMat;
    void Start()
    {
        var canvas = GameObject.Find("FadeCanvas");
        canvas.GetComponent<Fade>().FadeIn();             
        defaultSkyBoxMaterial = RenderSettings.skybox;
        blackMat = new Material(Shader.Find("Unlit/black"));
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) {            
            ChangeSceneMatsBKWH();
        }
        if(Input.GetKey(KeyCode.Escape)){
            ResetMats();
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
        fadeScript.fadeColor = new Color(0f,0f,0f,1.0f);
        fadeScript.FadeOut();        
        Invoke("ToResultScene",canvas.GetComponent<Fade>().speed);
    }

    void ToResultScene(){
        SceneManager.LoadScene("Result");
    }

    //シーン暗転
    public void SceneDarkness(){
        GameObject.Find("BlackImageUp").GetComponent<Image>().enabled = true;
        GameObject.Find("BlackImageDown").GetComponent<Image>().enabled = true;
    }

    //シーン明転    
    public void OpenDarkness(){
        GameObject.Find("Dark").GetComponent<DarkController>().Open();
    }

    //白黒シーン
    public void ChangeSceneMatsBKWH(){
        //RenderSettings.skybox = blackMat;
        foreach(GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject))){
            if(obj.activeInHierarchy){
                var r = obj.GetComponent<Renderer>();
                if(r != null){
                    if(obj.tag == "Player" || obj.tag == "Enemy"){
                        foreach(var m in r.materials){         
                            if(!changedMaterials.Contains(m)){                
                                changedMaterials.Add(m);
                                dictionaryMaterialShader.Add(m,m.shader);
                               m.shader = Shader.Find("Unlit/white");
                            }
                        }
                    }else{
                        foreach(var m in r.materials){
                            if(!changedMaterials.Contains(m)){
                                changedMaterials.Add(m);
                                dictionaryMaterialShader.Add(m,m.shader);
                                m.shader = Shader.Find("Unlit/black");
                            }
                        }
                    }
                }
            }
        }
    }

    public void ResetMats(){
        foreach(var mat in changedMaterials){
            mat.shader = dictionaryMaterialShader[mat];
        }
        RenderSettings.skybox = defaultSkyBoxMaterial;
        changedMaterials.Clear();
        dictionaryMaterialShader.Clear();
    }
}
