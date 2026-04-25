#if !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
#endif
using UnityEngine;

[RequireComponent(typeof(TestLoader))]
public class NativeInterface : MonoBehaviour {

    TestLoader m_TestLoader;
    StopWatchGui m_StopWatchGui;
    
    void Awake() {
        m_TestLoader = GetComponent<TestLoader>();
        m_TestLoader.loadingEnd += OnModelLoaded;
        m_StopWatchGui = GetComponent<StopWatchGui>();
        m_StopWatchGui.OnStateChange += OnStopWatchStateChange;
    }

    private void OnStopWatchStateChange(StopWatchState state)
    {
        StopWatchStateUpdate(
            state.duration,
            state.frameCount,
            state.averageFrameTime,
            state.maxFrameTime,
            state.minFrameTime
            );
    }

    void OnDestroy() {
        m_TestLoader.loadingEnd -= OnModelLoaded;
        m_StopWatchGui.OnStateChange -= OnStopWatchStateChange;
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    static extern void OnModelLoaded(bool success);

    [DllImport("__Internal")]
    static extern void StopWatchStateUpdate(
        float duration,
        int frameCount,
        float averageFrameTime,
        float maxFrameTime,
        float minFrameTime
    );
#else
    static void OnModelLoaded(bool success) {}

    static void StopWatchStateUpdate(
        float duration,
        int frameCount,
        float averageFrameTime,
        float maxFrameTime,
        float minFrameTime
        ) { }
#endif
}
