using System;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(TestLoader))]
public class NativeInterface : MonoBehaviour {

#if !UNITY_EDITOR
    TestLoader m_TestLoader;
    
    void Awake() {
        m_TestLoader = GetComponent<TestLoader>();
        m_TestLoader.loadingEnd += OnModelLoaded;
    }

    void OnDestroy() {
        m_TestLoader.loadingEnd -= OnModelLoaded;
    }

    [DllImport("__Internal")]
    static extern void OnModelLoaded(bool success);
#endif
}
