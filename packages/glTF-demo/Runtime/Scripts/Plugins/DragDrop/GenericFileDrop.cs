using System.Collections.Generic;
using UnityEngine;
using B83.Win32;
using UnityEngine.Events;

public class GenericFileDrop : MonoBehaviour
{
    public UnityEvent<string> OnGotFile;
    
    void OnEnable ()
    {
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }

    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }
    
    class DropInfo
    {
        public string file;
        public Vector2 pos;
    }
    
    private void OnFiles(List<string> paths, POINT position)
    {
        string file = null;
        
        // scan through dropped files and filter out supported types
        foreach (var f in paths)
        {
            Debug.Log("file dropped: " + f);
            var fi = new System.IO.FileInfo(f);
            var ext = fi.Extension.ToLower();
            if (ext == ".glb" || ext == ".gltf")
            {
                file = f;
                break;
            }
        }
        
        // If the user dropped a supported file, create a DropInfo
        if (!string.IsNullOrEmpty(file))
        {
            GotSupportedFile(new DropInfo
            {
                file = file,
                pos = new Vector2(position.x, position.y)
            });
        }
    }

    private void GotSupportedFile(DropInfo dropInfo)
    {
        Debug.Log("supported file found: " + dropInfo.file + " at " + dropInfo.pos);
        OnGotFile?.Invoke(dropInfo.file);
    }
}
