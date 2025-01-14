using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

[AddComponentMenu("Localization/Asset/" + nameof(LocalizedTmpFontEvent))]
[DisallowMultipleComponent]
[ValidateInput("ValidateInput")]
public class LocalizedTmpFontEvent : LocalizedAssetEvent<TMP_FontAsset, LocalizedTmpFont, UnityEventTmpFont>
{
    [Required] [SerializeField] private TMP_Text text;

    private void Reset()
    {
        text = GetComponent<TMP_Text>();
    }

    private bool ValidateInput()
    {
        return !AssetReference.IsEmpty;
    }

    protected override void UpdateAsset(TMP_FontAsset localizedAsset)
    {
        text.font = localizedAsset;
        OnUpdateAsset.Invoke(localizedAsset);
    }
}

[Serializable]
public class UnityEventTmpFont : UnityEvent<TMP_FontAsset>
{
}