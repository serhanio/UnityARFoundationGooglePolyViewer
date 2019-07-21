using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Colorful;

public class WiggleController : MonoBehaviour
{
    [SerializeField] Wiggle wiggle;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void WiggleAmplitude(Slider slider)
    {
        wiggle.Amplitude = slider.value;
    }
}
