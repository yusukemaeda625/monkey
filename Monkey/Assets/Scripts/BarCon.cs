using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BarCon : MonoBehaviour
{
    [SerializeField] GameObject lifeTimeBar;
    [SerializeField] GameObject TimeText;
    private float scaleParam = 1f;
    [SerializeField] float limitTime = 180;

    private float remainingTime;
    // Start is called before the first frame update
    void Start()
    {
        remainingTime = limitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            remainingTime -= 5f;
        }
        var time = this.GetComponent<MyGameTimer>().totalTime;        
        remainingTime -= Time.deltaTime;
        scaleParam = remainingTime / limitTime;         
        if(scaleParam >= 0f && scaleParam <= 1f){
            var tr = lifeTimeBar.GetComponent<RectTransform>();            
            tr.localScale = new Vector2(scaleParam,1f);          
            TimeText.GetComponent<Text>().text = remainingTime.ToString("F2");
        }else{
            TimeText.GetComponent<Text>().text = "0.00";
        }
    }
}
