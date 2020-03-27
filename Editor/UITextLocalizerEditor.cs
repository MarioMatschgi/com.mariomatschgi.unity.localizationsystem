using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MM
{
	namespace Libraries
	{
		namespace LocalizationSystem
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