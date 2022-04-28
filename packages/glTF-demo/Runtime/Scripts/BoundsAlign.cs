using UnityEngine;

public class BoundsAlign : MonoBehaviour
{
    public bool scaleUniform = true;
    public float scale = 5;
    
    public void UpdateFromBounds(Bounds bounds)
    {
        var bottomCenter = bounds.center;
        bottomCenter.y = bounds.min.y;

        transform.position = bottomCenter;
        
        var scl = bounds.size;
        if (scaleUniform) {
            var max = Mathf.Max(scl.x, scl.y, scl.z);
            scl = Vector3.one * max;
        }

        transform.localScale = scl * scale;
        
#if UNITY_HDRP
        // Probe doesn't take parent scale into account
        var probe = GetComponent<UnityEngine.Rendering.HighDefinition.PlanarReflectionProbe>();
        if (probe)
        {
            probe.influenceVolume.boxSize = scl * 1.5f;
        }
#endif
    }
}
