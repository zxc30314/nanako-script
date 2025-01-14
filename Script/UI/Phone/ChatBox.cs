using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

[RequireComponent(typeof(LocalizeStringEvent))]
internal class ChatBox : MonoBehaviour
{
    [Required] [SerializeField] private TMP_Text text;

    [Required] [SerializeField] private LocalizeStringEvent localizeStringEvent;

    private void Awake()
    {
        localizeStringEvent.OnUpdateString.AddListener(x => text.text = x);
    }

    private void Reset()
    {
        localizeStringEvent = GetComponent<LocalizeStringEvent>();
        localizeStringEvent.StringReference.WaitForCompletion = true;
    }

    public void SetChatKey(string tableName, string key)
    {
        localizeStringEvent.SetTable(tableName);
        localizeStringEvent.SetEntry(key);
    }

    public void SetChat(List<string> chat)
    {
        text.text = string.Join("\r\n", chat);
    }
}