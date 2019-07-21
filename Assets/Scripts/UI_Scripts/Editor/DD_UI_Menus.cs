using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DigitalDreams.UI
{
    public class DD_UI_Menus : MonoBehaviour
    {
        [MenuItem("Digital-Dreams/UI Tools/Create UI Group")]
        public static void CreateUIGroup()
        {
            Debug.Log("Creating UI Group");

            GameObject uiGroup = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UI_System/UI_Prefabs/UI_System.prefab");
            if(uiGroup)
            {
               GameObject createdGroup = Instantiate(uiGroup);
                createdGroup.name = "UI_GRP";
            }
            else
            {
                EditorUtility.DisplayDialog("UI Tools Warning", "Cannot Find UI Group Prefab!", "OK");
            }
        }
    }
}