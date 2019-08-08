using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalDreams.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DD_Asset_Menu_UI_Screen : MonoBehaviour
{
    #region variables
    [Header("Asset Screen Properties")]
    public UnityEvent onAssetsReceived = new UnityEvent();

    public GameObject assets_panel_prefab;

    public List<GameObject> asset_panel_list;
    public List<Button> asset_panel_buttons;
    public List<Image> asset_panel_images;
    public List<Text> asset_panel_title;
    public Text featured_artist_text;


    public Button search_button, assets_menu_button, delete_button, vr_toggle_button;
    public InputField search_input_field;
    public Sprite ar_toggle_image, vr_toggle_image;

    public DD_PolyAR poly_api;

    public GameObject camera_screen;

    public Vector2 panelSize;
    public RectTransform container;

    GameObject parent;

    SceneObjectManager objectManager;
    #endregion

    #region Helper Methods

    /*
    This method communicates with the DD_PolyAR.cs script to query 3d models from Google's poly api
         */

    public void Awake()
    {
        Debug.Log(this.name);
        parent = GameObject.FindWithTag("asset_panels");
        container = parent.GetComponent<RectTransform>();

        // get panel asset size from prefab
        panelSize = new Vector2(assets_panel_prefab.GetComponent<RectTransform>().sizeDelta.x, assets_panel_prefab.GetComponent<RectTransform>().sizeDelta.y);
        Debug.Log("Panel Size " + panelSize.ToString());

        // Add listener to search button
        search_button.onClick.AddListener(SearchButtonQuery);

        poly_api.onAssetImported.AddListener(SetFeaturedArtistText);

        objectManager = FindObjectOfType<SceneObjectManager>();
        objectManager.onObjectSelected.AddListener(SetFeaturedArtistText);
        //delete_button.onClick.AddListener(delegate { objectManager.RemoveObjectFromScene(SceneObjectManager.currObj); } );
        if (ar_toggle_image != null)
        {
            vr_toggle_button.image.sprite = ar_toggle_image;
            vr_toggle_button.onClick.AddListener(ToggleVRImage);
        }
    }

    public void InstantiatePanels()
    {
        ClearPanels();

        int search_result_count = poly_api.asset_id_name_list.Count;
        container.sizeDelta = new Vector2(container.sizeDelta.x, (panelSize.y * search_result_count) + (panelSize.y/2.0f));

        for (int i = 0; i < search_result_count; i++)
        {
            // instantiate a panel
            GameObject panel = Instantiate(assets_panel_prefab);
            panel.transform.parent = parent.transform;

            // add to list
            asset_panel_list.Add(panel);
            asset_panel_buttons.Add( panel.GetComponent<Button>() );
            asset_panel_images.Add( panel.GetComponent<Image>());
            asset_panel_title.Add( panel.GetComponentInChildren<Text>());

            asset_panel_buttons[i].onClick.AddListener(ImportAssetOnClick);

            //container size
            float init_y_pos = (container.sizeDelta.y / 2.0f) - (panelSize.y * i);

            // offset the panel by y amount
            float offset = panelSize.y;
            Vector2 screenSz = new Vector2(Screen.width, Screen.height);
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, init_y_pos - offset);
            //panel.GetComponent<RectTransform>().position = new Vector3(0, 1 * panelSize.y, 0);

            // content
            panel.name = poly_api.asset_id_name_list[i].Key;
            asset_panel_title[i].text = poly_api.asset_id_name_list[i].Value;
        }
    }

    public void SetThumbnails()
    {
        for (int i = 0; i < asset_panel_list.Count; i++)
        {
            // set thumbnail
            Texture2D t = poly_api.asset_thumbnail_list[i].Value;
            Sprite mySprite = Sprite.Create(t, new Rect(0.0f, 0.0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100.0f);
            asset_panel_images[i].sprite = mySprite;
            asset_panel_images[i].preserveAspect = true;
        }
    }

    void ClearPanels()
    {
        if (asset_panel_list.Count > 0)
        {
            for (int i = 0; i < asset_panel_list.Count; i++)
            {
                Destroy(asset_panel_list[i]);
            }

            asset_panel_list.Clear();
            asset_panel_title.Clear();
            asset_panel_images.Clear();
            asset_panel_buttons.Clear();
        }
    }

    public void SearchButtonQuery()
    {
        Debug.Log("Search Input " + search_input_field.textComponent.text);
        poly_api.PolyAssetSearchQuery(search_input_field.text);
    }

    public void ImportAssetOnClick()
    {
        Debug.Log("You have clicked the " + EventSystem.current.currentSelectedGameObject.name + " button!");
        string modelName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        Debug.Log("MODEL NAME: " + modelName);
        Debug.Log(EventSystem.current.currentSelectedGameObject.name.Remove(0, 7));
        poly_api.GetSingleAssetWithID(EventSystem.current.currentSelectedGameObject.name.Remove(0, 7));
        //GameObject hitObject = Instantiate(Resources.Load<GameObject>(modelName));
        //ARHitControls.m_HitTransform = hitObject.transform;
        //ui_system.SwitchScreen(camera_screen);
        ToggleAssetMenu();
    }

    public void ToggleAssetMenu()
    {
        if(!this.GetComponent<CanvasGroup>().interactable)
        {
            this.GetComponent<CanvasGroup>().alpha = 1;
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.GetComponent<CanvasGroup>().interactable = true;
            camera_screen.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            camera_screen.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            camera_screen.gameObject.GetComponent<CanvasGroup>().interactable = false;
        }
        else
        {
            camera_screen.GetComponent<CanvasGroup>().alpha = 1;
            camera_screen.GetComponent<CanvasGroup>().blocksRaycasts = true;
            camera_screen.GetComponent<CanvasGroup>().interactable = true;
            this.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            this.gameObject.GetComponent<CanvasGroup>().interactable = false;
        }
    }

    public void SetFeaturedArtistText()
    {
        if(featured_artist_text != null)
            featured_artist_text.text = "Artist: " + poly_api.featured_artist_name;
    }

    void ToggleVRImage()
    {
        vr_toggle_button.image.sprite = (ar_toggle_image == vr_toggle_button.image.sprite) ? vr_toggle_button.image.sprite = vr_toggle_image : vr_toggle_button.image.sprite = ar_toggle_image;
        //Debug.Log("toggle " + vr_toggle_button.image.sprite.name);
    }

    #endregion
}
