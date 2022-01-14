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
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GltfDemo.Editor {

    public static class BuildScripts {
        
        [MenuItem("glTF Demo/Build WebGL")]
        public static void BuildWebPlayer() {

            // var scenes = EditorBuildSettings.scenes;
            // var scenePaths = new string[scenes.Length];
            // for (var i = 0; i < scenes.Length; i++) {
            //     scenePaths[i] = scenes[i].path;
            // }

            var scenePaths = new[] {
                "Packages/com.atteneder.gltf-demo/Runtime/Scenes/TestLoadScene.unity"
            };

            string buildPath = null;
            var args = System.Environment.GetCommandLineArgs ();
            for (var i = 0; i < args.Length; i++) {
                if (args[i] == "-buildPath" && i<=args.Length) {
                    buildPath = args[i + 1];
                    break;
                }
            }
            
            if (buildPath == null) {
                buildPath = EditorUtility.SaveFolderPanel(
                    "glTF demo build path",
                    null,
                    $"glTFast"
                );
            }
            
            if (string.IsNullOrEmpty(buildPath)) {
                return;
            }
            else {
                Debug.Log($"Building at build path {buildPath}");
            }
            
            var buildPlayerOptions = new BuildPlayerOptions {
                scenes = scenePaths,
                locationPathName = buildPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            switch (summary.result) {
                case BuildResult.Succeeded:
                    Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                    break;
                case BuildResult.Failed:
                    Debug.LogError("Build failed");
                    break;
                case BuildResult.Unknown:
                    Debug.LogError("Build result unknown");
                    break;
                case BuildResult.Cancelled:
                    Debug.Log("Build canceled");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
}
