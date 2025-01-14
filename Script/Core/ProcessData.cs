using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public abstract class ProcessData
{
    [ValidateInput("ValidateInput")] [HideIf("ValidateInput")] [SerializeField] private SpineAnimationData animationData;
    public bool FadeIn;
    private ProcessData _nextPrecess;
    public abstract ProcessType ProcessType { get; }
    public abstract void Init();

    public List<string> GetAnimations()
    {
        return animationData.GetAnimations();
    }

    private bool ValidateInput(SpineAnimationData spineAnimationData)
    {
        return ProcessType == ProcessType.PhoneChat || spineAnimationData;
    }
}