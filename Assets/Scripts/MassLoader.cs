using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MassLoader : MonoBehaviour
{
    [SerializeField]
    protected GltfSampleSet[] sampleSets = null;

    [SerializeField]
    protected bool local = true;

    [SerializeField]
    protected StopWatch stopWatch = null;

    IEnumerator Start()
    {
        if(sampleSets!=null) {
            foreach(var set in sampleSets) {
                yield return set.Load();
            }
        }
        // Wait a bit to make sure profiling works
        yield return new WaitForSeconds(1);
        StartCoroutine(MassLoadRoutine());
    }

    protected abstract IEnumerator MassLoadRoutine();
}
