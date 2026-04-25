// SPDX-FileCopyrightText: 2020 Andreas Atteneder
// SPDX-License-Identifier: Apache-2.0

#if UNITY_IMGUI && (UNITY_EDITOR || !UNITY_WEBGL)
#define GUI_ENABLED
#endif

using UnityEngine;
using GLTFTest;

[RequireComponent(typeof(StopWatch))]
public class StopWatchGui : MonoBehaviour {
    
    StopWatch m_StopWatch;
    private StopWatchState m_State;

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
#if GUI_ENABLED
            UpdateGuiString();
#endif
        }
    }

#if GUI_ENABLED
    public float posX;
    string title = "glTFast";
    private string m_Label;

    void UpdateGuiString()
    {
        var duration = m_StopWatch.lastDuration;
        
        var frameCount = m_StopWatch.frameCount;
        var fpsString = (frameCount > 0)
            ? string.Format(
                " (fps avg: {0} min: {1} ms max: {2} ms)"
                ,m_StopWatch.averageFrameTime.ToString("0.00")
                ,m_StopWatch.minFrameTime < float.MaxValue ? m_StopWatch.minFrameTime.ToString("0.00") : "-"
                ,m_StopWatch.maxFrameTime > float.MinValue ? m_StopWatch.maxFrameTime.ToString("0.00") : "-"
            )
            : "";
        m_Label = string.Format(
            title + " time: {0:0.00} ms{1}"
            ,duration>=0 ? duration : m_StopWatch.now
            ,fpsString
        );
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

    public void SetTitle(string newTitle)
    {
        title = newTitle;
    }
#endif // GUI_ENABLED
}
