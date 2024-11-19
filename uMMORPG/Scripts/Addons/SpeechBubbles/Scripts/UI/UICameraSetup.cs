using UnityEngine;
[ExecuteInEditMode]
public class UICameraSetup : MonoBehaviour
{
#if _CLIENT
    private Camera overlayCam;
    private bool initialized;


    private void Start()
    {
        overlayCam = GetComponent<Camera>() as Camera;

        if (!initialized && Application.isEditor && !Application.isPlaying)
        {
            //set UICamera clear flag and Layermask, if not set properly
            overlayCam.clearFlags = CameraClearFlags.Depth;
            overlayCam.cullingMask |= 1 << LayerMask.NameToLayer("UI");
            //Set UICamera properties to be the same to the same
            overlayCam.orthographic = Camera.main.orthographic;
            overlayCam.orthographicSize = Camera.main.orthographicSize;

            overlayCam.nearClipPlane = Camera.main.nearClipPlane;
            overlayCam.farClipPlane = Camera.main.farClipPlane;
            overlayCam.fieldOfView = Camera.main.fieldOfView;

            overlayCam.depth = 0;

            overlayCam.useOcclusionCulling = Camera.main.useOcclusionCulling;
            overlayCam.allowHDR = Camera.main.allowHDR;
            overlayCam.allowMSAA = Camera.main.allowMSAA;

            overlayCam.targetDisplay = Camera.main.targetDisplay;

            // set UI Layer on Main camera to invisible
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
            Camera.main.depth = -1;

            initialized = true;
        }
    }
#endif
}