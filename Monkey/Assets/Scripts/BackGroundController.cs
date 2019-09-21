using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [SerializeField] float speedParam = 0.5f;

    void Update()
	{
		float scroll = Mathf.Repeat (Time.time * speedParam, 1);
		Vector2 offset = new Vector2 (scroll, 0);
		GetComponent<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", offset);
	}
}
