using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

namespace ConveyorX.Utils
{
    public class LanguageChanger : MonoBehaviour
    {
        public string currentLanguage = "English";

        private void Start()
        {
            LocalizationManager.CurrentLanguage = currentLanguage;
        }
    }
}