using UnityEngine;
using UnityEngine.Events;
#if !NO_GLTFAST
using GLTFast;
#endif
#if UNITY_GLTF
using UnityGLTF;
#endif

public class TestLoader : MonoBehaviour {

#if !NO_GLTFAST && UNITY_GLTF
    public const float variantDistance = 1;
#else
    public const float variantDistance = 0;
#endif

    public UnityAction<string> urlChanged;
    public UnityAction loadingBegin;
    public UnityAction loadingEnd;

    GameObject go1 = null;
    GameObject go2 = null;

#if !NO_GLTFAST
    GltfAsset gltf1;
#endif
#if UNITY_GLTF
    UnityGLTFLoader gltf2;
#endif

    float startTime = -1;
    GLTFast.IDeferAgent deferAgent;

    // Use this for initialization
    void Start () {
        deferAgent = new GLTFast.UninterruptedDeferAgent();
        // LoadUrl( GltfSampleModels.baseUrl+"Duck/glTF-Binary/Duck.glb" );
    }

    public void LoadUrl(string url) {

#if !NO_GLTFAST
        if(gltf1!=null) {
            gltf1.onLoadComplete-= GLTFast_onLoadComplete;
            gltf1 = null;
        }
#endif
#if UNITY_GLTF
        if(gltf2!=null) {
            gltf2.onLoadComplete-= UnityGltf_OnLoadComplete;
            gltf2 = null;
        }
#endif

        if(go1!=null) {
            Destroy(go1);
        }
        if(go2!=null) {
            Destroy(go2);
        }

        Debug.Log("loading "+url);

        startTime = Time.realtimeSinceStartup;
        loadingBegin();

#if !NO_GLTFAST
        go1 = new GameObject();
        gltf1 = go1.AddComponent<GLTFast.GltfAsset>();
        gltf1.loadOnStartup = false;
        gltf1.Load(url,null,deferAgent);
        gltf1.onLoadComplete += GLTFast_onLoadComplete;
#endif
#if UNITY_GLTF
        go2 = new GameObject();
        go2.transform.rotation = Quaternion.Euler(0,180,0);
        gltf2 = go2.AddComponent<UnityGLTFLoader>();
        gltf2.GLTFUri = url;
        gltf2.onLoadComplete += UnityGltf_OnLoadComplete;
#endif

        urlChanged(url);
    }

#if UNITY_GLTF
    void UnityGltf_OnLoadComplete()
    {
        loadingEnd();
        // time2Update((Time.realtimeSinceStartup-startTime)*1000);
        var bounds = CalculateLocalBounds(go2.transform);
        
        float targetSize = 2.0f;
        
        float scale = Mathf.Min(
            targetSize / bounds.extents.x,
            targetSize / bounds.extents.y,
            targetSize / bounds.extents.z
            );

        go2.transform.localScale = Vector3.one * scale;
        Vector3 pos = bounds.center;
        pos.x -= bounds.extents.x * variantDistance;
        pos *= -scale;
        go2.transform.position = pos;
    }
#endif

#if !NO_GLTFAST
    void GLTFast_onLoadComplete(GLTFast.GltfAsset asset, bool success)
    {
        loadingEnd();

        if(success) {
            var bounds = CalculateLocalBounds(gltf1.transform);
            
            float targetSize = 2.0f;
            
            float scale = Mathf.Min(
                targetSize / bounds.extents.x,
                targetSize / bounds.extents.y,
                targetSize / bounds.extents.z
                );
    
            if (!float.IsNaN(scale) && !float.IsInfinity(scale)) {
                gltf1.transform.localScale = Vector3.one * scale;
                Vector3 pos = bounds.center;
                pos.x += bounds.extents.x * variantDistance;;
                pos *= -scale;
                gltf1.transform.position = pos;
            }
        } else {
            Debug.LogError("TestLoader: loading failed!");
        }
    }
#endif

    static Bounds CalculateLocalBounds(Transform transform)
    {
        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        Vector3 localCenter = bounds.center - transform.position;
        bounds.center = localCenter;
        transform.rotation = currentRotation;
        return bounds;
    }
}
