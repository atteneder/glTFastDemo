using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "glTF-SampleSet-collection", menuName = "ScriptableObjects/GltfSampleSetCollection", order = 2)]
public class GltfSampleSetCollection : ScriptableObject
{
    public GltfSampleSet[] sampleSets;

    public IEnumerator LoadAll() {
        if(sampleSets!=null) {
            foreach(var set in sampleSets) {
                yield return set.Load();
            }
        }
    }
}
