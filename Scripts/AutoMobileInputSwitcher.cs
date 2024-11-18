using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorX.Utils
{
    public class AutoMobileInputSwitcher : MonoBehaviour
    {
        public GameObject mobileControlsUI;

        private void Start()
        {
            if (Application.isMobilePlatform)
            {
                mobileControlsUI.SetActive(true);
            }
            else 
            {
                mobileControlsUI.SetActive(false);
            }
        }
    }
}