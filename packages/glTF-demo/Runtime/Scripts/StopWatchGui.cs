// SPDX-FileCopyrightText: 2020 Andreas Atteneder
// SPDX-License-Identifier: Apache-2.0

#if UNITY_IMGUI && (UNITY_EDITOR || !UNITY_WEBGL)
#define GUI_ENABLED
#endif

using UnityEngine;
using GLTFTest;
using UnityEngine.Events;

[RequireComponent(typeof(StopWatch))]
public class StopWatchGui : MonoBehaviour {

    public float posX;

    StopWatch m_StopWatch;
    string title = "glTFast";
    private StopWatchState m_State;
    public UnityAction<StopWatchState> OnStateChange;

    void Start() {
        m_StopWatch = gameObject.GetComponent<StopWatch>();
        if(m_StopWatch==null) Destroy(this);
    }

    void Update()
    {
        if (!m_StopWatch.active) return;
        var duration = m_StopWatch.lastDuration;
        var current = new StopWatchState
        {
            duration = duration >= 0 ? duration : m_StopWatch.now,
            frameCount = m_StopWatch.frameCount,
            averageFrameTime = m_StopWatch.averageFrameTime,
            maxFrameTime = m_StopWatch.maxFrameTime,
            minFrameTime = m_StopWatch.minFrameTime,
        };

        if (!m_State.Equals(current))
        {
            m_State = current;
            OnStateChange?.Invoke(m_State);
#if GUI_ENABLED
            UpdateGuiString();
#endif
        }
    }
    
    public void SetTitle(string newTitle)
    {
        title = newTitle;
    }

#if GUI_ENABLED
    private string m_Label;

    void UpdateGuiString()
    {
        var frameCount = m_State.frameCount;
        var fpsString = frameCount > 0
            ? $" (fps avg: {m_StopWatch.averageFrameTime:0.00} min: {(m_StopWatch.minFrameTime < float.MaxValue ? m_StopWatch.minFrameTime.ToString("0.00") : "-")} ms max: {(m_StopWatch.maxFrameTime > float.MinValue ? m_StopWatch.maxFrameTime.ToString("0.00") : "-")} ms)"
            : "";
        m_Label = $"{title} time: {m_State.duration:0.00} ms{fpsString}";
    }

    void OnGUI()
    {
        if (m_Label == null) return;
        GlobalGui.Init();
        float width = Screen.width;
        float height = Screen.height;
        var timeHeight = GUI.skin.label.fontSize*1.5f;
        GUI.Label(new Rect(posX+10, height-timeHeight,width-posX-10, timeHeight), m_Label);
    }
#endif // GUI_ENABLED
}
