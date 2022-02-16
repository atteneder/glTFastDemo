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

using System;
using UnityEngine;
using GLTFTest;

[RequireComponent(typeof(StopWatch))]
public class StopWatchGui : MonoBehaviour {
    StopWatch m_StopWatch;
    
    public float posX;

    void Start() {
        m_StopWatch = gameObject.GetComponent<StopWatch>();
        if(m_StopWatch==null) Destroy(this);
    }
    
    void OnGUI() {
        if(m_StopWatch.active) {
            GlobalGui.Init();
            var duration = m_StopWatch.lastDuration;
            float width = Screen.width;
            float height = Screen.height;
            var frameCount = m_StopWatch.frameCount;
            var fpsString = (frameCount > 0)
            ? string.Format(
                " (fps avg: {0} min: {1} ms max: {2} ms)"
                ,m_StopWatch.averageFrameTime.ToString("0.00")
                ,m_StopWatch.minFrameTime < float.MaxValue ? m_StopWatch.minFrameTime.ToString("0.00") : "-"
                ,m_StopWatch.maxFrameTime > float.MinValue ? m_StopWatch.maxFrameTime.ToString("0.00") : "-"
            )
            : "";
            var label = string.Format(
                "glTFast time: {0:0.00} ms{1}"
                ,duration>=0 ? duration : m_StopWatch.now
                ,fpsString
                );

            var timeHeight = GUI.skin.label.fontSize*1.5f;
            GUI.Label(new Rect(posX+10,height-timeHeight,width-posX-10,timeHeight),label);
        }
    }
}
