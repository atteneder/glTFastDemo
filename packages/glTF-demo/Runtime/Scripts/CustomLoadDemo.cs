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
            generateMipMaps = true,
            anisotropicFilterLevel = 3,
            nodeNameMethod = ImportSettings.NameImportMethod.OriginalUnique
        };
        // Load the glTF and pass along the settings
        var success = await gltf.Load("file:///path/to/file.gltf", settings);

        if (success) {
#if GLTFAST_5_OR_NEWER
            await
#endif
            gltf.InstantiateMainScene(new GameObject("glTF").transform);
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
            await gltf.InstantiateMainScene(new GameObject("Instance 1").transform);
            await gltf.InstantiateMainScene(new GameObject("Instance 2").transform);
            await gltf.InstantiateMainScene(new GameObject("Instance 3").transform);

            // Create custom instantiation settings
            var settings = new InstantiationSettings {
                layer = 13,
                lightIntensityFactor = 1000,
                mask = ComponentType.Mesh | ComponentType.Animation,
                sceneObjectCreation = InstantiationSettings.SceneObjectCreation.Never,
                skinUpdateWhenOffscreen = false
            };
            
            // Feed settings into instantiator
            var instantiator = new GameObjectInstantiator(gltf, new GameObject("CustomizedInstance").transform, settings: settings);
            
            // Start instantiaton
            await gltf.InstantiateMainScene(instantiator);
            
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
            var gltf = new GLTFast.GltfImport(null,deferAgent);
#else
            var gltf = new GLTFast.GLTFast(null,deferAgent);
#endif
            var task = gltf.Load(url).ContinueWith(
                t => {
                    if (t.Result) {
#if GLTFAST_4_OR_NEWER
                        gltf.InstantiateMainScene(transform);
                        for (int sceneId = 0; sceneId < gltf.sceneCount; sceneId++) {
                            gltf.InstantiateScene(transform, sceneId);
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
}
