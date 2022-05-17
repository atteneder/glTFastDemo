using System;
using System.IO;
using UnityEngine;
#if UNITY_GLTF
using UnityGLTF;
using UnityGLTF.Timeline;
#endif

public class RecordGui : MonoBehaviour
{
    public ExportScreenshot fileExport;
    private Transform root;

    public void SetSceneRoot(Transform root)
    {
        this.root = root;
    }
    
    #if UNITY_GLTF && UNITY_IMGUI
    
    private void OnGUI()
    {
        var w = Screen.width;
        var h = Screen.height;

        if ((recorder == null || !recorder.IsRecording) && GUI.Button(new Rect(w - 150, h - 100, 150, 20), "Start Recording"))
        {
            StartRecording();
        }
        if ((recorder != null && recorder.IsRecording) && GUI.Button(new Rect(w - 150, h - 100, 150, 20), "Stop & Download"))
        {
            StopRecordingAndDownloadAsGlb();
        }
        if (GUI.Button(new Rect(w - 150, h - 100 + 20, 150, 20), "Export"))
        {
            DownloadAsGlb();
        }
        if (GUI.Button(new Rect(w - 150, h - 100 + 40, 150, 20), "Export with Animation"))
        {
            RecordAllAnimationAndDownloadAsGlb();
        }
    }

    private void RecordAllAnimationAndDownloadAsGlb()
    {
        var animation = root.GetComponentInChildren<Animation>();
        animation.Rewind();
        var length = animation.clip.length;
        recorder = new GLTFRecorder(root, true);
        for (var t = 0f; t <= length; t += 1 / 60f)
        {
            foreach (AnimationState state in animation)
                state.time = t;
            
            animation.Sample();
            
            if(t == 0) 
                recorder.StartRecording(0);
            else
                recorder.UpdateRecording(t);
        }
        StopRecordingAndDownloadAsGlb();
    }

    private void DownloadAsGlb()
    {
        var previousExportDisabledState = GLTFSceneExporter.ExportDisabledGameObjects;
        var previousExportAnimationState = GLTFSceneExporter.ExportAnimations;
        GLTFSceneExporter.ExportDisabledGameObjects = true;
        GLTFSceneExporter.ExportAnimations = false;

        // assume root is a scene (empty node): we actually want to export it's children as a new scene.
        var roots = new Transform[root.childCount];
        for (int i = 0; i < root.childCount; i++)
            roots[i] = root.GetChild(i);
        var exporter = new GLTFSceneExporter(roots, new ExportOptions()
        {
            ExportInactivePrimitives = true,
        });
            
        var data = exporter.SaveGLBToByteArray(root.name);

        GLTFSceneExporter.ExportDisabledGameObjects = previousExportDisabledState;
        GLTFSceneExporter.ExportAnimations = previousExportAnimationState;
        
        fileExport.DownloadGLB("todo.glb", data);
    }

    private void StopRecordingAndDownloadAsGlb()
    {
        using (var stream = new MemoryStream())
        {
            recorder.EndRecording(stream);
            stream.Flush();
            stream.Capacity = (int) stream.Length;
            fileExport.DownloadGLB("todo.glb", stream.GetBuffer());
        }
    }

    private GLTFRecorder recorder;
    
    private void StartRecording()
    {
        recorder = new GLTFRecorder(root, true);
        recorder.StartRecording(Time.realtimeSinceStartupAsDouble);
    }

    private void LateUpdate()
    {
        if(recorder != null && recorder.IsRecording)
            recorder.UpdateRecording(Time.realtimeSinceStartupAsDouble);
    }
    
    #endif
}
