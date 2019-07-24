using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    private ARSessionOrigin arOrigin;
    public ARRaycastManager arRaycastManager;
    ARSession arSession;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    public Text debugText;
    public GameObject loadedObj;

    bool isTouchOverUI = false;



    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }


    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && loadedObj != null /*&& Input.GetTouch(0).phase == TouchPhase.Began*/)
         {
            // PlaceObject();
            if (!IsPointerOverUIObject())
            {
                loadedObj.transform.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            }
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);

        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
        placementPoseIsValid = (hits.Count > 0);

        if(placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }

        if(debugText)
            debugText.text = placementPoseIsValid.ToString() + " | " + hits.Count;
    }

    public void SetCurrentlySelectedObject(GameObject obj)
    {
        loadedObj = obj;
    }

    bool IsPointerOverUIObject()
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
