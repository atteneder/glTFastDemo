using System.Collections;
using System.Collections.Generic;
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
