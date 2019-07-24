using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectManager : MonoBehaviour
{
    GameObject currObj;
    public List<GameObject> objectsInScene;
    public ARTapToPlaceObject arTap;
    public DD_PolyAR google_poly_api;
    Transform pivot;

    #region variables for object controls
    // used in scaling & rotation calculation 
    float startDistance = 0.0f;
    Vector3 currentScale;
    #endregion

    private void Start()
    {
        arTap = FindObjectOfType<ARTapToPlaceObject>();
        google_poly_api = FindObjectOfType<DD_PolyAR>();

        pivot = new GameObject().transform;
        pivot.name = "pivot";
    }

    public void AddObjectToScene()
    {
        RemoveObjectFromScene(currObj);

        SetSelectedObject(google_poly_api.importedObject);
        objectsInScene.Add( currObj );
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

            currObj = null;
            objectsInScene.Remove(obj);
        }
    }

    public void SetSelectedObject(GameObject obj)
    {
        currObj = obj;
        SetObjectPivot();
        arTap.loadedObj = currObj;
    }

    void SetObjectPivot()
    {
        if (pivot.transform.childCount > 0)
        {
            pivot.GetChild(0).parent = null;
        }

        float yOffset = GetTallestMeshBounds();
        pivot.transform.position = new Vector3(currObj.transform.position.x, currObj.transform.position.y - yOffset, currObj.transform.position.z);

        pivot.transform.LookAt(Camera.main.transform);
        pivot.transform.eulerAngles = new Vector3(0, pivot.transform.eulerAngles.y + 180f, 0);

        currObj.transform.SetParent(pivot);
        currObj = pivot.gameObject;
    }

    float GetTallestMeshBounds()
    {
        float yBounds = currObj.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.extents.y;

        for (int i = 0; i < currObj.transform.childCount; i++)
        {
            if (currObj.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.extents.y > yBounds)
                yBounds = currObj.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.extents.y;
        }

        return yBounds;
    }

    // object controls (scale, rotate)
    private void Update()
    {
        if(currObj != null)
            ScaleObject();
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
}
