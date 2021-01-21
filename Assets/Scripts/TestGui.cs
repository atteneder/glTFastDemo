// Copyright 2020-2021 Andreas Atteneder
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#if !(UNITY_ANDROID || UNITY_WEBGL) || UNITY_EDITOR
#define LOCAL_LOADING
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using GLTFast.Tests;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(TestLoader))]
public class TestGui : MonoBehaviour {

    GltfSampleSet sampleSet = null;


    [SerializeField]
    StopWatch stopWatch = null;

    [SerializeField]
    StopWatchGui stopWatchGui = null;

    public bool showMenu = true;
    // Load files locally (from streaming assets) or via HTTP
    public bool local = false;

    List<Tuple<string,string>> testItems = new List<Tuple<string, string>>();
    List<Tuple<string,string>> testItemsLocal = new List<Tuple<string, string>>();

    string urlField;

    Vector2 scrollPos;

    private void Awake()
    {
        stopWatchGui.posX = GlobalGui.listWidth;

#if PLATFORM_WEBGL && !UNITY_EDITOR
        // Hide UI in glTF compare web
        showMenu = false;
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

        if(newSet==null || newSet.itemCount<1) {
            Debug.LogError("Empty sample set!");
        }

        sampleSet = newSet;

        foreach(var item in sampleSet.GetItemsPrefixed()) {
#if LOCAL_LOADING
            testItemsLocal.Add(
                new Tuple<string, string>(
                    item.name,
                    string.Format( "file://{0}", item.path)
                )
            );
#endif
            testItems.Add( new Tuple<string, string>(item.name,item.path));
        }
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
                    LoadUrl("file://"+path);
                }
            }
            urlFieldWidth -= GlobalGui.buttonWidth;
#endif

            urlField = GUI.TextField( new Rect(0,0,urlFieldWidth,GlobalGui.barHeightWidth),urlField);
            if(GUI.Button( new Rect(width-GlobalGui.buttonWidth,0,GlobalGui.buttonWidth,GlobalGui.barHeightWidth),"Load")) {
                LoadUrl(urlField);
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

    void GUIDrawItems( List<Tuple<string,string>> items, float listItemWidth, float yPos) {
        float y = yPos;
        foreach( var item in items ) {
            if(GUI.Button(new Rect(0,y,listItemWidth,GlobalGui.listItemHeight),item.Item1)) {
                // Hide menu during loading, since it can distort the performance profiling.
                showMenu = false;
                LoadUrl(item.Item2);
            }
            y+=GlobalGui.listItemHeight;
        }
    }

    async void LoadUrl(string url) {
        await GetComponent<TestLoader>().LoadUrl(url);
    }

    void OnDestroy() {
        var tl = GetComponent<TestLoader>();
        tl.urlChanged -= UrlChanged;
        tl.loadingBegin -= OnLoadingBegin;
        tl.loadingEnd -= OnLoadingEnd;
    }
}
