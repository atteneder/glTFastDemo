using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ExportScreenshot : MonoBehaviour {

    // from https://forum.unity.com/threads/user-image-download-from-in-webgl-app.474715/

    [DllImport("__Internal")]
    private static extern void ImageDownloader(string str, string fn, string contentType, bool noAutoClick);

    public static byte[] ssData = null;

    public Texture2D tex;

    void Log(object o)
    {
        if (Application.isEditor)
            Debug.Log(o);
    }

    public void DownloadScreenshot(string filename, Texture2D tex, bool noAutoClick)
    {
        var imageData = tex.EncodeToPNG();
        
        Log("Downloading PNG file: " + filename);
#if UNITY_EDITOR
        System.IO.File.WriteAllBytes(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "/" + filename, imageData);
#else
        var contentType = "image/png";
        ImageDownloader(System.Convert.ToBase64String(imageData), filename, contentType, noAutoClick);
#endif
    }

    public void DownloadJson(string filename, string json, bool noAutoClick)
    {
        var contentType = "application/json";

        Log("Downloading JSON file: " + filename);
        ImageDownloader(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json)), filename, contentType, noAutoClick);
    }

    public void DownloadGLB(string filename, byte[] glb, bool noAutoClick = false)
    {

        Log("Downloading GLB file: " + filename + ", " + glb.Length + " bytes");
#if UNITY_EDITOR
        System.IO.File.WriteAllBytes(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + filename, glb);
#else
        var contentType = "model/gltf-binary";
        ImageDownloader(System.Convert.ToBase64String(glb), filename, contentType, noAutoClick);
#endif
    }
}
