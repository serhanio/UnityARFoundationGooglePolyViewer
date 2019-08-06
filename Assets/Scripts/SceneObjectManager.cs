using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SceneObjectManager : MonoBehaviour
{
    public static GameObject currObj;
    public List<GameObject> objectsInScene;
    public ARPlacementIndicator arTap;
    public DD_PolyAR google_poly_api;
    Transform pivot;

    public ARRaycastManager arRaycastManager;

    #region variables for object controls
    // used in scaling & rotation calculation 
    float startDistance = 0.0f;
    Vector3 currentScale;

    private Pose touchPose;
    public static Vector3 touchPos = Vector3.zero;
    public static bool touchPoseIsValid = false;
    private static bool isTouchOverUI = false;

    public UnityEvent onObjectSelected = new UnityEvent();
    public UnityEvent onObjectRemoved = new UnityEvent();
    #endregion

    private void Start()
    {
        Debug.Log(this.name);
        arTap = FindObjectOfType<ARPlacementIndicator>();
        google_poly_api = FindObjectOfType<DD_PolyAR>();

        pivot = new GameObject().transform;
        pivot.name = "pivot";

        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    public void AddObjectToScene()
    {
        //RemoveObjectFromScene(currObj);

        // Set selected object to
        Debug.Log(google_poly_api.importedObject);
        SetSelectedObject(google_poly_api.importedObject);
        objectsInScene.Add(google_poly_api.importedObject);
    }

    public void RemoveObjectFromScene(GameObject obj)
    {
        if (currObj)
        {
            if (currObj == pivot.gameObject)
            {
                for (int i = 0; i < pivot.childCount; i++)
                {
                    Destroy(pivot.GetChild(i).gameObject);
                }
            }
            
            objectsInScene.IndexOf(currObj.transform.GetChild(0).gameObject);
            objectsInScene.Remove(currObj.transform.GetChild(0).gameObject);

            if (objectsInScene.Count > 0)
                currObj = objectsInScene[objectsInScene.Count-1];
            else
                currObj = null;

            if (onObjectRemoved != null)
            {
                onObjectRemoved.Invoke();
            }
        }
    }

    public void SetSelectedObject(GameObject obj)
    {
        currObj = obj;
        SetObjectPivot();
        arTap.loadedObj = currObj;

        if (onObjectSelected != null)
        {
            onObjectSelected.Invoke();
        }
    }

    void SetObjectPivot()
    {
        /*if (pivot.transform.childCount > 0)
        {
            pivot.GetChild(0).parent = null;
        }*/
        Unpivot();

        float yOffset = GetTallestMeshBounds();
        pivot.transform.position = new Vector3(currObj.transform.position.x, currObj.transform.position.y - yOffset, currObj.transform.position.z);

        pivot.transform.LookAt(Camera.main.transform);
        pivot.transform.eulerAngles = new Vector3(0, pivot.transform.eulerAngles.y + 180f, 0);

        currObj.transform.SetParent(pivot);
        currObj = pivot.gameObject;
    }

    public void Unpivot()
    {
        if(pivot.childCount > 0)
        {
            for(int i = 0; i < pivot.childCount; i++)
            {
                pivot.transform.GetChild(i).parent = null;
            }
        }
    }

    float GetTallestMeshBounds()
    {
        float yBounds = currObj.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.extents.y;
        int tallestIndex = 0;

        for (int i = 0; i < currObj.transform.childCount; i++)
        {
            if (currObj.transform.GetChild(i).GetComponent<MeshRenderer>() != null)
            {
                if (currObj.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.extents.y > yBounds)
                {
                    yBounds = currObj.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.extents.y;
                    tallestIndex = i;
                }
            }
        }

        // if first time adding GameObject to scene
        if (currObj.transform.GetChild(tallestIndex).transform.GetComponent<BoxCollider>() == null)
        {
            // give tallest object a collider
            GameObject tallestObj = currObj.transform.GetChild(tallestIndex).gameObject;
            tallestObj.AddComponent<BoxCollider>();
            tallestObj.AddComponent<DragOnTouchAR>();

            Debug.Log("ADDED BOX COLLIDER TO CHILD INDEX " + tallestIndex);
        }

        return yBounds;
    }

    // object controls (scale, rotate)
    private void Update()
    {
        if(currObj != null)
        {
            ScaleObject();
            UpdateTouchPose();
        }
    }

    void ScaleObject()
    {
        // TODO change touchDistance to fraction of screen size
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            float touchDistance = getTouchDistance();
            // For the starting touch,
            if (Input.GetTouch(0).phase == TouchPhase.Began
                || Input.GetTouch(1).phase == TouchPhase.Began
                && touchDistance > (Screen.width / 10))
            {
                // save distance and scale for later
                startDistance = touchDistance;
                Debug.Log("start distance set: " + touchDistance);
                currentScale = currObj.transform.localScale;
            }

            // if fingers are being dragged
            if (Input.GetTouch(0).phase == TouchPhase.Moved
                && Input.GetTouch(1).phase == TouchPhase.Moved
                && touchDistance > (Screen.width / 5))
            {
                // compute percent difference in distance between fingers compared to first tap
                float distDifference = (startDistance - touchDistance) / startDistance;
                Debug.Log(distDifference);
                currObj.transform.localScale = currentScale * (1 - distDifference);
            }

            /*if (scaleDisplay != null)
            {
                if (!scaleDisplay.activeInHierarchy)
                    scaleDisplay.SetActive(true);
                scaleDisplay.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = ((int)(arHit.m_HitTransform.localScale.x * 100f)).ToString() + "%";
                StartCoroutine(HideScaleDisplay(1.5f));
            }*/
        }
    }

    // Touch Distance
    float getTouchDistance()
    {
        // get current distance between fingers
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);
        return (touchOne.position - touchZero.position).magnitude;
    }
    float getTouchDistance(int idx1, int idx2)
    {
        // get current distance between fingers
        Touch touchZero = Input.GetTouch(idx1);
        Touch touchOne = Input.GetTouch(idx2);
        return (touchOne.position - touchZero.position).magnitude;
    }

    public Color gizmoColor = Color.yellow;

    void OnDrawGizmos()
    {
        // Draw a yellow cube at the transform's position
        Gizmos.color = gizmoColor;
        if (pivot != null && pivot.transform.childCount > 0)
        {
            if(pivot.GetChild(0).GetChild(0) != null)
                Gizmos.DrawWireCube(pivot.GetChild(0).position, GetBoundSize(pivot.GetChild(0).GetChild(0)));
        }
    }

    public Vector3 GetBoundSize(Transform t)
    {
        Vector3 m_Size;
        Bounds bounds = t.GetComponent<MeshRenderer>().bounds;
        m_Size = bounds.size;
        Debug.Log(m_Size);
        return m_Size;
    }

    private void UpdateTouchPose()
    {

        if (Input.touchCount > 0)
        {
            // PlaceObject();
            if (!IsPointerOverUIObject())
            {
                // ARRaycast from touch position
                var hits = new List<ARRaycastHit>();
                arRaycastManager.Raycast(Input.touches[0].position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
                touchPoseIsValid = (hits.Count > 0);

                if (touchPoseIsValid)
                {
                    touchPose = hits[0].pose;
                    touchPos = touchPose.position;
                   /* currObj.transform.position = touchPose.position;
                    currObj.transform.LookAt(Camera.main.transform);
                    currObj.transform.eulerAngles = new Vector3(0, currObj.transform.eulerAngles.y + 180f, 0);*/
                }
            }
        }
    }


    public static bool IsPointerOverUIObject()
    {
        // Check if there is a touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Check if finger is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Debug.Log("Touched the UI");
                isTouchOverUI = true;
            }
            else
            {
                isTouchOverUI = false;
            }
        }

        return isTouchOverUI;
    }
}
