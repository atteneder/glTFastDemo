using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SampleSetSelectGui : MonoBehaviour
{
    [SerializeField]
    GltfSampleSetCollection sampleSetCollection = null;

    public UnityAction<GltfSampleSet> onSampleSetSelected;

    Vector2 scrollPos;

    void Awake() {
        StartCoroutine(InitGui());
    }

    IEnumerator InitGui() {
        if(sampleSetCollection!=null) {
            yield return sampleSetCollection.LoadAll();
        }
    }

    private void OnGUI()
    {
        GlobalGui.Init();
        float width = Screen.width;
        float height = Screen.height;

        float listItemWidth = GlobalGui.listWidth-16;
        scrollPos = GUI.BeginScrollView(
            new Rect(0,GlobalGui.barHeightWidth,GlobalGui.listWidth,height-GlobalGui.barHeightWidth),
            scrollPos,
            new Rect(0,0,listItemWidth, GlobalGui.listItemHeight*sampleSetCollection.sampleSets.Length)
        );


        float y = 0;
        foreach( var set in sampleSetCollection.sampleSets ) {
            if(GUI.Button(new Rect(0,y,listItemWidth,GlobalGui.listItemHeight),set.name)) {
                // Hide menu during loading, since it can distort the performance profiling.
                this.enabled = false;
                onSampleSetSelected(set);
            }
            y+=GlobalGui.listItemHeight;
        }

        GUI.EndScrollView();
    }
}
