#if !(UNITY_ANDROID || UNITY_WEBGL) || UNITY_EDITOR
#define LOCAL_LOADING
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimultaneousMassLoader : MassLoader {

    public enum Strategy {
        Fast,
        Responsive
    }

    [SerializeField] Strategy strategy = Strategy.Responsive;
    [SerializeField] int numVisibleAssets = 1;

    int loadedCount;

    Queue<GLTFast.GltfAssetBase> visibleAssets = new Queue<GLTFast.GltfAssetBase>();

    protected override IEnumerator MassLoadRoutine (GltfSampleSet sampleSet) {

        stopWatch.StartTime();

        int count = 0;
        loadedCount = 0;

        GLTFast.IDeferAgent deferAgent;
        if(strategy==Strategy.Fast) {
            deferAgent = new GLTFast.UninterruptedDeferAgent();
        } else {
            deferAgent = gameObject.AddComponent<GLTFast.TimeBudgetPerFrameDeferAgent>();
        }

        
        if(local) {
            foreach(var item in sampleSet.GetItemsPrefixed()) {
                LoadIt(
#if LOCAL_LOADING
                    string.Format( "file://{0}", item.path)
#else
                    item.path
#endif
                    ,deferAgent
                );
                count++;
                if(deferAgent.ShouldDefer()) {
                    yield return null;
                }
            }
        } else {
            foreach(var item in sampleSet.GetItemsPrefixed(false)) {
                LoadIt(item.path,deferAgent);
                count++;
                if(deferAgent.ShouldDefer()) {
                    yield return null;
                }
            }
        }

        while(loadedCount<count) {
            yield return null;
        }

        stopWatch.StopTime();
        Debug.LogFormat("Finished loading {1} glTFs in {0} milliseconds!",stopWatch.lastDuration,count);

        var selectSet = GetComponent<SampleSetSelectGui>();
        selectSet.enabled = true;
    }

    void LoadIt(string n, GLTFast.IDeferAgent deferAgent) {
        var url = n;
        var go = new GameObject(System.IO.Path.GetFileNameWithoutExtension(url));
        // Debug.Log(go.name);
        var gltfAsset = go.AddComponent<GLTFast.GltfAsset>();
        gltfAsset.loadOnStartup = false; // prevent auto-loading
        gltfAsset.onLoadComplete += OnComplete;
        gltfAsset.Load(url,null,deferAgent); // load manually with custom defer agent
    }

    void OnComplete(GLTFast.GltfAssetBase asset, bool success) {
        if(visibleAssets.Count>=numVisibleAssets) {
            var oldAsset = visibleAssets.Dequeue();
            // oldAsset.gameObject.SetActive(false);
            Destroy(oldAsset.gameObject);
        }
        visibleAssets.Enqueue(asset);
        loadedCount++;
    }
}
