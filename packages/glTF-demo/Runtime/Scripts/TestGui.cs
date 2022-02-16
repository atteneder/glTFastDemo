﻿// Copyright 2020-2022 Andreas Atteneder
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
using GLTFast;
using GLTFTest;
using GLTFTest.Sample;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(TestLoader))]
public class TestGui : MonoBehaviour {

    public class DropDown {
        public bool show { get; private set; }
        string[] items;
        Vector2 scrollViewVector = Vector2.zero;
        int indexNumber;
        float buttonHeight = GlobalGui.listItemHeight;
        bool allowUnset = false;
        string label = null;
        GUIStyle buttonStyle;

        public DropDown(string[] items, bool allowUnset = false, string label = null ) {
            this.items = items;
            this.allowUnset = allowUnset;
            this.label = label;
            buttonHeight = GlobalGui.listItemHeight;
        }

        public void SetIndex(int index) {
            indexNumber = index;
        }
        
        public void DrawGUI( Rect dropDownRect, Action<int> selectCallback ) {

            if (buttonStyle == null) {
                buttonStyle = GUI.skin.button;
                buttonStyle.clipping = TextClipping.Clip;
                buttonStyle.wordWrap = false;
            }
            
            string mainButtonText;
            if (string.IsNullOrEmpty(label)) {
                mainButtonText = indexNumber>=0 ? items[indexNumber] : "Select";
            }
            else {
                mainButtonText = indexNumber>=0 ? $"{label}: {items[indexNumber]}" : $"Select {label}";
            }
            
            if (GUI.Button(new Rect((dropDownRect.x), dropDownRect.y, dropDownRect.width, buttonHeight), mainButtonText)) {
                show = !show;
            }

            if (show) {
                var itemCount = items.Length + (allowUnset ? 1 : 0);
                var totalHeight = itemCount * buttonHeight;
                scrollViewVector = GUI.BeginScrollView(
                    new Rect(dropDownRect.x,dropDownRect.y+buttonHeight,dropDownRect.width,dropDownRect.height-buttonHeight),
                    scrollViewVector,
                    new Rect(0, 0, dropDownRect.width, totalHeight)
                    );

                GUI.Box(new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, (itemCount * buttonHeight))), "");

                var y = 0f;
                for (int index = allowUnset?-1:0; index < items.Length; index++) {

                    var buttonLabel = index < 0 ? "None" : items[index];
                    if (GUI.Button(new Rect(0, y, dropDownRect.width, buttonHeight), buttonLabel)) {
                        show = false;
                        indexNumber = index;
                        selectCallback(index);
                    }

                    y += buttonHeight;
                }

                GUI.EndScrollView();
            }
        }
    }

    SampleSet sampleSet = null;


    [SerializeField]
    StopWatch stopWatch = null;

    [SerializeField]
    StopWatchGui stopWatchGui = null;

    public bool showMenu = true;
    // Load files locally (from streaming assets) or via HTTP
    public bool local = false;

    [SerializeField]
    GameObject cameraObject;

    
    List<Tuple<string,string>> testItems = new List<Tuple<string, string>>();
    List<Tuple<string,string>> testItemsLocal = new List<Tuple<string, string>>();

    string urlField;

    Vector2 scrollPos;

    string[] sceneNames;
    GameObjectInstantiator.SceneInstance sceneInstance;
    DropDown sceneDropDown;
    DropDown cameraDropDown;
    
    TestLoader testLoader;
    
    int? currentSceneIndex => testLoader.currentSceneIndex;

    void Awake() {
        testLoader = GetComponent<TestLoader>();
        stopWatchGui.posX = GlobalGui.listWidth;

#if PLATFORM_WEBGL && !UNITY_EDITOR
        // Hide UI in glTF compare web
        showMenu = false;
#endif

        var selectSet = GetComponent<SampleSetSelectGui>();
        selectSet.onSampleSetSelected += OnSampleSetSelected;

        testLoader.urlChanged += UrlChanged;
        testLoader.loadingBegin += OnLoadingBegin;
        testLoader.loadingEnd += OnLoadingEnd;
    }

    void OnLoadingBegin() {
        stopWatch.StartTime();
    }

    void OnLoadingEnd() {
        showMenu = true;
        stopWatch.StopTime();
    }

    void OnSampleSetSelected(SampleSet newSet) {

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
            
            var y = 0f;
            
            GUI.BeginGroup( new Rect(0,0,width,GlobalGui.barHeightWidth) );
            
            float urlFieldWidth = width-GlobalGui.buttonWidth;

#if UNITY_EDITOR
            if(GUI.Button( new Rect(width-GlobalGui.buttonWidth*2,0,GlobalGui.buttonWidth,GlobalGui.barHeightWidth),"Open")) {
                string path = EditorUtility.OpenFilePanel("Select glTF", "", "glb");
                if (path.Length != 0)
                {
                    LoadUrlAsync("file://"+path);
                }
            }
            urlFieldWidth -= GlobalGui.buttonWidth;
#endif

            urlField = GUI.TextField( new Rect(0,0,urlFieldWidth,GlobalGui.barHeightWidth),urlField);
            if(GUI.Button( new Rect(width-GlobalGui.buttonWidth,0,GlobalGui.buttonWidth,GlobalGui.barHeightWidth),"Load")) {
                LoadUrlAsync(urlField);
            }
            GUI.EndGroup();
            y += GlobalGui.barHeightWidth;

            float listItemWidth = GlobalGui.listWidth-16;
            local = GUI.Toggle(new Rect(GlobalGui.listWidth,GlobalGui.barHeightWidth,GlobalGui.listWidth*2,GlobalGui.barHeightWidth),local,local?"local":"http");
            
            if (sceneDropDown != null || cameraDropDown != null) {
                
                bool somethingShown = false;
                
                if(GUI.Button(new Rect(0,y,listItemWidth,GlobalGui.listItemHeight),"back to set")) {
                    sceneNames = null;
                    sceneDropDown = null;
                    cameraDropDown = null;
                    return;
                }

                var dropdownHeight = Screen.height-y;
                y += GlobalGui.listItemHeight;
                
                if (sceneDropDown != null) {
                    sceneDropDown.DrawGUI(
                        new Rect(0, y, listItemWidth, dropdownHeight), 
                        i => SetSceneIndex(i));
                    y += GlobalGui.listItemHeight;
                    somethingShown = sceneDropDown.show;
                }

                dropdownHeight = Screen.height-y;
                if (!somethingShown && cameraDropDown != null) {
                    cameraDropDown?.DrawGUI(
                        new Rect(0, y, listItemWidth, dropdownHeight),
                        i => SetCameraIndex(i) );
                    somethingShown = cameraDropDown.show;
                }
            } else {
                var items = local ? testItemsLocal : testItems;
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
    }

    void GUIDrawItems( List<Tuple<string,string>> items, float listItemWidth, float yPos) {
        float y = yPos;
        foreach( var item in items ) {
            if(GUI.Button(new Rect(0,y,listItemWidth,GlobalGui.listItemHeight),item.Item1)) {
                // Hide menu during loading, since it can distort the performance profiling.
                showMenu = false;
                LoadUrlAsync(item.Item2);
            }
            y+=GlobalGui.listItemHeight;
        }
    }

    void SetSceneIndex(int index) {
        var loader = GetComponent<TestLoader>();
        cameraDropDown = null;
        loader.ClearScene();
        stopWatch.StartTime();
        loader.InstantiateScene(index);
        stopWatch.StopTime();
        CreateCameraDropDown(loader);
    }
    
    void SetCameraIndex(int index) {
        for (var i = 0; i < sceneInstance.cameras.Count; i++) {
            var camera = sceneInstance.cameras[i];
            camera.enabled = i == index;
        }

        if (cameraObject != null) {
            cameraObject.SetActive(index < 0);
        }
    }

    async void LoadUrlAsync(string url) {
        var loader = GetComponent<TestLoader>();
        await loader.LoadUrl(url);
        sceneNames = loader.GetSceneNames();
        if (sceneNames != null && sceneNames.Length > 1) {
            sceneDropDown = new DropDown(sceneNames, label:"Scene");
            if (loader.currentSceneIndex.HasValue) {
                sceneDropDown.SetIndex(loader.currentSceneIndex.Value);
            }
        } else {
            sceneDropDown = null;
        }
        
        CreateCameraDropDown(loader);
    }

    void CreateCameraDropDown(TestLoader loader) {
        if (cameraObject != null) {
            cameraObject.SetActive(true);
        }
        
#if UNITY_DOTS_HYBRID
        // TODO: query cameras and feed into list
#else
        sceneInstance = loader.sceneInstance;
        if (sceneInstance?.cameras != null && sceneInstance.cameras.Count > 0) {
            var names = new string[sceneInstance.cameras.Count];
            for (var index = 0; index < sceneInstance.cameras.Count; index++) {
                names[index] = sceneInstance.cameras[index].name;
            }

            cameraDropDown = new DropDown(names, true, "Camera");
            cameraDropDown.SetIndex(-1);
        }
        else
#endif
        {
            cameraDropDown = null;
        }
    }

    void OnDestroy() {
        testLoader.urlChanged -= UrlChanged;
        testLoader.loadingBegin -= OnLoadingBegin;
        testLoader.loadingEnd -= OnLoadingEnd;
    }
}
