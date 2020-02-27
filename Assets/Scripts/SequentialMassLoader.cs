#if !(UNITY_ANDROID || UNITY_WEBGL) || UNITY_EDITOR
#define LOCAL_LOADING
#endif

using System.Collections;
using UnityEngine;

public class SequentialMassLoader : MonoBehaviour
{
    [SerializeField]
    GltfSampleSet[] sampleSets = null;

    [SerializeField]
    bool local = true;

    bool waitForIt;
    GLTFast.GltfAsset gltfAsset;

    IEnumerator Start()
    {
        if(sampleSets!=null) {
            foreach(var set in sampleSets) {
                yield return set.Load();
            }
        }

        // Wait a bit to make sure profiling works
        yield return new WaitForSeconds(1);

        float startTime = Time.realtimeSinceStartup;

        // foreach (var n in GltfSampleModels.gltfFileUrls)
        // {
        //     yield return LoadIt<GLTFast.GltfAsset>(n,baseUrl);
        // }

        // foreach (var n in GltfSampleModels.glbFileUrls)
        // {
        //     yield return LoadIt<GLTFast.GltfAsset>(n,baseUrl);
        // }

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
                        }
                        count++;
                    } else {
                        foreach(var item in set.items) {
                            yield return LoadIt<GLTFast.GltfAsset>(item.Item2);
                        }
                        count++;
                    }
                }
            }
        }


        Debug.LogFormat("Finished loading {1} glTFs in {0} seconds!",Time.realtimeSinceStartup-startTime,count);
    }

    IEnumerator LoadIt<T>(string n) where T:GLTFast.GltfAsset {
        // var url = string.Format(
        //     "{0}/{1}"
        //     , baseUrl
        //     , n
        //     );
        var url = n;
        var go = new GameObject(System.IO.Path.GetFileNameWithoutExtension(url));
        
        Debug.Log(go.name);

        waitForIt = true;

        // GLTFast.GLTFast.LoadGlbFile( url, go.transform );
        gltfAsset = go.AddComponent<T>();
        gltfAsset.onLoadComplete += OnComplete;
        gltfAsset.url = url;

        while(waitForIt) {
            yield return null;
        }
        Destroy(go);
        yield return null;
    }

    void OnComplete(bool success) {
        gltfAsset.onLoadComplete -= OnComplete;
        if(!success) {
            Debug.LogError("Ups");
            Destroy(this);
        } else {
            waitForIt = false;
        }
    }
}
