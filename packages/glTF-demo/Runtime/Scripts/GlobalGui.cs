// Copyright 2020-2022 Andreas Atteneder
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

using UnityEngine;

public static class GlobalGui
{
    public static float screenFactor;
    public static float listWidth = 150;
    public static float barHeightWidth = 25;
    public static float listItemHeight = 25;
    public static float buttonWidth = 50;

    static bool initialized;

    public static void Init() {
        if(initialized) return;

        screenFactor = Mathf.Max( 1, Mathf.Floor( Screen.dpi / 100f ));

        var guiStyle = GUI.skin.button;
        guiStyle.fontSize = Mathf.RoundToInt(14 * screenFactor);

        guiStyle = GUI.skin.toggle;
        guiStyle.fontSize = Mathf.RoundToInt(14 * screenFactor);

        guiStyle = GUI.skin.label;
        guiStyle.fontSize = Mathf.RoundToInt(14 * screenFactor);

        barHeightWidth *= screenFactor;
        buttonWidth *= screenFactor;
        listWidth *= screenFactor;
        listItemHeight *= screenFactor;

        initialized = true;
    }
}
