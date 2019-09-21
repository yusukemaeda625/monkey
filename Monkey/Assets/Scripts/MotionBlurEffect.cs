using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera)),ImageEffectAllowedInSceneView,ExecuteInEditMode]
public class MotionBlurEffect : MonoBehaviour
{
    [SerializeField] Material material;
    private RenderTexture accumBuffer;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (accumBuffer == null)
        {
            accumBuffer = new RenderTexture(src.width, src.height, 0);
            Graphics.Blit(src, accumBuffer);
        }
        accumBuffer.MarkRestoreExpected();
        Graphics.Blit(src, accumBuffer, material);
        Graphics.Blit(accumBuffer, dest);
    }
}
