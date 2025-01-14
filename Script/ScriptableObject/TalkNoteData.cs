using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
[Serializable]
public class TalkNoteData : ScriptableObject
{
    [Required] [SerializeField] private List<LocalizedStringData> talks;

    public List<LocalizedString> GetTalks()
    {
        return talks.Select(x => x.LocalizedString).ToList();
    }

    [Serializable]
    private class LocalizedStringData
    {
        [ValidateInput("ValidateInput")] public LocalizedString LocalizedString;

        private bool ValidateInput(LocalizedString localizedKey)
        {
            return !localizedKey.IsEmpty;
        }
    }
}