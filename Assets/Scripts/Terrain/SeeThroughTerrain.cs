using System.Collections;
using UnityEngine;

public class SeeThroughTerrain : MonoBehaviour
{
    private Renderer[] renderers;

    private bool isSeeThrough = false;

    private float fadeDuration = 1.0f; 

    private void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void ToggleSeeThrough()
    {
        isSeeThrough = !isSeeThrough;

        foreach (Renderer renderer in renderers)
        {
            
            foreach (Material material in renderer.materials)
            {
                SetMaterialTransparent(material);
            }

            StartCoroutine(FadeRenderer(renderer, isSeeThrough));
        }
    }

    public bool IsSeeThrough()
    {
        return isSeeThrough;
    }

    private void SetMaterialTransparent(Material material)
    {
        
        if (material.shader.name != "Standard")
        {
            material.shader = Shader.Find("Standard");
        }

        
        material.SetFloat("_Mode", 3); 
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }

    private IEnumerator FadeRenderer(Renderer renderer, bool fadeOut)
    {
        foreach (Material material in renderer.materials)
        {
            Color originalColor = material.color;
            float startAlpha = originalColor.a;
            float endAlpha = fadeOut ? 0.0f : 1.0f;

            for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
            {
                float blend = Mathf.Clamp01(t / fadeDuration);
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(startAlpha, endAlpha, blend);
                material.color = newColor;
                yield return null;
            }

            
            Color finalColor = originalColor;
            finalColor.a = endAlpha;
            material.color = finalColor;
        }
    }
}
