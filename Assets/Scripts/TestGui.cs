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

    public static float screenFactor;
    public static float listWidth = 150;
    public static float barHeightWidth = 25;
    public static float listItemHeight = 25;
    public static float buttonWidth = 50;

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

    public static void CalculateScreenFactor() {
        screenFactor = Mathf.Max( 1, Mathf.Floor( Screen.dpi / 100f ));
    }

    public static void TrySetGUIStyles() {
        if(!float.IsNaN(screenFactor)) {
            // Init time gui style adjustments
            var guiStyle = GUI.skin.button;
            guiStyle.fontSize = Mathf.RoundToInt(14 * screenFactor);

            guiStyle = GUI.skin.toggle;
            guiStyle.fontSize = Mathf.RoundToInt(14 * screenFactor);

            guiStyle = GUI.skin.label;
            guiStyle.fontSize = Mathf.RoundToInt(14 * screenFactor);
            screenFactor = float.NaN;
       }
    }

    private void Awake()
    {
        CalculateScreenFactor();

        barHeightWidth *= screenFactor;
        buttonWidth *= screenFactor;
        listWidth *= screenFactor;
        listItemHeight *= screenFactor;

        stopWatch.posX = listWidth;

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
        TrySetGUIStyles();

        float width = Screen.width;
        float height = Screen.height;

        if(showMenu && sampleSet!=null) {
            GUI.BeginGroup( new Rect(0,0,width,barHeightWidth) );
            
            float urlFieldWidth = width-buttonWidth;

#if UNITY_EDITOR
            if(GUI.Button( new Rect(width-buttonWidth*2,0,buttonWidth,barHeightWidth),"Open")) {
                string path = EditorUtility.OpenFilePanel("Select glTF", "", "glb");
                if (path.Length != 0)
                {
                    GetComponent<TestLoader>().LoadUrl("file://"+path);
                }
            }
            urlFieldWidth -= buttonWidth;
#endif

            urlField = GUI.TextField( new Rect(0,0,urlFieldWidth,barHeightWidth),urlField);
            if(GUI.Button( new Rect(width-buttonWidth,0,buttonWidth,barHeightWidth),"Load")) {
                GetComponent<TestLoader>().LoadUrl(urlField);
            }
            GUI.EndGroup();

            var items = local ? testItemsLocal : testItems;

            float listItemWidth = listWidth-16;
            local = GUI.Toggle(new Rect(listWidth,barHeightWidth,listWidth*2,barHeightWidth),local,local?"local":"http");
            scrollPos = GUI.BeginScrollView(
                new Rect(0,barHeightWidth,listWidth,height-barHeightWidth),
                scrollPos,
                new Rect(0,0,listItemWidth, listItemHeight*items.Count)
            );

            if(GUI.Button(new Rect(0,0,listItemWidth,listItemHeight),"change set")) {
                ResetSampleSet();
                return;
            }

            GUIDrawItems( items, listItemWidth, listItemHeight );
    
            GUI.EndScrollView();
        }
    }

    void GUIDrawItems( List<GLTFast.Tuple<string,string>> items, float listItemWidth, float yPos) {
        float y = yPos;
        foreach( var item in items ) {
            if(GUI.Button(new Rect(0,y,listItemWidth,listItemHeight),item.Item1)) {
                // Hide menu during loading, since it can distort the performance profiling.
                showMenu = false;
                GetComponent<TestLoader>().LoadUrl(item.Item2);
            }
            y+=listItemHeight;
        }
    }

    void OnDestroy() {
        var tl = GetComponent<TestLoader>();
        tl.urlChanged -= UrlChanged;
        tl.loadingBegin -= OnLoadingBegin;
        tl.loadingEnd -= OnLoadingEnd;
    }
}
