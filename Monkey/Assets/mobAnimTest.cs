using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobAnimTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlayShotAnim(){
        var anmtr = GetComponent<Animator>();
        anmtr.SetTrigger("Shot");
    }    
}
