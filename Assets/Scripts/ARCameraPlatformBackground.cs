//
//
// purpose: set camera background material depending on platform used
//
//
// author: ge
//
// (c) 2019 innovation.rocks consulting gmbh
//

using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARCameraBackground))]
public class ARCameraPlatformBackground : MonoBehaviour
{
    public Material _ARCoreBackground;
    public Material _ARKitBackground;

    void Start()
    {
        Debug.Log(this.name);
        ARCameraBackground arcbg = GetComponent<ARCameraBackground>();
        arcbg.useCustomMaterial = true;
#if UNITY_IOS
        arcbg.customMaterial = _ARKitBackground;
#else
        arcbg.customMaterial = _ARCoreBackground;
#endif
    }
}
