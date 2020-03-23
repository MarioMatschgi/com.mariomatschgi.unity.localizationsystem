#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MM.Extentions;
using MM.Helper;

namespace MM
{
    namespace Libraries
    {
        namespace LocalizationSystem
        {
            public class TextLocalizerEditWindow : EditorWindow
            {
                // Public static
                public static TextLocalizerEditWindow window { get; private set; }

                // Private static
                static Vector2 standardWindowSize = new Vector2(620, 228);

                static Color boxBackgroundColor = new Color(0, 0, 0, .2f);

                static bool isLocalizedValuesFoldout;

                static float margin = 2;

                static float labelWidth = 130;
                static float labelHeight = 25;

                static float subLabelWidth = 100;
                static float subLabelHeight = 20;

                static float nextLangButtonWidth = 20;
                static float nextLangButtonHeight = 20;

                static float saveButtonWidth = 300;
                static float saveButtonHeight = 50;

                static float localizedValuesHeight = 50;

                static float smallSpace = 0;
                static float largeSpace = 10;

                // Public
                public bool shouldResetOnDestroy = true;
                public bool useProperty;
                public LocalizedString localizedString;
                public SerializedProperty property;
                public Language languageToLocalize;

                public GUIStyle boxStyle;

                public string defaultLocalizedValue = "Enter a text";

                public string editExistingLS = "Edit existing localized string";
                public string addNewLS = "Add new localized string";


                // Private
                Vector2 startMousePos;
                LocalizedString originalLocalizedString;


                #region Callback Methodes
                /*
                 *
                 * Callback Methodes
                 * 
                 */

                /// <summary>
                /// Opens the TextLocalizerEditWindow window for the LocalizedString <paramref name="_localizedString"/>
                /// </summary>
                /// <param name="_localizedString"></param>
                public static void Open(LocalizedString _localizedString)
                {
                    // If a TextLocalizerEditWindow is open, close it
                    if (window != null)
                        window.Close();

                    // Setup and open the window
                    Setup(false);

                    // Setup specific variables
                    window.localizedString = _localizedString;
                    window.originalLocalizedString = _localizedString;
                }
                /// <summary>
                /// Opens the TextLocalizerEditWindow window for the SerializedProperty <paramref name="_property"/>
                /// </summary>
                /// <param name="_property"></param>
                public static void Open(SerializedProperty _property)
                {
                    // If a TextLocalizerEditWindow is open, close it
                    if (window != null)
                        window.Close();

                    // Setup and open the window
                    Setup(true);

                    // Setup specific variables
                    window.property = _property;
                    window.localizedString = (LocalizedString)_property.GetValue();
                    window.originalLocalizedString = (LocalizedString)_property.GetValue();
                }

                /// <summary>
                /// Sets up the window properties
                /// </summary>
                /// <param name="_useProperty"></param>
                static void Setup(bool _useProperty)
                {
                    // Create the window
                    window = ScriptableObject.CreateInstance<TextLocalizerEditWindow>();
                    window.titleContent = new GUIContent("Localization Edit-Window");

                    // Show the window
                    window.startMousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                    window.ShowAsDropDown(new Rect(window.startMousePos.x - 350, window.startMousePos.y + 15, 10, 10), standardWindowSize);

                    // Setup non-specific variables
                    window.boxStyle = new GUIStyle();
                    window.boxStyle.normal.background = GenerateColorTexture(boxBackgroundColor);
                    window.shouldResetOnDestroy = true;

                    window.useProperty = _useProperty;
                }

                void OnDestroy()
                {
                    // Reset the localizedString
                    if (shouldResetOnDestroy)
                        Reset();

                    window = null;
                }

                public void OnGUI()
                {
                    // Set margin for window
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(margin), GUILayout.Height(margin));
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("", GUILayout.Width(margin), GUILayout.Height(margin));

                    #region Key TextField
                    /*
                     * Key TextField
                     */

                    EditorGUILayout.BeginHorizontal(boxStyle);
                    EditorGUILayout.LabelField("Key:", EditorStyles.boldLabel, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
                    localizedString.key = EditorGUILayout.TextField("", localizedString.key);
                    // If key has changed, set the values to the old, so the values for the new key doesnt get applyed
                    if (!localizedString.key.Equals(originalLocalizedString.key))
                        localizedString.localizedValues = originalLocalizedString.localizedValues;

                    EditorGUILayout.EndHorizontal();
                    #endregion


                    EditorGUILayout.Space(smallSpace);


                    #region Values LableField with foldout & content
                    /*
                     * Values LableField
                     */

                    EditorGUILayout.BeginHorizontal(boxStyle);
                    EditorGUILayout.LabelField("Values:", EditorStyles.boldLabel, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));

                    /*
                     * Foldout
                     */
                    bool _wasHierarchyMode = EditorGUIUtility.hierarchyMode;
                    EditorGUIUtility.hierarchyMode = false;
                    isLocalizedValuesFoldout = EditorGUILayout.Foldout(isLocalizedValuesFoldout, "Localized values");
                    EditorGUIUtility.hierarchyMode = _wasHierarchyMode;
                    EditorGUILayout.EndHorizontal();

                    /*
                     * Foldout content
                     */
                    if (isLocalizedValuesFoldout)
                    {
                        EditorGUILayout.BeginVertical();
                        foreach (Language _lang in Enum.GetValues(typeof(Language)))
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("", GUILayout.Width(labelWidth), GUILayout.Height(0));
                            EditorGUILayout.LabelField(_lang.ToString() + ":", EditorStyles.boldLabel, GUILayout.Width(subLabelWidth), GUILayout.Height(subLabelHeight));
                            EditorGUILayout.LabelField(localizedString.localizedValues.ContainsKey(_lang) ? localizedString.localizedValues[_lang] : "", EditorStyles.wordWrappedLabel);
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();

                        SetWindowSize(standardWindowSize.x, standardWindowSize.y + 6 + Enum.GetValues(typeof(Language)).Length * subLabelHeight);
                    }
                    else
                        SetWindowSize(standardWindowSize.x, standardWindowSize.y);
                    #endregion


                    EditorGUILayout.Space(largeSpace);


                    #region Language enum to localize and nextLanguage Button
                    /*
                     * Language enum to localize and nextLanguage Button
                     */

                    EditorGUILayout.BeginHorizontal(boxStyle);
                    EditorGUILayout.LabelField("Localizing language:", EditorStyles.boldLabel, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));

                    GUIContent _nextLangButtonContent = new GUIContent(ImageHelperMethodes.LoadPngEditor("Packages/com.mariomatschgi.unity.localizationsystem/Resources/ArrowDownIcon.png"));
                    if (GUILayout.Button(_nextLangButtonContent, GUILayout.Width(nextLangButtonWidth), GUILayout.Height(nextLangButtonHeight)))
                    {
                        // Increase languageToLocalize idx and clamp
                        int _max = Enum.GetValues(typeof(Language)).Length;
                        int _tmp = (int)languageToLocalize + 1;
                        if (_tmp >= _max)
                            _tmp %= _max;
                        languageToLocalize = (Language)Mathf.Clamp(_tmp, 0, _max);
                    }

                    languageToLocalize = (Language)EditorGUILayout.EnumPopup(languageToLocalize, EditorStyles.popup, GUILayout.Height(nextLangButtonHeight));
                    EditorGUILayout.EndHorizontal();
                    #endregion


                    EditorGUILayout.Space(smallSpace);


                    #region Value to localize
                    /*
                     * Value to localize
                     */

                    EditorGUILayout.BeginHorizontal(boxStyle);
                    EditorGUILayout.LabelField("Localizing value:", EditorStyles.boldLabel, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));

                    EditorStyles.textArea.wordWrap = true;

                    string _valueToLocalize = EditorGUILayout.TextArea(localizedString.localizedValues.ContainsKey(languageToLocalize) ? localizedString.localizedValues[languageToLocalize] : defaultLocalizedValue,
                        EditorStyles.textArea, GUILayout.Height(localizedValuesHeight));

                    if (localizedString.localizedValues.ContainsKey(languageToLocalize))
                        localizedString.localizedValues[languageToLocalize] = _valueToLocalize;
                    else
                        localizedString.localizedValues.Add(languageToLocalize, _valueToLocalize);

                    EditorGUILayout.EndHorizontal();
                    #endregion


                    EditorGUILayout.Space(largeSpace);


                    #region Button to save
                    /*
                     * Button to save
                     */

                    EditorGUILayout.BeginHorizontal(boxStyle);
                    GUILayout.FlexibleSpace();
                    GUIStyle _saveButtonStyle = new GUIStyle("button");
                    _saveButtonStyle.fontSize = 15;
                    string _saveButtonText = editExistingLS;
                    if (originalLocalizedString.key.Equals(string.Empty))
                        _saveButtonText = addNewLS;

                    if (GUILayout.Button(_saveButtonText, _saveButtonStyle, GUILayout.Width(saveButtonWidth), GUILayout.Height(saveButtonHeight)))
                    {
                        for (int i = 0; i < Enum.GetValues(typeof(Language)).Length; i++)
                        {
                            if (!localizedString.localizedValues.ContainsKey((Language)i))
                                localizedString.localizedValues.Add((Language)i, defaultLocalizedValue);
                            else if (localizedString.localizedValues[(Language)i].Trim() == string.Empty)
                                localizedString.localizedValues[(Language)i] = defaultLocalizedValue;
                        }

                        if (!localizedString.key.Equals(originalLocalizedString))
                        {
                            LocalizationSystem.Remove(originalLocalizedString.key);

                            LocalizationSystem.Add(localizedString);
                        }
                        else
                            LocalizationSystem.Edit(localizedString);

                        shouldResetOnDestroy = false;

                        // If a property is used, set the localized string to the property
                        if (useProperty)
                        {
                            property.SetValue(localizedString);
                            // Redraw drawer
#if UNITY_EDITOR
                            EditorUtility.SetDirty(property.serializedObject.targetObject);
#endif
                        }

                        window.Close();
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    #endregion

                    // Set margin for window
                    EditorGUILayout.LabelField("", GUILayout.Width(margin), GUILayout.Height(margin));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.LabelField("", GUILayout.Width(margin), GUILayout.Height(margin));
                    EditorGUILayout.EndHorizontal();
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

                void SetWindowSize(float _width, float _height)
                {
                    // Set min and max size, then update the position with size
                    minSize = new Vector2(_width, _height);
                    maxSize = minSize;
                    position = new Rect(window.startMousePos.x - _width, window.startMousePos.y + 15, minSize.x, minSize.y);
                }

                static Texture2D GenerateColorTexture(Color col)
                {
                    // Set color array with only one color
                    Color[] _pixels = new Color[1];
                    _pixels[0] = col;

                    // Apply the color to the texture
                    Texture2D _texture = new Texture2D(1, 1);
                    _texture.SetPixels(_pixels);
                    _texture.Apply();

                    // Return the texture
                    return _texture;
                }

                void Reset()
                {
                    localizedString = originalLocalizedString;
                    if (useProperty)
                        property.SetValue(localizedString);
                }

                #endregion
            }
        }
    }
}
#endif