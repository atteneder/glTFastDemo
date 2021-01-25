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

using System;
using System.Collections;
using UnityEngine;
using GLTFast.Loading;

public class TestDownloads : MonoBehaviour
{
    public string[] urls;

    public Texture2D[] results;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        var headers = new []{
            new HttpHeader(){Key="key",Value="value"}
        };
        var x = new CustomHeaderDownloadProvider(headers);

        var dls = new ITextureDownload[urls.Length];
        results = new Texture2D[urls.Length];

        for (int i = 0; i < urls.Length; i++)
        {
            dls[i] = x.RequestTexture(new Uri(urls[i]));
        }

        for (int i = 0; i < urls.Length; i++)
        {
            var ad = dls[i];
            yield return ad;
            results[i] = ad.texture;
        }
    }
}
