using System;
using System.Collections;
using System.Collections.Generic;
using MM.Attributes;
using MM.Extentions;
using UnityEngine;

namespace MM
{
    namespace Libraries
    {
        namespace LocalizationSystem
        {
            public class LocalizationSystem
            {
                // Public static
                public static Language language = Language.English;
                public static bool isInit;
                public static CSVLoader csvLoader;


                // Private static
                static Dictionary<Language, Dictionary<string, string>> languageDictionaries;
                static Dictionary<string, Dictionary<Language, string>> keysLanguages;


                #region Callback Methodes
                /*
                 *
                 *  Callback Methodes
                 *
                 */

                /// <summary>
                /// Initializes the LocalizationSystem
                /// </summary>
                public static void Init()
                {
                    csvLoader = new CSVLoader();

                    UpdateDictionaries();

                    isInit = true;
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

                /// <summary>
                /// Gets the Dictionary of language keys and their localized values for the unlocalized key <paramref name="_key"/>
                /// </summary>
                /// <param name="_key"></param>
                /// <returns>the gotten Dictionary for the given unlocalized key</returns>
                public static Dictionary<Language, string> GetLanguagesValuesByKey(string _key)
                {
                    if (_key == null)
                        return null;

                    if (!isInit)
                        Init();

                    if (!keysLanguages.ContainsKey(_key))
                        return new Dictionary<Language, string>();

                    return keysLanguages[_key];
                }
                /// <summary>
                /// Sets the Dictionary <paramref name="_value"/> for the unlocalized key <paramref name="_key"/>
                /// </summary>
                /// <param name="_key"></param>
                /// <returns>the gotten Dictionary for the given unlocalized key</returns>
                public static void SetLanguagesKeys(string _key, Dictionary<Language, string> _value)
                {
                    if (!isInit)
                        Init();

                    keysLanguages[_key] = _value;
                }

                /// <summary>
                /// Gets the keysLanguages Dictionary
                /// </summary>
                /// <returns>the gotten Dictionary for the given unlocalized key</returns>
                public static Dictionary<string, Dictionary<Language, string>> GetKeysLanguages()
                {
                    if (!isInit)
                        Init();

                    return keysLanguages;
                }
                /// <summary>
                /// Sets the keysLanguages Dictionary
                /// </summary>
                /// <returns>the gotten Dictionary for the given unlocalized key</returns>
                public static void SetKeysLanguages(Dictionary<string, Dictionary<Language, string>> _value)
                {
                    if (!isInit)
                        Init();

                    keysLanguages = _value;
                }
                /// <summary>
                /// If a key is being contained in keysLanguages Dictionary
                /// </summary>
                /// <param name="_key"></param>
                /// <returns></returns>
                public static bool ContainsKey(string _key)
                {
                    return keysLanguages.ContainsKey(_key);
                }

                /// <summary>
                /// Adds a LocalizedString <paramref name="_localizedString"/> to the CSVLoader
                /// </summary>
                /// <param name="_localizedString"></param>
                public static void Add(LocalizedString _localizedString)
                {
                    if (_localizedString.localizedValue.Contains("\""))
                        _localizedString.localizedValue.Replace('"', '\"');

                    if (csvLoader == null)
                        csvLoader = new CSVLoader();

                    csvLoader.LoadLanguageCsvFile();
                    csvLoader.Add(_localizedString);
                    csvLoader.LoadLanguageCsvFile();

                    UpdateDictionaries();
                }
                /// <summary>
                /// Removes a LocalizedString <paramref name="_key"/> from the CSVLoader
                /// </summary>
                /// <param name="_key"></param>
                public static void Remove(string _key)
                {
                    if (csvLoader == null)
                        csvLoader = new CSVLoader();

                    csvLoader.LoadLanguageCsvFile();
                    csvLoader.Remove(_key);
                    csvLoader.LoadLanguageCsvFile();

                    UpdateDictionaries();
                }

                /// <summary>
                /// Edits a LocalizedString <paramref name="_localizedString"/> in the CSVLoader
                /// </summary>
                /// <param name="_localizedString"></param>
                public static void Edit(LocalizedString _localizedString)
                {
                    if (_localizedString.localizedValue.Contains("\""))
                        _localizedString.localizedValue.Replace('"', '\"');

                    if (csvLoader == null)
                        csvLoader = new CSVLoader();

                    csvLoader.LoadLanguageCsvFile();
                    csvLoader.Edit(_localizedString);
                    csvLoader.LoadLanguageCsvFile();

                    UpdateDictionaries();
                }

                #endregion

                #region Helper Methodes
                /*
                 *
                 *  Helper Methodes
                 * 
                 */

                /// <summary>
                /// Updates the Dictionaries
                /// </summary>
                static void UpdateDictionaries()
                {
                    // languageDictionaries
                    languageDictionaries = new Dictionary<Language, Dictionary<string, string>>();

                    foreach (Language _lang in Enum.GetValues(typeof(Language)))
                        languageDictionaries.Add(_lang, csvLoader.GetDictionaryValues(_lang.GetStringValue()));


                    // keysLanguages
                    keysLanguages = new Dictionary<string, Dictionary<Language, string>>();

                    foreach (KeyValuePair<Language, Dictionary<string, string>> _item in languageDictionaries)
                        foreach (KeyValuePair<string, string> _item2 in _item.Value)
                        {
                            if (!keysLanguages.ContainsKey(_item2.Key))
                                keysLanguages.Add(_item2.Key, new Dictionary<Language, string>() { { _item.Key, _item2.Value }, });
                            else if (!keysLanguages[_item2.Key].ContainsKey(_item.Key))
                                keysLanguages[_item2.Key].Add(_item.Key, _item2.Value);
                            else
                                keysLanguages[_item2.Key][_item.Key] = _item2.Value;
                        }
                }

                #endregion
            }

            [System.Serializable]
            public enum Language
            {
                [StringValue("en")]
                English,
                [StringValue("de")]
                German,
            }
        }
    }
}