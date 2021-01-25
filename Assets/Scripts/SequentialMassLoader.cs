// Copyright 2020-2021 Andreas Atteneder
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

#if !(UNITY_ANDROID || UNITY_WEBGL) || UNITY_EDITOR
#define LOCAL_LOADING
#endif

using System.Collections;
using UnityEngine;
using GLTFast.Tests;

public class SequentialMassLoader : MassLoader
{
    bool waitForIt;
    GLTFast.GltfAsset gltfAsset;

    protected override IEnumerator MassLoadRoutine(GltfSampleSet sampleSet) {

        stopWatch.StartTime();

        int count = 0;
        if(local) {
            foreach(var item in sampleSet.GetItemsPrefixed()) {
                yield return LoadIt<GLTFast.GltfAsset>(
#if LOCAL_LOADING
                    string.Format( "file://{0}", item.path)
#else
                    item.path
#endif
                );
                count++;
            }
        } else {
            foreach(var item in sampleSet.GetItemsPrefixed(false)) {
                yield return LoadIt<GLTFast.GltfAsset>(item.path);
                count++;
            }
        }

        stopWatch.StopTime();
        Debug.LogFormat("Finished loading {1} glTFs in {0} milliseconds!",stopWatch.lastDuration,count);

        var selectSet = GetComponent<SampleSetSelectGui>();
        selectSet.enabled = true;
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

    void OnComplete(GLTFast.GltfAssetBase asset, bool success) {
        asset.onLoadComplete -= OnComplete;
        if(!success) {
            Debug.LogError("Ups");
            Destroy(this);
        } else {
            waitForIt = false;
        }
    }
}
