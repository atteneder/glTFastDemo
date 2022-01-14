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

using UnityEngine;
using UnityEngine.Events;
using GLTFTest.Sample;

public class SampleSetSelectGui : MonoBehaviour
{
    [SerializeField]
    SampleSetCollection sampleSetCollection = null;

    public UnityAction<SampleSet> onSampleSetSelected;

    Vector2 scrollPos;

    void Awake() {
#if PLATFORM_WEBGL && !UNITY_EDITOR
        // Hide UI in glTF compare web
        Destroy(this);
        return;
#endif
    }

    private void OnGUI()
    {
        GlobalGui.Init();
        float width = Screen.width;
        float height = Screen.height;

        float listItemWidth = GlobalGui.listWidth-16;
        scrollPos = GUI.BeginScrollView(
            new Rect(0,GlobalGui.barHeightWidth,GlobalGui.listWidth,height-GlobalGui.barHeightWidth),
            scrollPos,
            new Rect(0,0,listItemWidth, GlobalGui.listItemHeight*sampleSetCollection.sampleSets.Length)
        );


        float y = 0;
        foreach( var set in sampleSetCollection.sampleSets ) {
            if(GUI.Button(new Rect(0,y,listItemWidth,GlobalGui.listItemHeight),set.name)) {
                // Hide menu during loading, since it can distort the performance profiling.
                this.enabled = false;
                onSampleSetSelected(set);
            }
            y+=GlobalGui.listItemHeight;
        }

        GUI.EndScrollView();
    }
}
