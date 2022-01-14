// Copyright 2020-2022 Andreas Atteneder
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System.Threading.Tasks;
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
    
    [SerializeField] bool responsive = true;

    public UnityAction<string> urlChanged;
    public UnityAction loadingBegin;
    public UnityAction loadingEnd;

    [SerializeField] TrackballCamera trackBallCtrl;
    
    public string[] GetSceneNames() {
#if GLTFAST_4_OR_NEWER
        return gltf1.sceneNames;
#else
        return null;
#endif 
    }

    public int? currentSceneIndex {
        get {
#if GLTFAST_4_OR_NEWER
            return gltf1.currentSceneId;
#else
            return null;
#endif
        }
    }
    
    GameObject go1 = null;
    GameObject go2 = null;

#if !NO_GLTFAST
#if UNITY_DOTS_HYBRID
    GltfEntityAsset gltf1;
#else
    GltfAsset gltf1;
    public GameObjectInstantiator.SceneInstance sceneInstance { get; private set; }
#endif
#endif
#if UNITY_GLTF
    UnityGLTFLoader gltf2;
#endif

    float startTime = -1;
    GLTFast.IDeferAgent deferAgent;

    // Use this for initialization
    void Start () {
        if(responsive) {
            deferAgent = gameObject.AddComponent<TimeBudgetPerFrameDeferAgent>();
        } else {
            deferAgent = new UninterruptedDeferAgent();
        }
        // LoadUrl( GltfSampleModels.baseUrl+"Duck/glTF-Binary/Duck.glb" );
    }

    public async Task LoadUrl(string url) {

#if UNITY_GLTF
        if(gltf2!=null) {
            gltf2.onLoadComplete-= UnityGltf_OnLoadComplete;
            gltf2 = null;
        }
#endif

        if(go1!=null) {
            gltf1.ClearScenes();
            Destroy(go1);
        }
        if(go2!=null) {
            Destroy(go2);
        }

        // Wait one frame to minimize distortion by current frame's delta time
        await Task.Yield();
        
        Debug.Log("loading "+url);

        startTime = Time.realtimeSinceStartup;
        urlChanged(url);
        loadingBegin();

#if !NO_GLTFAST
        go1 = new GameObject();
        
#if UNITY_DOTS_HYBRID
        gltf1 = go1.AddComponent<GltfEntityAsset>();
#else
        gltf1 = go1.AddComponent<GltfAsset>();
#endif
        gltf1.loadOnStartup = false;
        var success = await gltf1.Load(url,null,deferAgent);
        loadingEnd();
        if(success) {
            if (!gltf1.currentSceneId.HasValue && gltf1.sceneCount > 0) {
                // Fallback to first scene
                Debug.LogWarning("glTF has no main scene. Falling back to first scene.");
                gltf1.InstantiateScene(0);
            }
            GLTFast_onLoadComplete(gltf1);
        } else {
            Debug.LogError("TestLoader: loading failed!");
        }
#endif
#if UNITY_GLTF
        go2 = new GameObject();
        go2.transform.rotation = Quaternion.Euler(0,180,0);
        gltf2 = go2.AddComponent<UnityGLTFLoader>();
        gltf2.GLTFUri = url;
        gltf2.onLoadComplete += UnityGltf_OnLoadComplete;
#endif
    }

    public void ClearScene() {
#if GLTFAST_4_OR_NEWER
        gltf1.ClearScenes();
#endif
    }

    public void InstantiateScene(int sceneIndex) {
#if GLTFAST_4_OR_NEWER
        var success = gltf1.InstantiateScene(sceneIndex);
#if !UNITY_DOTS_HYBRID
        sceneInstance = gltf1.sceneInstance;
#endif
#endif
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
    void GLTFast_onLoadComplete(GltfAssetBase asset) {
#if UNITY_DOTS_HYBRID
        // TODO: calculate the bounding box
        trackBallCtrl.SetTarget(new Bounds(asset.transform.position,Vector3.one));
#else
        sceneInstance = (asset as GltfAsset).sceneInstance;
        var bounds = CalculateLocalBounds(asset.transform);

        if (trackBallCtrl != null) {
            trackBallCtrl.SetTarget(bounds);
        }
#endif
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
