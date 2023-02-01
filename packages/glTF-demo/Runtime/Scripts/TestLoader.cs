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
#if GLTFAST_5_OR_NEWER
using GLTFast.Logging;
#endif
#if GLTFAST_4_OR_NEWER
using GLTFast;
#endif
#if UNITY_GLTF
using UnityGLTF;
#endif

public class TestLoader : MonoBehaviour {
    
    [SerializeField] bool responsive = true;
    [SerializeField] private LoadType loadType = LoadType.glTFast;
    public LoadType Loader
    {
        get => loadType;
        set => loadType = value;
    }

    public enum LoadType
    {
        glTFast = 2,
        UnityGltf = 4,
    }
    
    public UnityAction<string> urlChanged;
    public UnityAction<string> loadingBegin;
    public UnityAction<bool> loadingEnd;
    public UnityEvent<Transform> gotSceneRoot;
    public UnityEvent<Bounds> gotSceneBounds;

    [SerializeField] TrackballCamera trackBallCtrl;
    
    public string[] GetSceneNames() {
#if GLTFAST_4_OR_NEWER
        if (gltf1) return gltf1.SceneNames;
#else
#endif
        return null; 
    }

    public int? currentSceneIndex {
        get {
#if GLTFAST_4_OR_NEWER
            if (gltf1) return gltf1.CurrentSceneId;
#else
#endif
            return null;
        }
    }
    
    GameObject go1 = null;
    GameObject go2 = null;

#if GLTFAST
#if UNITY_DOTS_HYBRID
    GltfEntityAsset gltf1;
#else
    GltfAsset gltf1;
    public GameObjectSceneInstance sceneInstance { get; private set; }
#endif
#endif
#if UNITY_GLTF
    GLTFComponent gltf2;
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

    public async void LoadFromUrl(string url) => await LoadUrl(url);
    
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

        sceneInstance = null;

        // Wait one frame to minimize distortion by current frame's delta time
        await Task.Yield();
        
        Debug.Log("[TestLoader] loading " + url);

        startTime = Time.realtimeSinceStartup;
        urlChanged?.Invoke(url);
        loadingBegin?.Invoke(loadType.ToString());

        if (loadType.HasFlag(LoadType.glTFast))
        {
#if GLTFAST
            go1 = new GameObject("glTFast Loader");
            
#if UNITY_DOTS_HYBRID
            gltf1 = go1.AddComponent<GltfEntityAsset>();
#else
            gltf1 = go1.AddComponent<GltfAsset>();
#endif
            gltf1.LoadOnStartup = false;
            var logger = new CollectingLogger();
            var success = await gltf1.Load(url,null,deferAgent,logger:logger);
            loadingEnd?.Invoke(success);
            if(success) {
                if (!gltf1.CurrentSceneId.HasValue && gltf1.SceneCount > 0) {
                    // Fallback to first scene
                    Debug.LogWarning("glTF has no main scene. Falling back to first scene.");
#if GLTFAST_5_OR_NEWER
                    await
#endif
                    gltf1.InstantiateScene(0);
                }
                GLTFast_onLoadComplete(gltf1);
            } else {
                Debug.LogError("TestLoader: loading failed!");
                logger.LogAll();
            }
#endif
        }
        
        if(loadType.HasFlag(LoadType.UnityGltf))
        {
#if UNITY_GLTF
            Debug.Log("[TestLoader] Loading UnityGltf (loadType=" + loadType + ")");
            go2 = new GameObject("UnityGltf Loader");
            gltf2 = go2.AddComponent<GLTFComponent>();
            gltf2.AppendStreamingAssets = false;
            gltf2.Multithreaded = false;
            gltf2.GLTFUri = url;
            gltf2.onLoadComplete += UnityGltf_OnLoadComplete;
#endif
        }
    }

    public void ClearScene() {
#if GLTFAST_4_OR_NEWER
        if (gltf1) gltf1.ClearScenes();
#endif
    }

    public void InstantiateScene(int sceneIndex) {
#if GLTFAST_4_OR_NEWER
        var success = gltf1.InstantiateScene(sceneIndex);
#if !UNITY_DOTS_HYBRID
        sceneInstance = gltf1.SceneInstance;
#endif
#endif
    }

#if UNITY_GLTF
    void UnityGltf_OnLoadComplete()
    {
        Debug.Log("[TestLoader] " + nameof(UnityGltf_OnLoadComplete));
        loadingEnd?.Invoke(true);
        
        var bounds = CalculateLocalBounds(go2.transform);
        
        if (trackBallCtrl != null) {
            trackBallCtrl.SetTarget(bounds);
        }

        gotSceneRoot?.Invoke(go2.transform.childCount > 0 ? go2.transform.GetChild(0) : go2.transform);
        gotSceneBounds?.Invoke(bounds);
    }
#endif

#if GLTFAST
    void GLTFast_onLoadComplete(GltfAssetBase asset) {
#if UNITY_DOTS_HYBRID
        // TODO: calculate the bounding box
        trackBallCtrl.SetTarget(new Bounds(asset.transform.position,Vector3.one));
#else
        sceneInstance = (asset as GltfAsset).SceneInstance;
        
        var bounds = CalculateLocalBounds(asset.transform);

        if (trackBallCtrl != null) {
            trackBallCtrl.SetTarget(bounds);
        }

        gotSceneRoot?.Invoke(asset.transform.childCount > 0 ? asset.transform.GetChild(0) : asset.transform);
        gotSceneBounds?.Invoke(bounds);
#endif
    }
#endif

    static Bounds CalculateLocalBounds(Transform transform)
    {
        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        var rends = transform.GetComponentsInChildren<Renderer>();
        
        if (rends.Length < 1) return new Bounds(Vector3.zero, Vector3.one);
        
        Bounds bounds = new Bounds(rends[0].bounds.center, Vector3.zero);
        foreach (Renderer renderer in rends)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        Vector3 localCenter = bounds.center - transform.position;
        bounds.center = localCenter;
        transform.rotation = currentRotation;
        return bounds;
    }
}
