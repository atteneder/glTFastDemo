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

#if !(UNITY_ANDROID || UNITY_WEBGL) || UNITY_EDITOR
#define LOCAL_LOADING
#endif

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GLTFTest.Sample;

public class SimultaneousMassLoader : MassLoader {

    public enum Strategy {
        Fast,
        Responsive
    }

    [SerializeField] Strategy strategy = Strategy.Responsive;
    [SerializeField] int numVisibleAssets = 1;

    Queue<GLTFast.GltfAssetBase> visibleAssets = new Queue<GLTFast.GltfAssetBase>();

    protected override async void MassLoadRoutine (SampleSet sampleSet) {

        stopWatch.StartTime();

        GLTFast.IDeferAgent deferAgent;
        if(strategy==Strategy.Fast) {
            deferAgent = new GLTFast.UninterruptedDeferAgent();
        } else {
            deferAgent = gameObject.AddComponent<GLTFast.TimeBudgetPerFrameDeferAgent>();
        }

        var loadTasks = new List<Task>(sampleSet.itemCount);

        if(local) {
            foreach(var item in sampleSet.GetItemsPrefixed()) {
                var loadTask = LoadIt(
#if LOCAL_LOADING
                    string.Format( "file://{0}", item.path)
#else
                    item.path
#endif
                    ,deferAgent
                );
                loadTasks.Add(loadTask);
                await deferAgent.BreakPoint();
            }
        } else {
            foreach(var item in sampleSet.GetItemsPrefixed(false)) {
                var loadTask = LoadIt(item.path,deferAgent);
                loadTasks.Add(loadTask);
                await deferAgent.BreakPoint();
            }
        }

        await Task.WhenAll(loadTasks);
        
        stopWatch.StopTime();
        Debug.LogFormat("Finished loading {1} glTFs in {0} milliseconds!",stopWatch.lastDuration,sampleSet.itemCount);

        var selectSet = GetComponent<SampleSetSelectGui>();
        selectSet.enabled = true;
    }

    async Task LoadIt(string n, GLTFast.IDeferAgent deferAgent) {
        var url = n;
        var go = new GameObject(System.IO.Path.GetFileNameWithoutExtension(url));
        // Debug.Log(go.name);
        var gltfAsset = go.AddComponent<GLTFast.GltfAsset>();
        gltfAsset.LoadOnStartup = false; // prevent auto-loading
        await gltfAsset.Load(url,null,deferAgent); // load manually with custom defer agent
        if(visibleAssets.Count>=numVisibleAssets) {
            var oldAsset = visibleAssets.Dequeue();
            // oldAsset.gameObject.SetActive(false);
            Destroy(oldAsset.gameObject);
        }
        visibleAssets.Enqueue(gltfAsset);
    }
}
