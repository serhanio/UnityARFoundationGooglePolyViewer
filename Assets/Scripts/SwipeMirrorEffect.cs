using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;

public class SwipeMirrorEffect : MonoBehaviour
{
    [SerializeField] Mirror _mirror;
    [SerializeField] float endPosX, normVal, mirrorVal;
    [SerializeField] UnityEngine.UI.Text mirrorValueText;

    // Start is called before the first frame update
    void Start()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
        //Debug.Log("Swipe in Direction: " + data.Direction);
        //Debug.Log("End Position: " + data.EndPosition.x);
        //Debug.Log("Normalized End Position: " + (data.EndPosition.x/Screen.width));

        endPosX = data.EndPosition.x;
        normVal = endPosX / Screen.width;

        if (normVal > 0.1f)
        {
            _mirror._symmetry = true;
            _mirror._repeat = (int)(normVal * 10);
            Debug.Log("Normalized Mirror: " + (normVal * 10));
        }
        else
        {
            _mirror._symmetry = false;
            _mirror._repeat = 0;
        }

        SetMirrorTextValue(mirrorValueText, _mirror._repeat);
    }

    public void SetMirrorTextValue(UnityEngine.UI.Text text, int value)
    {
        text.text = value.ToString();
    }
}
