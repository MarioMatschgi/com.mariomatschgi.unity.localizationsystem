﻿using System.Collections.Generic;

namespace MM.Systems.LocalizationSystem
{
    [System.Serializable]
    public class LocalizedString
    {
        // Private
        public string m_key;

        // Public
        public string key
        {
            get
            {
                return m_key;
            }
            set
            {
                m_key = value;

                localizedValues = LocalizationSystem.GetLanguagesValuesByKey(m_key);
            }
        }
        public Dictionary<Language, string> localizedValues;
        public string localizedValue
        {
            get
            {
                if (localizedValues == null)
                    localizedValues = LocalizationSystem.GetLanguagesValuesByKey(key);

                if (!localizedValues.ContainsKey(LocalizationSystem.language))
                    return "";

                return localizedValues[LocalizationSystem.language];
            }
            set
            {
                localizedValues[LocalizationSystem.language] = value;
            }
        }


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

        /// <summary>
        /// Constructor used to init a LocalizedString with a key <paramref name="_key"/> and a Dictionary <paramref name="_localizedValues"/>
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_localizedValues"></param>
        public LocalizedString(string _key, Dictionary<Language, string> _localizedValues)
        {
            key = _key;
            localizedValues = _localizedValues;
        }

        #endregion

        #region Gameplay Methodes
        /*
         *
         *  Gameplay Methodes
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