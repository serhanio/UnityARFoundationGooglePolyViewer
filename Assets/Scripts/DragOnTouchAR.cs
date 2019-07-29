using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class DragOnTouchAR : MonoBehaviour
{
    public SceneObjectManager objectManager;

    private void Start()
    {
        objectManager = FindObjectOfType<SceneObjectManager>();
    }

    private void OnMouseDrag()
    {
        if (!SceneObjectManager.IsPointerOverUIObject())
        {
            Debug.Log("Dragging object " + this.transform.parent.name);

            // set current gameObject to drag
            Debug.Log("Current GameObject " + SceneObjectManager.currObj.name);

            if (SceneObjectManager.touchPoseIsValid)
            {
                SceneObjectManager.currObj.transform.position = SceneObjectManager.touchPos;
                SceneObjectManager.currObj.transform.LookAt(Camera.main.transform);
                SceneObjectManager.currObj.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y + 180f, 0);
            }
        }
    }

}
