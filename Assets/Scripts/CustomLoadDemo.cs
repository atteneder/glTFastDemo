using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

public class CustomLoadDemo : MonoBehaviour {
    
    public string[] manyUrls;
    
    // Start is called before the first frame update
    async void Start() {
        await CustomInstantiation();
        await CustomDeferAgent();
    }

    static async Task CustomInstantiation() {
        var gltf = new GLTFast.GltfImport();
        var success = await gltf.Load("file:///path/to/file.gltf");

        if (success) {
            // Get the first material
            var material = gltf.GetMaterial();
            Debug.LogFormat("The first material is called {0}", material.name);

            // Instantiate the scene multiple times
            gltf.InstantiateMainScene(new GameObject("Instance 1").transform);
            gltf.InstantiateMainScene(new GameObject("Instance 2").transform);
            gltf.InstantiateMainScene(new GameObject("Instance 3").transform);
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
            var gltf = new GLTFast.GltfImport(null,deferAgent);
            var task = gltf.Load(url).ContinueWith(
                t => {
                    if (t.Result) {
                        gltf.InstantiateMainScene(transform);
                    }
                },
                TaskScheduler.FromCurrentSynchronizationContext()
                );
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }
}
