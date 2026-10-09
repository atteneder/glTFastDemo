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
using GLTFast.Logging;
using GLTFast;

public class TestLoader : MonoBehaviour {
    
    [SerializeField] bool responsive = true;

    public UnityAction<string> urlChanged;
    public UnityAction loadingBegin;
    public UnityAction<bool> loadingEnd;
    public UnityEvent<Transform> gotSceneRoot;
    public UnityEvent<Bounds> gotSceneBounds;

    [SerializeField] TrackballCamera trackBallCtrl;
    
    public string[] GetSceneNames()
    {
        return gltf?.SceneNames;
    }

    public int? currentSceneIndex => gltf?.CurrentSceneId;

    GameObject go;

#if UNITY_DOTS_HYBRID
    GltfEntityAsset gltf1;
#else
    GltfAsset gltf;
    public GameObjectSceneInstance sceneInstance { get; private set; }
#endif

    IDeferAgent deferAgent;

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
        if(go!=null) {
            gltf.ClearScenes();
            Destroy(go);
        }

        sceneInstance = null;

        // Wait one frame to minimize distortion by current frame's delta time
        await Task.Yield();
        
        Debug.Log("[TestLoader] loading " + url);

        urlChanged?.Invoke(url);
        loadingBegin?.Invoke();
        
        go = new GameObject("glTFast Loader");
        
#if UNITY_DOTS_HYBRID
        gltf1 = go1.AddComponent<GltfEntityAsset>();
#else
        gltf = go.AddComponent<GltfAsset>();
#endif
        gltf.LoadOnStartup = false;
        var logger = new CollectingLogger();
        var success = await gltf.Load(url,null,deferAgent,logger:logger);
        loadingEnd?.Invoke(success);
        if(success) {
            if (!gltf.CurrentSceneId.HasValue && gltf.SceneCount > 0) {
                // Fallback to first scene
                Debug.LogWarning("glTF has no main scene. Falling back to first scene.");
                await gltf.InstantiateScene(0);
            }
            GLTFast_onLoadComplete(gltf);
        } else {
            Debug.LogError("TestLoader: loading failed!");
            logger.LogAll();
        }
    }

    public void ClearScene() {
        gltf?.ClearScenes();
    }

    public void InstantiateScene(int sceneIndex)
    {
        gltf.InstantiateScene(sceneIndex);
#if !UNITY_DOTS_HYBRID
        sceneInstance = gltf.SceneInstance;
#endif
    }

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
