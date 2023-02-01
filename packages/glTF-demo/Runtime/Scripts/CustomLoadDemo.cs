using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using GLTFast.Schema;
using UnityEngine;

public class CustomLoadDemo : MonoBehaviour {
    
    public string[] manyUrls;
    
    // Start is called before the first frame update
    async void Start() {
        await CustomInstantiation();
        await CustomDeferAgent();
    }

    static async Task CustomImportSettings() {
        var gltf = new GLTFast.GltfImport();
        // Create a settings object and configure it accordingly
        var settings = new ImportSettings {
            GenerateMipMaps = true,
            AnisotropicFilterLevel = 3,
            NodeNameMethod = NameImportMethod.OriginalUnique
        };
        // Load the glTF and pass along the settings
        var success = await gltf.Load("file:///path/to/file.gltf", settings);

        if (success) {
            var gameObject = new GameObject("glTF");
#if GLTFAST_5_OR_NEWER
            await gltf.InstantiateMainSceneAsync(gameObject.transform);
#else
            gltf.InstantiateMainScene(gameObject.transform);
#endif
        }
        else {
            Debug.LogError("Loading glTF failed!");
        }
    }
    
    static async Task CustomInstantiation() {
#if GLTFAST_4_OR_NEWER
        var gltf = new GltfImport();
#else
        var gltf = new GLTFast();
#endif
        var success = await gltf.Load("file:///path/to/file.gltf");

        if (success) {
            // Get the first material
            var material = gltf.GetMaterial();
            Debug.LogFormat("The first material is called {0}", material.name);

            // Instantiate the scene multiple times
#if GLTFAST_5_OR_NEWER
            await gltf.InstantiateMainSceneAsync(new GameObject("Instance 1").transform);
            await gltf.InstantiateMainSceneAsync(new GameObject("Instance 2").transform);
            await gltf.InstantiateMainSceneAsync(new GameObject("Instance 3").transform);

            // Create custom instantiation settings
            var settings = new InstantiationSettings {
                Layer = 13,
                LightIntensityFactor = 1000,
                Mask = ComponentType.Mesh | ComponentType.Animation,
                SceneObjectCreation = SceneObjectCreation.Never,
                SkinUpdateWhenOffscreen = false
            };
            
            // Feed settings into instantiator
            var instantiator = new GameObjectInstantiator(gltf, new GameObject("CustomizedInstance").transform, settings: settings);
            
            // Start instantiaton
            await gltf.InstantiateMainSceneAsync(instantiator);
            
#elif GLTFAST_4_OR_NEWER
            gltf.InstantiateMainScene(new GameObject("Instance 1").transform);
            gltf.InstantiateMainScene(new GameObject("Instance 2").transform);
            gltf.InstantiateMainScene(new GameObject("Instance 3").transform);
#else
            gltf.InstantiateGltf(new GameObject("Instance 1").transform);
            gltf.InstantiateGltf(new GameObject("Instance 2").transform);
            gltf.InstantiateGltf(new GameObject("Instance 3").transform);
#endif
        }
        else {
            Debug.LogError("Loading glTF failed!");
        }
    }

    async Task CustomDeferAgent() {
        // Recommended: Use a common defer agent across multiple GLTFast instances!
        
        // For a stable frame rate:
        IDeferAgent deferAgent = gameObject.AddComponent<TimeBudgetPerFrameDeferAgent>();
        // Or for faster loading:
        deferAgent = new UninterruptedDeferAgent();

        var tasks = new List<Task>();
        
        foreach( var url in manyUrls) {
#if GLTFAST_4_OR_NEWER
            var gltf = new GltfImport(null,deferAgent);
#else
            var gltf = new GLTFast.GLTFast(null,deferAgent);
#endif
            var task = gltf.Load(url).ContinueWith(
#if GLTFAST_5_OR_NEWER
                async
#endif
                t => {
                    if (t.Result) {
#if GLTFAST_4_OR_NEWER
#if GLTFAST_5_OR_NEWER
                        await gltf.InstantiateMainSceneAsync(transform);
#else
                        gltf.InstantiateMainScene(transform);
#endif
                        for (int sceneId = 0; sceneId < gltf.SceneCount; sceneId++) {
#if GLTFAST_5_OR_NEWER
                            await gltf.InstantiateSceneAsync(transform, sceneId);
#else
                            gltf.InstantiateScene(transform, sceneId);
#endif
                        }
#else
                        gltf.InstantiateGltf(transform);
#endif
                    }
                },
                TaskScheduler.FromCurrentSynchronizationContext()
                );
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }
    
    async Task InstanceAccess() 
    {
        var gltfImport = new GltfImport();
        await gltfImport.Load("test.gltf");
        var instantiator = new GameObjectInstantiator(gltfImport,transform);
        var success = await gltfImport.InstantiateMainSceneAsync(instantiator);
        if (success) {
        
            // Get the SceneInstance to access the instance's properties
            var sceneInstance = instantiator.SceneInstance;

            // Enable the first imported camera (which are disabled by default)
            if (sceneInstance.Cameras is { Count: > 0 }) {
                sceneInstance.Cameras[0].enabled = true;
            }
        
            // Decrease lights' ranges
            if (sceneInstance.Lights != null) {
                foreach (var glTFLight in sceneInstance.Lights) {
                    glTFLight.range *= 0.1f;
                }
            }
        
            // Play the default (i.e. the first) animation clip
            var legacyAnimation = instantiator.SceneInstance.LegacyAnimation;
            if (legacyAnimation != null) {
                legacyAnimation.Play();
            }
        }
    }
}
