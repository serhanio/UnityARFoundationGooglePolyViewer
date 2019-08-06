using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class DragOnTouchAR : MonoBehaviour
{
    public SceneObjectManager objectManager;
    public UnityEvent onMouseDrag;

    private void Start()
    {
        Debug.Log(this.name);
        objectManager = FindObjectOfType<SceneObjectManager>();
    }

    private void OnMouseDrag()
    {
        if (!SceneObjectManager.IsPointerOverUIObject())
        {
            Debug.Log("Mouse Drag");
            // if current object is not same as touched object
            if (SceneObjectManager.currObj != GetRootGameObject(this.transform.gameObject))
            {
                Debug.Log("Dragging object " + this.transform.parent.name);

                // set current gameObject to dragged object
                objectManager.SetSelectedObject(GetRootGameObject(this.transform.gameObject));
            }

            if (SceneObjectManager.touchPoseIsValid && SceneObjectManager.currObj != null)
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
           // Debug.Log(go.transform.name);
            go = go.transform.parent.gameObject;
        }

        return go;
    }

}
