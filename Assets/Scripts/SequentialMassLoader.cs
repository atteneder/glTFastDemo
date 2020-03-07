﻿#if !(UNITY_ANDROID || UNITY_WEBGL) || UNITY_EDITOR
#define LOCAL_LOADING
#endif

using System.Collections;
using UnityEngine;

public class SequentialMassLoader : MassLoader
{
    bool waitForIt;
    GLTFast.GltfAsset gltfAsset;

    protected override IEnumerator MassLoadRoutine() {

        stopWatch.StartTime();

        int count = 0;
        if(sampleSets!=null) {
            foreach(var set in sampleSets) {
                if(set.items!=null) {
                    if(local) {
                        foreach(var item in set.itemsLocal) {
                            yield return LoadIt<GLTFast.GltfAsset>(
#if LOCAL_LOADING
                                string.Format( "file://{0}", item.Item2)
#else
                                item.Item2
#endif
                            );
                            count++;
                        }
                    } else {
                        foreach(var item in set.items) {
                            yield return LoadIt<GLTFast.GltfAsset>(item.Item2);
                            count++;
                        }
                    }
                }
            }
        }

        stopWatch.StopTime();
        Debug.LogFormat("Finished loading {1} glTFs in {0} milliseconds!",stopWatch.lastDuration,count);
    }

    IEnumerator LoadIt<T>(string n) where T:GLTFast.GltfAsset {
        var url = n;
        var go = new GameObject(System.IO.Path.GetFileNameWithoutExtension(url));
        
        Debug.Log(go.name);

        waitForIt = true;

        gltfAsset = go.AddComponent<T>();
        gltfAsset.onLoadComplete += OnComplete;
        gltfAsset.url = url;

        while(waitForIt) {
            yield return null;
        }
        Destroy(go);
    }

    void OnComplete(GLTFast.GltfAsset asset, bool success) {
        asset.onLoadComplete -= OnComplete;
        if(!success) {
            Debug.LogError("Ups");
            Destroy(this);
        } else {
            waitForIt = false;
        }
    }
}