using System.Collections;
using System.Collections.Generic;
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
            dls[i] = x.RequestTexture(urls[i]);
        }

        for (int i = 0; i < urls.Length; i++)
        {
            var ad = dls[i];
            yield return ad;
            results[i] = ad.texture;
        }
    }
}
