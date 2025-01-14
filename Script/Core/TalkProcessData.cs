using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

internal class TalkProcessData : ProcessData
{
    [Required] [SerializeField] private TalkNoteData _note;

    private Queue<LocalizedString> _talksQueue = new();
    public override ProcessType ProcessType => ProcessType.Talk;

    public override void Init()
    {
        _talksQueue = new Queue<LocalizedString>();
        foreach (var t in _note.GetTalks())
        {
            _talksQueue.Enqueue(t);
        }
    }

    public bool GetNextText(out LocalizedString talk)
    {
        talk = null;
        var hasValue = _talksQueue.Any();
        if (hasValue)
        {
            talk = _talksQueue.Dequeue();
        }

        return hasValue;
    }
}