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

using UnityEngine;

public class StopWatch : MonoBehaviour
{
    public float posX;

    float startTime = -1;
    float minFrame = float.MaxValue;
    float maxFrame = float.MinValue;
    float duration = -1;
    float fpsDuration;
    int startFrameCount;
    int endFrameCount;
    bool running;

    float now {
        get {
            return (Time.realtimeSinceStartup-startTime)*1000;
        }
    }

    public float lastDuration {
        get {
            return duration;
        }
    }

    public void StartTime() {
        startTime = Time.realtimeSinceStartup;
        duration = 0;
        fpsDuration = 0;
        minFrame = float.MaxValue;
        maxFrame = float.MinValue;
        startFrameCount = Time.frameCount;
        endFrameCount = startFrameCount;
        running = true;
    }

    public void StopTime() {
        running = false;
        var delta = Time.deltaTime * 1000;
        UpdateFrameTimes(delta);
    }

    void Update() {
        if(running) {
            var delta = Time.deltaTime * 1000;
            fpsDuration += delta;
            UpdateFrameTimes(delta);
        }
    }

    void OnGUI() {
        GlobalGui.Init();
        if(running || duration>=0) {
            float width = Screen.width;
            float height = Screen.height;
            var frameCount = endFrameCount-startFrameCount;
            var fpsString = (frameCount > 0)
            ? string.Format(
                " (fps avg: {0} min: {1} ms max: {2} ms)"
                ,(fpsDuration/(float)frameCount).ToString("0.00")
                ,minFrame < float.MaxValue ? minFrame.ToString("0.00") : "-"
                ,maxFrame > float.MinValue ? maxFrame.ToString("0.00") : "-"
            )
            : "";
            var label = string.Format(
                "glTFast time: {0:0.00} ms{1}"
                ,duration>=0 ? duration : now
                ,fpsString
                );

            var timeHeight = GUI.skin.label.fontSize*1.5f;
            GUI.Label(new Rect(posX+10,height-timeHeight,width-posX-10,timeHeight),label);
        }
    }

    void UpdateFrameTimes(float delta) {
        duration = now;
        endFrameCount = Time.frameCount;
        if(endFrameCount > startFrameCount) {
            minFrame = Mathf.Min(minFrame, delta );
            maxFrame = Mathf.Max(maxFrame, delta );
        }
    }
}
