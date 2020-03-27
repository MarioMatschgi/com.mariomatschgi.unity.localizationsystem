using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MM.Extentions;
using UnityEngine;

namespace MM
{
    namespace Libraries
    {
        namespace LocalizationSystem
        {
            public class CSVLoader
            {
                // Public
                public string standardLanguageCsvText = "" +
                    "\"key\",\"en\",\"de\",\"\"\n" + 
                    "\"hello_world\",\"Hello World!\",\"Hallo Welt!\",";


                TextAsset languageCsvFile;
                char lineSeperator = '\n';
                char surrround = '"';
                string[] fieldSeperator = { "\",\"" };


                #region Callback Methodes
                /*
                 *
                 *  Callback Methodes
                 * 
                 */

                #endregion

                #region Gameplay Methodes
                /*
                 *
                 * 
                 *  Gameplay Methodes
                 *
                 *  
                 */

                /// <summary>
                /// Gets the Dictionary of unlocalized keys and localized values for the language <paramref name="_lang"/>
                /// </summary>
                /// <param name="_lang"></param>
                /// <returns>the gotten Dictionary for the given language</returns>
                public Dictionary<string, string> GetDictionaryValues(string _lang)
                {
                    Dictionary<string, string> _dictionary = new Dictionary<string, string>();

                    if (languageCsvFile == null)
                        LoadLanguageCsvFile();

                    string[] _lines = languageCsvFile.text.Split(lineSeperator);
                    int _attributeIdx = -1;
                    string[] _headers = _lines[0].Split(fieldSeperator, StringSplitOptions.None);

                    for (int i = 1; i < _headers.Length; i++)
                    {
                        if (_headers[i].Contains(_lang))
                        {
                            _attributeIdx = i;
                            break;
                        }
                    }

                    Regex _regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    for (int i = 1; i < _lines.Length; i++)
                    {
                        string _line = _lines[i];
                        string[] _fields = _regex.Split(_line);

                        for (int j = 0; j < _fields.Length; j++)
                        {
                            _fields[j] = _fields[j].TrimStart(' ', surrround);
                            _fields[j] = _fields[j].TrimEnd(surrround);
                        }

                        if (_fields.Length > _attributeIdx)
                        {
                            string _key = _fields[0];
                            if (_dictionary.ContainsKey(_key))
                                continue;

                            string _value = _fields[_attributeIdx];
                            _dictionary.Add(_key, _value);
                        }
                    }

                    return _dictionary;
                }

                /// <summary>
                /// Loads the language csv file
                /// </summary>
                public void LoadLanguageCsvFile()
                {
                    string _path = Path.Combine(Application.streamingAssetsPath, LocalizationSystem.languageCsvPath);

                    if (File.Exists(_path))
                    {
                        languageCsvFile = new TextAsset(File.ReadAllText(_path));

                        if (LocalizationSystem.logSuccessLoad)
                            Debug.Log("Successfully loaded localizedLanguages.csv...");
                    }
                    else
                    {
                        if (LocalizationSystem.logErrorLoad)
                            Debug.LogError("Cannot load file \"" + _path + "\", creating a new file!");

                        // Save standard
                        SaveLanguageCsvFile(standardLanguageCsvText);

                        if (LocalizationSystem.logSuccessCreate)
                            Debug.Log("Successfully created localizedLanguages.csv...");

                        // Load again
                        LoadLanguageCsvFile();
                    }
                }

                /// <summary>
                /// Saves the language csv file
                /// </summary>
                public void SaveLanguageCsvFile(string _text)
                {
                    string _path = Path.Combine(Application.streamingAssetsPath, LocalizationSystem.languageCsvPath);
                    string _dirPath = _path.Replace(_path.Split('/')[_path.Split('/').Length - 1], "");

                    if (!Directory.Exists(_dirPath))
                        Directory.CreateDirectory(_dirPath);

                    File.WriteAllText(_path, _text);
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                }

                #endregion

                #region Helper Methodes
                /*
                 *
                 *  Helper Methodes
                 * 
                 */

                #endregion

                #region Editor Methodes
                /*
                 *
                 *  Editor Methodes
                 * 
                 */

#if UNITY_EDITOR
                /// <summary>
                /// Adds a LocalizedString <paramref name="_localizedString"/> to the language csv file
                /// </summary>
                /// <param name="_localizedString"></param>
                public void Add(LocalizedString _localizedString)
                {
                    string _str = string.Format("\n\"{0}\",", _localizedString.key);
                    string[] _lines = languageCsvFile.text.Split(lineSeperator);
                    string[] _headers = _lines[0].Split(fieldSeperator, StringSplitOptions.None);

                    for (int i = 1; i < _headers.Length; i++)
                        foreach (KeyValuePair<Language, string> _item in _localizedString.localizedValues)
                        {
                            if (_headers[i].Equals(_item.Key.GetStringValue()))
                            {
                                _str += string.Format("\"{0}\",", _item.Value);
                                break;
                            }
                        }

                    LoadLanguageCsvFile();
                    SaveLanguageCsvFile(string.Join(lineSeperator.ToString(), languageCsvFile.text, _str));

                    UnityEditor.AssetDatabase.Refresh();
                }

                /// <summary>
                /// Removes a LocalizedString <paramref name="_key"/> from the language csv file
                /// </summary>
                /// <param name="_key"></param>
                public void Remove(string _key)
                {
                    string[] _lines = languageCsvFile.text.Split(lineSeperator);
                    string[] _keys = new string[_lines.Length];

                    for (int i = 0; i < _lines.Length; i++)
                        _keys[i] = _lines[i].Split(fieldSeperator, StringSplitOptions.None)[0];

                    int _idx = -1;

                    for (int i = 0; i < _keys.Length; i++)
                        if (_keys[i].Contains(_key))
                        {
                            _idx = i;
                            break;
                        }

                    LoadLanguageCsvFile();
                    if (_idx > -1)
                        SaveLanguageCsvFile(string.Join(lineSeperator.ToString(), _lines.Where(w => w != _lines[_idx]).ToArray()));
                }

                /// <summary>
                /// Edits a LocalizedString <paramref name="_localizedString"/> in the language csv file
                /// </summary>
                /// <param name="_localizedString"></param>
                public void Edit(LocalizedString _localizedString)
                {
                    Remove(_localizedString.key);
                    Add(_localizedString);
                }
#endif

                #endregion
            }
        }
    }
}