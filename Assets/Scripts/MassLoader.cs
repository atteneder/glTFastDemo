#if !(UNITY_ANDROID || UNITY_WEBGL) || UNITY_EDITOR
#define LOCAL_LOADING
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassLoader : MonoBehaviour {

    public bool local = false;

	// Use this for initialization
	IEnumerator Start () {

        yield return GltfSampleModels.LoadGltfFileUrls();

        // Wait a bit to make sure profiling works
        yield return new WaitForSeconds(1);

        foreach( var n in GltfSampleModels.gltfFileUrls ) {
#if LOCAL_LOADING
            var url = string.Format( "file://{0}", n);
#else
            var ulr = n;
#endif
            var go = new GameObject(System.IO.Path.GetFileNameWithoutExtension(url));

#if UNITY_GLTF
            var gltf = go.AddComponent<UnityGLTF.GLTFComponent>();
            gltf.GLTFUri = url;
#endif
            
#if !NO_GLTFAST
            var gltfAsset = go.AddComponent<GLTFast.GltfAsset>();
            gltfAsset.url = url;
#endif
        }
	}
}
