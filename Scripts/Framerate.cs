using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorX.Utils
{
    public class Framerate : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = 30;
        }
    }
}