using UnityEngine;

[RequireComponent(typeof(Camera)),ExecuteInEditMode,ImageEffectAllowedInSceneView]
public class PostEffect : MonoBehaviour
{
    [SerializeField] Material postProcessMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, postProcessMaterial);
    }
}
