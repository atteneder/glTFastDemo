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

#if UNITY_GLTF

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Runtime.ExceptionServices;
using UnityGLTF;
using UnityGLTF.Loader;

/// <summary>
/// This is a copy of UnityGLTF.GLTFComponent with the onLoadComplete event exposed.
/// </summary>
public class UnityGLTFLoader : MonoBehaviour
{
    public string GLTFUri;
    public bool UseStream = false;

    public int MaximumLod = 300;
    public GLTFSceneImporter.ColliderType Colliders = GLTFSceneImporter.ColliderType.None;

	public UnityAction onLoadComplete;

    IEnumerator Start()
    {
        GLTFSceneImporter sceneImporter = null;
        IDataLoader loader = null;

        if (UseStream)
        {
            // Path.Combine treats paths that start with the separator character
            // as absolute paths, ignoring the first path passed in. This removes
            // that character to properly handle a filename written with it.
            GLTFUri = GLTFUri.TrimStart(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            string fullPath = Path.Combine(Application.streamingAssetsPath, GLTFUri);
            string directoryPath = URIHelper.GetDirectoryName(fullPath);
            loader = new FileLoader(directoryPath);
            sceneImporter = new GLTFSceneImporter(
                Path.GetFileName(GLTFUri),
                new ImportOptions() { DataLoader = loader }
                );
        }
        else
        {
            string directoryPath = URIHelper.GetDirectoryName(GLTFUri);
            loader = new WebRequestLoader(directoryPath);
            sceneImporter = new GLTFSceneImporter(
                URIHelper.GetFileFromUri(new Uri(GLTFUri)),
                new ImportOptions() { DataLoader = loader }
                );

        }

        sceneImporter.SceneParent = gameObject.transform;
        sceneImporter.Collider = Colliders;
        sceneImporter.MaximumLod = MaximumLod;
        yield return sceneImporter.LoadScene(-1, true, HandleAction);
    }

    void HandleAction(GameObject obj, ExceptionDispatchInfo exceptionDispatchInfo)
    {
        if(onLoadComplete!=null) {
            onLoadComplete();
        }
    }
}

#endif