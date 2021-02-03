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

using System.Threading.Tasks;
using UnityEngine;
using GLTFast;
using GLTFast.Utils;
using GLTFast.Samples;

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

    void OnSampleSetSelected(SampleSet sampleSet) {
        MassLoadRoutine(sampleSet);
    }

    protected abstract void MassLoadRoutine(SampleSet sampleSet);
}
