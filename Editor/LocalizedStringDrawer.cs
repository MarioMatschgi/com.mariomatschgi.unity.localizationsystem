#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using MM.Extentions;
using MM.Helper;

namespace MM
{
    namespace Libraries
    {
        namespace LocalizationSystem
        {
            [CustomPropertyDrawer(typeof(LocalizedString))]
            public class LocalizedStringDrawer : PropertyDrawer
            {
                public int asd;

                // Private
                bool localizedValuesFoldout;
                float height;


                #region Callback Methodes
                /*
                 *
                 *  Callback Methodes
                 * 
                 */

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    // If localizedValuesFoldout is true, add 30 (the line height) times the number of languages to the height and return this value
                    if (localizedValuesFoldout)
                        return height + 2 + Enum.GetValues(typeof(Language)).Length * 30;

                    // Else return 30 as the default height
                    return 30;
                }

                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    EditorGUI.BeginProperty(position, label, property);
                    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                    position.width -= 34;
                    position.height = 20;

                    Rect _valueRect = new Rect(position);
                    _valueRect.x += 15;
                    _valueRect.width -= 15;

                    Rect _foldoutRect = new Rect(position);
                    _foldoutRect.width = 25;


                    bool hierarchyMode = EditorGUIUtility.hierarchyMode;
                    EditorGUIUtility.hierarchyMode = false;
                    localizedValuesFoldout = EditorGUI.Foldout(_foldoutRect, localizedValuesFoldout, "");
                    EditorGUIUtility.hierarchyMode = hierarchyMode;

                    position.x += 15;
                    position.width -= 50;

                    LocalizedString _localizedString = (LocalizedString)property.GetValue();
                    GUIStyle _localizedStringStyle = new GUIStyle("textfield");
                    _localizedStringStyle.fontSize = 13;
                    _localizedString.key = EditorGUI.TextField(position, _localizedString.key, _localizedStringStyle);
                    property.SetValue(_localizedString);

                    position.x += position.width + 2;
                    position.width = position.height;

                    Texture _searchIcon = ImageHelperMethodes.LoadPngEditor("Packages/com.mariomatschgi.unity.localizationsystem/Resources/SearchIcon.png");
                    GUIContent _searchContent = new GUIContent(_searchIcon);

                    if (GUI.Button(position, _searchContent))
                    {
                        if (TextLocalizerSearchWindow.window == null)
                            TextLocalizerSearchWindow.Open(ref property);
                        else
                            TextLocalizerSearchWindow.window.Close();
                    }

                    position.x += position.width + 2;

                    Texture _storeIcon = ImageHelperMethodes.LoadPngEditor("Packages/com.mariomatschgi.unity.localizationsystem/Resources/EditSearchIcon.png");
                    GUIContent _storeContent = new GUIContent(_storeIcon);

                    if (GUI.Button(position, _storeContent))
                    {
                        if (TextLocalizerEditWindow.window == null)
                            TextLocalizerEditWindow.Open(property);
                        else
                            TextLocalizerEditWindow.window.Close();
                    }

                    if (localizedValuesFoldout)
                    {
                        _valueRect.y += 7;
                        _valueRect.height = GUI.skin.box.CalcHeight(new GUIContent(_localizedString.localizedValue), _valueRect.width);
                        _valueRect.y += 21;

                        EditorGUILayout.BeginVertical();
                        foreach (Language _lang in Enum.GetValues(typeof(Language)))
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUI.LabelField(_valueRect, _lang.ToString() + ":", EditorStyles.boldLabel);
                            _valueRect.x += 100;
                            EditorGUI.LabelField(_valueRect, _localizedString.localizedValues.ContainsKey(_lang) ? _localizedString.localizedValues[_lang] : "", EditorStyles.wordWrappedLabel);
                            EditorGUILayout.EndHorizontal();

                            _valueRect.x -= 100;
                            _valueRect.y += 20;
                        }
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUI.EndProperty();
                }

                #endregion

                #region Gameplay Methodes
                /*
                 *
                 * 
                 *  Gameplay Methodes
                 *
                 *  
                 */

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
    }
}
#endif