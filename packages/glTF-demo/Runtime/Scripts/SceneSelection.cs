// Copyright (c) 2020-2021 Andreas Atteneder, All Rights Reserved.

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelection : MonoBehaviour
{
    [SerializeField] string[] scenes = null;
    [SerializeField] float width = 300;
    [SerializeField] float height = 100;
    [SerializeField] float yGap = 10;
    [SerializeField] bool alignRight = false;
    [SerializeField] bool alignBottom = false;

    bool hidden;

    void Start() {
#if PLATFORM_WEBGL && !UNITY_EDITOR
        // Hide UI in glTF compare web
        Destroy(this);
#else
        if(scenes!=null && scenes.Length>0) {
            var scene = SceneManager.GetActiveScene();
            hidden = scene.name!=scenes[0];
        } else {
            enabled = false;
        }
#endif
    }

    void OnGUI() {
        float x = alignRight ? Screen.width-width : 0;
        float y = alignBottom ? Screen.height - (1+(hidden?0:scenes.Length))*(yGap+height) : yGap;

        if(!hidden) {
            foreach(var scene in scenes) {
                if( GUI.Button( new Rect(x,y,width,height),scene)) {
                    SceneManager.LoadScene(scene,LoadSceneMode.Single);
                }
                y += height + yGap;
            }
        }
        if( GUI.Button( new Rect(x,y,width,height),hidden?"Menu":"Hide")) {
            hidden = !hidden;
        }
    }
}
