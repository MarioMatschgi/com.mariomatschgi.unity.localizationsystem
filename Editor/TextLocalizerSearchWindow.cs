#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using MM.Extentions;
using MM.Helper;

namespace MM
{
    namespace Libraries
    {
        namespace LocalizationSystem
        {
            public class TextLocalizerSearchWindow : EditorWindow
            {
                // Public static
                public static TextLocalizerSearchWindow window;
                public static string value;


                // Private static
                static Vector2 standardDropdownWindowSize = new Vector2(600, 300);

                static float labelWidth = 100;
                static float labelHeight = 25;

                static float subLabelWidth = 100;
                static float subLabelHeight = 20;

                static float addButtonWidth = 21;
                static float addButtonHeight = 21;

                static float languageEnumWidth = 200;
                static float languageEnumHeight = 21;

                static float deleteButtonWidth = 20;
                static float deleteButtonHeight = 20;

                static float setButtonWidth = 20;
                static float setButtonHeight = 20;

                static float editButtonWidth = 20;
                static float editButtonHeight = 20;

                static float keyTextFieldWidth = 200;
                static float keyTextFieldHeight = 20;


                // Public
                public bool useProperty;
                public SerializedProperty property;
                public Vector2 scrollViewPosition;


                // Private
                Vector2 startMousePos;


                #region Callback Methodes
                /*
                 *
                 * Callback Methodes
                 * 
                 */

                /// <summary>
                /// Opens the TextLocalizerSearchWindow window
                /// </summary>
                public static void Open()
                {
                    // If a TextLocalizerSearchWindow is open, close it
                    if (window != null)
                        window.Close();

                    // Setup and open the window
                    Setup(false);

                    // Setup specific variables
                }
                /// <summary>
                /// Opens the TextLocalizerSearchWindow window for the SerializedProperty <paramref name="_property"/>
                /// </summary>
                public static void Open(SerializedProperty _property)
                {
                    // If a TextLocalizerSearchWindow is open, close it
                    if (window != null)
                        window.Close();

                    // Setup and open the window
                    Setup(true);

                    // Setup specific variables
                    window.property = _property;
                }

                /// <summary>
                /// Sets up the window properties
                /// </summary>
                /// <param name="_useProperty"></param>
                static void Setup(bool _useProperty)
                {
                    // Create the window
                    window = ScriptableObject.CreateInstance<TextLocalizerSearchWindow>();
                    window.titleContent = new GUIContent("Localization Search-Window");

                    // Show the window
                    window.startMousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                    window.ShowAsDropDown(new Rect(window.startMousePos.x - 350, window.startMousePos.y + 10, 10, 10), standardDropdownWindowSize);

                    // Setup non-specific variables
                    window.useProperty = _useProperty;
                }

                void OnDestroy()
                {
                    window = null;
                }

                public void OnGUI()
                {
                    // Cache dictionary
                    Dictionary<string, Dictionary<Language, string>> dictionary = LocalizationSystem.GetKeysLanguages();


                    EditorGUILayout.BeginHorizontal("Box");
                    EditorGUILayout.BeginVertical();
                    #region Search field
                    /*
                     * Search field
                     */

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Search for entry: ", EditorStyles.boldLabel, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
                    value = EditorGUILayout.TextField(value);
                    EditorGUILayout.EndHorizontal();
                    #endregion


                    EditorGUILayout.BeginHorizontal();
                    #region Add Button
                    /*
                     * Add Button
                     */

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Add new entry: ", EditorStyles.boldLabel, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));

                    GUIContent _addButtonContent = new GUIContent(ImageHelperMethodes.LoadPngEditor("Packages/com.mariomatschgi.unity.localizationsystem/Resources/PlusIcon.png"));
                    if (GUILayout.Button(_addButtonContent, GUILayout.Width(addButtonWidth), GUILayout.Height(addButtonHeight)))
                    {
                        TextLocalizerEditWindow.Open(new LocalizedString("", new Dictionary<Language, string>()));
                    }
                    EditorGUILayout.EndHorizontal();

                    #endregion


                    #region Language Enum
                    /*
                     * Language Enum
                     */

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Set language: ", EditorStyles.boldLabel, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));

                    LocalizationSystem.language = (Language)EditorGUILayout.EnumPopup(LocalizationSystem.language, GUILayout.Width(languageEnumWidth), GUILayout.Height(languageEnumHeight));
                    EditorGUILayout.EndHorizontal();

                    #endregion
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();


                    if (value == null)
                        value = "";


                    #region Search results
                    /*
                     * Search results
                     */

                    EditorGUILayout.BeginVertical();
                    scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition);


                    Dictionary<string, Dictionary<Language, string>> _tmpDictionary = dictionary.ToDictionary(entry => entry.Key, entry => entry.Value);
                    bool _shouldSetTmpToReal = true;

                    foreach (KeyValuePair<string, Dictionary<Language, string>> element in dictionary)
                        if (element.Key.ToLower().Contains(value.ToLower()) || element.Value.Any(kvp => kvp.Value.ToLower().Contains(value.ToLower())))
                        {
                            EditorGUILayout.BeginHorizontal("box");


                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.BeginVertical();
                            #region Delete Button
                            /*
                             * Delete Button & Key TextField
                             */

                            GUIContent _deleteButtonContent = new GUIContent(ImageHelperMethodes.LoadPngEditor("Packages/com.mariomatschgi.unity.localizationsystem/Resources/CrossIcon.png"));
                            if (GUILayout.Button(_deleteButtonContent, GUILayout.Width(deleteButtonWidth), GUILayout.Height(deleteButtonHeight)))
                                if (EditorUtility.DisplayDialog("Remove Key " + element.Key + "?", "This will remove the element from localization, are you sure?", "Ok", "Cancel"))
                                {
                                    LocalizationSystem.Remove(element.Key);

                                    AssetDatabase.Refresh();
                                    LocalizationSystem.Init();

                                    _shouldSetTmpToReal = false;

                                    // Cache dictionary
                                    dictionary = LocalizationSystem.GetKeysLanguages();
                                }
                            #endregion


                            #region Set Button
                            /*
                             * Set Button
                             */

                            // Only show the set Button if a property is referenced
                            if (useProperty)
                            {
                                GUIContent _setButtonContent = new GUIContent(ImageHelperMethodes.LoadPngEditor("Packages/com.mariomatschgi.unity.localizationsystem/Resources/ArrowRightIcon.png"));
                                if (GUILayout.Button(_setButtonContent, GUILayout.Width(setButtonWidth), GUILayout.Height(setButtonHeight)))
                                {
                                    LocalizedString _propertyLocalizedString = (LocalizedString)property.GetValue();
                                    _propertyLocalizedString = new LocalizedString(element.Key, element.Value);
                                    property.SetValue(_propertyLocalizedString);
                                    // Redraw drawer
                                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                                }
                            }
                            #endregion
                            EditorGUILayout.EndVertical();


                            #region Edit Button
                            /*
                             * Edit Button
                             */

                            GUIContent _editButtonContent = new GUIContent(ImageHelperMethodes.LoadPngEditor("Packages/com.mariomatschgi.unity.localizationsystem/Resources/EditSearchIcon.png"));
                            if (GUILayout.Button(_editButtonContent, GUILayout.Width(editButtonWidth), GUILayout.Height(editButtonHeight)))
                            {
                                TextLocalizerEditWindow.Open(new LocalizedString(element.Key, element.Value));
                            }
                            #endregion


                            #region Key TextField
                            /*
                             * Key TextField
                             */

                            EditorGUILayout.TextField(element.Key, GUILayout.Width(keyTextFieldWidth), GUILayout.Height(keyTextFieldHeight));
                            #endregion
                            EditorGUILayout.EndHorizontal();


                            #region Localized values
                            /*
                             * Localized values
                             */

                            EditorGUILayout.BeginVertical();
                            foreach (KeyValuePair<Language, string> _value in element.Value)
                            {
                                EditorGUILayout.BeginHorizontal();

                                EditorGUILayout.LabelField(_value.Key.ToString() + ":", EditorStyles.boldLabel, GUILayout.Width(subLabelWidth), GUILayout.Height(subLabelHeight));
                                EditorGUILayout.LabelField(_value.Value, EditorStyles.wordWrappedLabel);

                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();
                            #endregion


                            EditorGUILayout.EndHorizontal();
                        }

                    if (_shouldSetTmpToReal)
                        LocalizationSystem.SetKeysLanguages(_tmpDictionary);


                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                    #endregion
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
                 * Helper Methodes
                 * 
                 */

                #endregion
            }
        }
    }
}
#endif