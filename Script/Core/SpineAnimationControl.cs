using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Spine.Unity;
using UniRx;
using UnityEngine;
using AnimationState = Spine.AnimationState;

public class SpineAnimationControl : MonoBehaviour
{
    [Required] [SerializeField] private SkeletonGraphic spine;

    private int _animationIndex;
    private List<string> _animations;
    private int _skinIndex;
    private List<string> _skinNames;

    private void Start()
    {
        _skinNames = spine.SkeletonData.Skins.Items.Select(x => x.Name).ToList();
        _skinNames.Remove("nomal");
    }

    public IDisposable SubscribeOnSetAnimationEvent(AnimationState.TrackEntryDelegate action)
    {
        spine.AnimationState.Start += action;
        return Disposable.Create(() =>
        {
            if (spine)
            {
                spine.AnimationState.Start -= action;
            }
        });
    }

    public IDisposable SubscribeAnimationEvent(AnimationState.TrackEntryEventDelegate action)
    {
        spine.AnimationState.Event += action;
        return Disposable.Create(() =>
        {
            if (spine)
            {
                spine.AnimationState.Event -= action;
            }
        });
    }

    public string GetCurrentAnimationName()
    {
        return spine.AnimationState.GetCurrent(0).Animation.Name;
    }

    public void SwitchAnimation()
    {
        var animationName = _animations[++_animationIndex % _animations.Count];
        PlayAnimation(animationName);
    }

    private void PlayAnimation(string animationName, AnimationState.TrackEntryDelegate onEnd = null)
    {
        var trackEntry = spine.AnimationState.SetAnimation(0, animationName, onEnd == null);
        trackEntry.Complete += onEnd;
    }

    public void SwitchSkin()
    {
        spine.Skeleton.SetSkin(_skinNames[++_skinIndex % _skinNames.Count]);
        spine.Skeleton.SetSlotsToSetupPose();
        spine.AnimationState.Apply(spine.Skeleton);
    }

    public void SetAnimations(List<string> animations, AnimationState.TrackEntryDelegate onEnd = null)
    {
        _animations = animations;
        PlayAnimation(_animations.FirstOrDefault(), onEnd);
    }
}