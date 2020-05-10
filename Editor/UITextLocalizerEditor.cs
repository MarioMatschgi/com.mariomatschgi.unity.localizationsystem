#if UNITY_EDITOR

using UnityEditor;

namespace MM.Systems.LocalizationSystem
{
    [CustomEditor(typeof(UITextLocalizer))]
    public class UITextLocalizerEditor : Editor
    {
        // Private
        UITextLocalizer textLocalizer;


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

        void OnEnable()
        {
            textLocalizer = (UITextLocalizer)base.target;
        }

        public override void OnInspectorGUI()
        {
            // Draw GUI
            base.OnInspectorGUI();

            // Update Text
            textLocalizer.UpdateText();
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

#endif