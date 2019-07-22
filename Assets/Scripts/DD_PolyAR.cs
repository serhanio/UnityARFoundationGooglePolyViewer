using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using PolyToolkit;
using UnityEngine.Events;

public class DD_PolyAR : MonoBehaviour {

    #region protected variables
    // a list of - key, value pairs ordered asset.name (ID), asset.displayName (name)
    [SerializeField] public List<KeyValuePair<string, string>> asset_id_name_list;
    [SerializeField] public List<KeyValuePair<string, Texture2D>> asset_thumbnail_list;
    [SerializeField] Transform m_cameraTransform; 
    int resultCount = 20;
    Texture2D texture;
    public GameObject importedObject;
    [SerializeField] ARTapToPlaceObject ar_tap_to_place_object;
    [Header("Unity Events")]
    public UnityEvent onPolyAssetsLoaded = new UnityEvent();
    public UnityEvent onPolyThumbLoaded = new UnityEvent();
    public UnityEvent onAssetImported = new UnityEvent();
    public string[] display_names;
    /* [SerializeField] ObjectControls objectManager; */
    #endregion


    #region public variables

    public string searchKeyword = "tree";

    #endregion

    #region main methods

    void Start () 
    {
        m_cameraTransform = GameObject.FindWithTag("MainCamera").transform;

        asset_id_name_list = new List<KeyValuePair<string, string>>();
        asset_thumbnail_list = new List<KeyValuePair<string, Texture2D>>();
        Debug.Log("Requesting List of Assets...");
        // list featured assets
        PolyApi.ListAssets(PolyListAssetsRequest.Featured(), FeaturedAssetListCallback);
        //PolyAssetSearchQuery(searchKeyword);
	}

    #endregion


    #region helper methods

    void FeaturedAssetListCallback(PolyStatusOr<PolyListAssetsResult> result)
    {
        if (!result.Ok)
        {
            // Handle error.
            Debug.LogError("Failed to import featured list. :( Reason: " + result.Status);
            return;
        }
        // Success. result.Value is a PolyListAssetsResult and
        // result.Value.assets is a list of PolyAssets.
        foreach (PolyAsset asset in result.Value.assets)
        {
            if (asset_id_name_list.Count < resultCount)
            {
                // Do something with the asset here.
                //Debug.Log(asset);
                Debug.Log(asset.displayName);
                // add name and ID KeyValuePair to List
                asset_id_name_list.Add(new KeyValuePair<string, string>(asset.name, asset.displayName));
                // request thumbnail of each asset
                PolyApi.FetchThumbnail(asset, GetThumbnailCallback);
            }
        }

        asset_id_name_list.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.Ordinal));
        //      uI.InstatiatePolyAssetMenu(asset_id_name_list);
        //uI.SetPolyMenuInfo(assetNames, assetID);
        //uI.enabled = true;
        if(onPolyAssetsLoaded != null)
        {
            onPolyAssetsLoaded.Invoke();
        }
    }

    private void GetThumbnailCallback(PolyAsset asset, PolyStatus status)
    {
        if (!status.ok)
        {
            Debug.LogError("Failed to import thumbnail. :( Reason: " + status);
            return;
        }
        //Debug.Log("Successfully imported thumbnail!");

        // add thumbnail textures to list
        asset_thumbnail_list.Add(new KeyValuePair<string, Texture2D>(asset.name, asset.thumbnailTexture));

        //Debug.Log("thumb list size " + asset_thumbnail_list.Count + " result count " + resultCount);

        if (asset_thumbnail_list.Count == resultCount)
        {
            //Debug.Log("THIS IS GETTING CALLED EACH SEARCH");
            asset_thumbnail_list.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.Ordinal));

            if (onPolyThumbLoaded != null)
            {
                onPolyThumbLoaded.Invoke();
            }
        }

    }

    private void GetAssetCallback(PolyStatusOr<PolyAsset> result)
    {
        if (!result.Ok)
        {
            Debug.LogError("Failed to get assets. Reason: " + result.Status);
            return;
        }
        Debug.Log("Successfully got asset!");

        // Set the import options.
        PolyImportOptions options = PolyImportOptions.Default();
        // We want to rescale the imported mesh to a specific size.
        options.rescalingMode = PolyImportOptions.RescalingMode.FIT;
        // The specific size we want assets rescaled to (fit in a 5x5x5 box):
        options.desiredSize = 0.5f;
        // We want the imported assets to be recentered such that their centroid coincides with the origin:
        options.recenter = true;

        //statusText.text = "Importing...";
        PolyApi.Import(result.Value, options, ImportAssetCallback);
    }

    // Callback invoked when an asset has just been imported.
    private void ImportAssetCallback(PolyAsset asset, PolyStatusOr<PolyImportResult> result)
    {
        if (!result.Ok)
        {
            Debug.LogError("Failed to import asset. :( Reason: " + result.Status);
            //statusText.text = "ERROR: Import failed: " + result.Status;
            return;
        }
        Debug.Log("Successfully imported asset!");

        // Show attribution (asset title and author).
        //statusText.text = asset.displayName + "\nby " + asset.authorName;

        // Here, you would place your object where you want it in your scene, and add any
        // behaviors to it as needed by your app. As an example, let's just make it
        // slowly rotate:
        Vector3 objPosition = m_cameraTransform.position + (m_cameraTransform.forward.normalized * 0.75f); //place sphere 10cm in front of device
        result.Value.gameObject.transform.position = objPosition;
        //ar_tap_to_place_object.loadedObj = result.Value.gameObject;
        importedObject = result.Value.gameObject;

        if (onAssetImported != null)
        {
            onAssetImported.Invoke();
        }
        //result.Value.gameObject.AddComponent<Rotate>();
        /*for (int i = 0; i < result.Value.gameObject.transform.childCount; i++)
        {
            result.Value.gameObject.transform.GetChild(i).gameObject.AddComponent<MeshCollider>();
        }*/

        // arHit.SetTransformSelection(result.Value.gameObject.transform);

        /*if(objectManager.worldCanvas != null)
            Destroy(objectManager.worldCanvas);*/
        //objectManager.worldCanvas = Instantiate(objectManager.worldCanvasPrefab, result.Value.gameObject.transform.position + new Vector3(0, 0.3f, 0), result.Value.gameObject.transform.rotation, result.Value.gameObject.transform);
        //objectManager.AddObjectToList(result.Value.gameObject);
    }

    // get single asset w/ ID
    public void GetSingleAssetWithID(string modelId)
    {
        PolyApi.GetAsset(modelId, GetAssetCallback);
    }

    public void PolyAssetSearchQuery(string searchKey)
    {
        PolyListAssetsRequest req = new PolyListAssetsRequest();
        // Search by keyword:
        req.keywords = searchKey;
        // Only curated assets:
        req.curated = true;
        // Limit complexity to simple low poly.
        req.maxComplexity = PolyMaxComplexityFilter.COMPLEX;
        // Only Blocks objects.
        //req.formatFilter = PolyFormatFilter.BLOCKS;
        // Order from best to worst.
        req.orderBy = PolyOrderBy.BEST;
        // Up to 20 results per page.
        req.pageSize = 20;
        // Send the request.
        PolyApi.ListAssets(req, SearchAssetListCallback);
    }

    void SearchAssetListCallback(PolyStatusOr<PolyListAssetsResult> result)
    {
        if (!result.Ok)
        {
            // Handle error.
            Debug.LogError("Failed to import featured list. :( Reason: " + result.Status);
            return;
        }

        asset_id_name_list.Clear();
        asset_thumbnail_list.Clear();
        resultCount = result.Value.assets.Count;

        // Success. result.Value is a PolyListAssetsResult and
        // result.Value.assets is a list of PolyAssets.
        foreach (PolyAsset asset in result.Value.assets)
        {
            // Do something with the asset here.
            //Debug.Log(asset);
            Debug.Log(asset.displayName);
            // add name and ID KeyValuePair to List
            asset_id_name_list.Add(new KeyValuePair<string, string>(asset.name, asset.displayName));
            // request thumbnail of each asset
            PolyApi.FetchThumbnail(asset, GetThumbnailCallback);
        }
        Debug.Log("RESULT COUNT: " + result.Value.assets.Count);

        asset_id_name_list.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.Ordinal));

        if (onPolyAssetsLoaded != null)
            onPolyAssetsLoaded.Invoke();
    }

    #endregion
}
