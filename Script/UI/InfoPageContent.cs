using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[RequireComponent(typeof(LocalizeStringEvent))]
internal class InfoPageContent : MonoBehaviour
{
    [Required] [SerializeField] private Image image;
    [Required] [SerializeField] private TMP_Text text;
    [Required] [SerializeField] private LocalizeStringEvent localizeStringEvent;

    private void Awake()
    {
        localizeStringEvent.OnUpdateString.AddListener(x => text.text = x);
        localizeStringEvent.RefreshString();
    }

    private void Reset()
    {
        localizeStringEvent = GetComponent<LocalizeStringEvent>();
    }

    public void SetValue(Sprite sprite, LocalizedString localizedString)
    {
        image.sprite = sprite;
        localizeStringEvent.StringReference = localizedString;
    }
}