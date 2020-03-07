using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MassLoader : MonoBehaviour
{
    [SerializeField]
    protected bool local = true;

    [SerializeField]
    protected StopWatch stopWatch = null;

    void Start()
    {
        var selectSet = GetComponent<SampleSetSelectGui>();
        selectSet.onSampleSetSelected += OnSampleSetSelected;
    }

    void OnSampleSetSelected(GltfSampleSet sampleSet) {
        StartCoroutine(MassLoadRoutine(sampleSet));
    }

    protected abstract IEnumerator MassLoadRoutine(GltfSampleSet sampleSet);
}
