#if !(UNITY_ANDROID || UNITY_WEBGL) || UNITY_EDITOR
#define LOCAL_LOADING
#endif

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(TestLoader))]
public class TestGui : MonoBehaviour {

    GltfSampleSet sampleSet = null;


    [SerializeField]
    StopWatch stopWatch = null;

    public bool showMenu = true;
    // Load files locally (from streaming assets) or via HTTP
    public bool local = false;

    List<GLTFast.Tuple<string,string>> testItems = new List<GLTFast.Tuple<string, string>>();
    List<GLTFast.Tuple<string,string>> testItemsLocal = new List<GLTFast.Tuple<string, string>>();

    string urlField;

    Vector2 scrollPos;

    private void Awake()
    {
        stopWatch.posX = GlobalGui.listWidth;

#if PLATFORM_WEBGL && !UNITY_EDITOR
        // Hide UI in glTF compare web
        HideUI();
#endif

        var selectSet = GetComponent<SampleSetSelectGui>();
        selectSet.onSampleSetSelected += OnSampleSetSelected;

        var tl = GetComponent<TestLoader>();
        tl.urlChanged += UrlChanged;
        tl.loadingBegin += OnLoadingBegin;
        tl.loadingEnd += OnLoadingEnd;
    }

    void OnLoadingBegin() {
        stopWatch.StartTime();
    }

    void OnLoadingEnd() {
        showMenu = true;
        stopWatch.StopTime();
    }

    void OnSampleSetSelected(GltfSampleSet newSet) {

        if(newSet==null || newSet.items==null) {
            Debug.LogError("Empty sample set!");
        }

        sampleSet = newSet;

#if LOCAL_LOADING
        foreach(var item in sampleSet.itemsLocal) {
            testItemsLocal.Add(
                new GLTFast.Tuple<string, string>(
                    item.Item1,
                    string.Format( "file://{0}", item.Item2)
                )
            );
        }
#else
        testItems.AddRange(set.itemsLocal);
#endif
    }

    void ResetSampleSet() {
        sampleSet = null;
        testItemsLocal.Clear();
        testItems.Clear();
        var selectSet = GetComponent<SampleSetSelectGui>();
        selectSet.enabled = true;
    }
    void UrlChanged(string newUrl)
    {
        stopWatch.StartTime();
        urlField = newUrl;
    }

    private void OnGUI()
    {
        float width = Screen.width;
        float height = Screen.height;

        if(showMenu && sampleSet!=null) {
            GUI.BeginGroup( new Rect(0,0,width,GlobalGui.barHeightWidth) );
            
            float urlFieldWidth = width-GlobalGui.buttonWidth;

#if UNITY_EDITOR
            if(GUI.Button( new Rect(width-GlobalGui.buttonWidth*2,0,GlobalGui.buttonWidth,GlobalGui.barHeightWidth),"Open")) {
                string path = EditorUtility.OpenFilePanel("Select glTF", "", "glb");
                if (path.Length != 0)
                {
                    GetComponent<TestLoader>().LoadUrl("file://"+path);
                }
            }
            urlFieldWidth -= GlobalGui.buttonWidth;
#endif

            urlField = GUI.TextField( new Rect(0,0,urlFieldWidth,GlobalGui.barHeightWidth),urlField);
            if(GUI.Button( new Rect(width-GlobalGui.buttonWidth,0,GlobalGui.buttonWidth,GlobalGui.barHeightWidth),"Load")) {
                GetComponent<TestLoader>().LoadUrl(urlField);
            }
            GUI.EndGroup();

            var items = local ? testItemsLocal : testItems;

            float listItemWidth = GlobalGui.listWidth-16;
            local = GUI.Toggle(new Rect(GlobalGui.listWidth,GlobalGui.barHeightWidth,GlobalGui.listWidth*2,GlobalGui.barHeightWidth),local,local?"local":"http");
            scrollPos = GUI.BeginScrollView(
                new Rect(0,GlobalGui.barHeightWidth,GlobalGui.listWidth,height-GlobalGui.barHeightWidth),
                scrollPos,
                new Rect(0,0,listItemWidth, GlobalGui.listItemHeight*items.Count)
            );

            if(GUI.Button(new Rect(0,0,listItemWidth,GlobalGui.listItemHeight),"change set")) {
                ResetSampleSet();
                return;
            }

            GUIDrawItems( items, listItemWidth, GlobalGui.listItemHeight );
    
            GUI.EndScrollView();
        }
    }

    void GUIDrawItems( List<GLTFast.Tuple<string,string>> items, float listItemWidth, float yPos) {
        float y = yPos;
        foreach( var item in items ) {
            if(GUI.Button(new Rect(0,y,listItemWidth,GlobalGui.listItemHeight),item.Item1)) {
                // Hide menu during loading, since it can distort the performance profiling.
                showMenu = false;
                GetComponent<TestLoader>().LoadUrl(item.Item2);
            }
            y+=GlobalGui.listItemHeight;
        }
    }

    void OnDestroy() {
        var tl = GetComponent<TestLoader>();
        tl.urlChanged -= UrlChanged;
        tl.loadingBegin -= OnLoadingBegin;
        tl.loadingEnd -= OnLoadingEnd;
    }
}
