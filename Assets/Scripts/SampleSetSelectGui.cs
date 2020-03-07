using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SampleSetSelectGui : MonoBehaviour
{
    [SerializeField]
    GltfSampleSet[] sampleSets = null;

    public UnityAction<GltfSampleSet> onSampleSetSelected;

    Vector2 scrollPos;

    void Awake() {
        StartCoroutine(InitGui());
    }

    IEnumerator InitGui() {

        var names = new List<string>();

        if(sampleSets!=null) {
            foreach(var set in sampleSets) {
                yield return set.Load();
            }
        }
    }

    private void OnGUI()
    {
        TestGui.TrySetGUIStyles();

        float width = Screen.width;
        float height = Screen.height;

        float listItemWidth = TestGui.listWidth-16;
        scrollPos = GUI.BeginScrollView(
            new Rect(0,TestGui.barHeightWidth,TestGui.listWidth,height-TestGui.barHeightWidth),
            scrollPos,
            new Rect(0,0,listItemWidth, TestGui.listItemHeight*sampleSets.Length)
        );


        float y = 0;
        foreach( var set in sampleSets ) {
            if(GUI.Button(new Rect(0,y,listItemWidth,TestGui.listItemHeight),set.name)) {
                // Hide menu during loading, since it can distort the performance profiling.
                this.enabled = false;
                onSampleSetSelected(set);
            }
            y+=TestGui.listItemHeight;
        }

        GUI.EndScrollView();
    }
}
