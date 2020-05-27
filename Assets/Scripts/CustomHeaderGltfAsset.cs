using UnityEngine;
using GLTFast;
using GLTFast.Loading;

public class CustomHeaderGltfAsset : MonoBehaviour
{

    public HttpHeader[] headers;

    CustomHeaderDownloadProvider downloadProvider;

    void Start() {
        downloadProvider = new CustomHeaderDownloadProvider(headers);
        var gltf = GetComponent<GltfAsset>();
        gltf.Load(gltf.url,downloadProvider);
    }
}
