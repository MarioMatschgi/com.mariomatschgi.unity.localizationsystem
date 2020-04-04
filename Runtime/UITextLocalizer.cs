using UnityEngine;
using TMPro;

namespace MM.Systems.LocalizationSystem
{
    [RequireComponent(typeof(TMP_Text))]
    [AddComponentMenu("MM Localization System/UITextLocalizer")]
    [ExecuteAlways]
    public class UITextLocalizer : MonoBehaviour
    {
        // Public
        [Header("General")]
        public LocalizedString localizedString;


        // Private
        TMP_Text textToLocalize;


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

        void Start()
        {
            UpdateText();
        }

        void Update()
        {
            // If update in Editor is triggered and value has changed
            if (!Application.isPlaying && !localizedString.localizedValue.Equals(textToLocalize.text))
                UpdateText();
        }

        #endregion

        #region Gameplay Methodes
        /*
         *
         *  Gameplay Methodes
         *
         */

        /// <summary>
        /// Updates the Text label
        /// </summary>
        public void UpdateText()
        {
            if (textToLocalize == null)
                textToLocalize = GetComponent<TMP_Text>();

            textToLocalize.text = localizedString.localizedValue;
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