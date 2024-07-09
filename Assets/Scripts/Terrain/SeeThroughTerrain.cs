using UnityEngine;

public class SeeThroughTerrain : MonoBehaviour
{
    private Renderer[] renderers;
    
    private bool isSeeThrough = false;

    private void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void ToggleSeeThrough()
    {
        isSeeThrough = !isSeeThrough;

        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = !isSeeThrough;
        }
    }

    public bool IsSeeThrough()
    {
        return isSeeThrough;
    }
}
