using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

internal class LocalizedTmpFontCreat : MonoBehaviour
{
    [AssetList(Path = "/Prefab/", AutoPopulate = true)] [SerializeField]
    private List<GameObject> list;

    [Button]
    private void Do()
    {
        var componentsInChildren = FindObjectsOfType<TMP_Text>();
        foreach (var componentsInChild in componentsInChildren)
        {
            componentsInChild.gameObject.AddComponent<LocalizedTmpFontEvent>();
        }

        foreach (var go in list)
        foreach (var componentsInChild in go.GetComponentsInChildren<TMP_Text>())
        {
            componentsInChild.gameObject.AddComponent<LocalizedTmpFontEvent>();
        }
    }
}