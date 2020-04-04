#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityToolbarExtender;

namespace MM.Systems.LocalizationSystem
{
    [InitializeOnLoad]
    public static class ToolbarItem
    {
        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

        static ToolbarItem()
        {
            ToolbarExtender.LeftToolbarGUI.Add(DrawLeftGUI);
            ToolbarExtender.RightToolbarGUI.Add(DrawRightGUI);
        }

        #endregion

        #region Gameplay Methodes
        /*
         * 
         *  Gameplay Methodes
         *
         */

        static void DrawLeftGUI()
        {

        }

        static void DrawRightGUI()
        {
            if (GUILayout.Button(new GUIContent("Localization Search", "Open the localization search window"), EditorStyles.toolbarButton, GUILayout.Width(180)))
            {
                if (TextLocalizerSearchWindow.window == null)
                    TextLocalizerSearchWindow.Open();
                else
                    TextLocalizerSearchWindow.window.Close();
            }
            GUILayout.FlexibleSpace();
        }

        #endregion

        #region Helper Methodes
        /*
         *
         *  Helper Methodes
         * 
         */

        #endregion
    }
}

#endif