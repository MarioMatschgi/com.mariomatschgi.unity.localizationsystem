using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MM
{
    namespace Libraries
    {
        namespace LocalizationSystem
        {
            [RequireComponent(typeof(TMP_Text))]
            [AddComponentMenu("MM Localization System/UITextLocalizer")]
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
                    textToLocalize = GetComponent<TMP_Text>();
                    Debug.Log("SSSS " + localizedString.key);
                    textToLocalize.text = localizedString.localizedValue;
                }

                void Update()
                {

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
