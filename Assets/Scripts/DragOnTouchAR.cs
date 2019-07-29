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
            if(SceneObjectManager.currObj)
            Debug.Log("Current GameObject " + SceneObjectManager.currObj.name);
            Debug.Log("Parent GameObject " + GetRootGameObject(this.transform.gameObject).transform.name);

            if (SceneObjectManager.currObj != GetRootGameObject(this.transform.gameObject))
            {
                // set selected object
                objectManager.SetSelectedObject(GetRootGameObject(this.transform.gameObject));
            }

            if (SceneObjectManager.touchPoseIsValid)
            {
                SceneObjectManager.currObj.transform.position = SceneObjectManager.touchPos;
                SceneObjectManager.currObj.transform.LookAt(Camera.main.transform);
                SceneObjectManager.currObj.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y + 180f, 0);
            }
        }
    }

    GameObject GetRootGameObject(GameObject go)
    {
       /* if (go.transform.parent == null)
            return go;*/

        // while parent exists
        while(go.transform.parent != null)
        {
            go = go.transform.parent.gameObject;
        }

        return go;
    }

}
