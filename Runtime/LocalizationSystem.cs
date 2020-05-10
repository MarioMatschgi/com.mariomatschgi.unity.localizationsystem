using System;
using System.Collections.Generic;
using System.IO;
using MM.Attributes;
using MM.Extentions;
using UnityEngine;

namespace MM.Systems.LocalizationSystem
{
    public class LocalizationSystem
    {
        // Public static
        static Language m_language;
        public static Language language
        {
            get
            {
                // Language gets loaded on init
                return m_language;
            }
            set
            {
                // If language has changed, save
                if (m_language != value)
                    SaveCurrentLanguageFile(value);

                m_language = value;
            }
        }
        public static bool logSuccessLoad = false;
        public static bool logSuccessCreate = false;
        public static bool logErrorLoad = false;

        public static bool isInit;
        public static CSVLoader csvLoader;
        public static string languageCsvPath = "data/lang/localizedLanguages.csv";
        public static string currentLanguagePath = "data/lang/currentLanguage.txt";


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

            m_language = LoadCurrentLanguageFile();
            UpdateDictionaries();

            isInit = true;
        }

        #endregion

        #region Gameplay Methodes
        /*
         *
         *  Gameplay Methodes
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
                return new Dictionary<Language, string>();

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

#if UNITY_EDITOR
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

            csvLoader.Add(_localizedString);

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

            csvLoader.Remove(_key);

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

            csvLoader.Edit(_localizedString);

            UpdateDictionaries();
        }
#endif

        #endregion

        #region Helper Methodes
        /*
         *
         *  Helper Methodes
         * 
         */

        /// <summary>
        /// Saves the current language to the file
        /// </summary>
        static Language LoadCurrentLanguageFile()
        {
            string _path = Path.Combine(Application.streamingAssetsPath, currentLanguagePath);
            Language _language;

            int _idx = 0;
            if (File.Exists(_path) && int.TryParse(File.ReadAllText(_path), out _idx))
            {
                _language = (Language)_idx;

                if (logSuccessLoad)
                    Debug.Log("Successfully loaded currentLanguage.txt...");
            }
            else
            {
                if (logErrorLoad)
                    Debug.LogError("Cannot load file \"" + _path + "\", creating a new file!");

                SaveCurrentLanguageFile((Language)0);

                _language = LoadCurrentLanguageFile();
            }

            return _language;
        }

        /// <summary>
        /// Loads the current language from the file
        /// </summary>
        static void SaveCurrentLanguageFile(Language _language)
        {
            string _path = Path.Combine(Application.streamingAssetsPath, currentLanguagePath);
            string _dirPath = _path.Replace(_path.Split('/')[_path.Split('/').Length - 1], "");

            if (!Directory.Exists(_dirPath))
                Directory.CreateDirectory(_dirPath);

            File.WriteAllText(_path, ((int)_language).ToString());
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            if (logSuccessCreate)
                Debug.Log("Successfully created currentLanguage.txt...");
        }

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