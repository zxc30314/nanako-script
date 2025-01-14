using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
internal class SpineAnimationData : ScriptableObject
{
    [Required] [SerializeField] private SkeletonDataAsset skeletonDataAsset;

    [Required] [SerializeField] [RequiredListLength(1, null)] [ListDrawerSettings(CustomAddFunction = "AnimationsCustomAddFunction")]
    private List<SpineAnimationName> animations;

    private SpineAnimationName AnimationsCustomAddFunction()
    {
        return new SpineAnimationName(skeletonDataAsset);
    }

    public List<string> GetAnimations()
    {
        return animations.Select(x => x.AnimationName).ToList();
    }

    [Serializable]
    private class SpineAnimationName
    {
        [Required] [SerializeField] [ValidateInput("AnimationNameValidateInput")] [SpineAnimation(dataField: "_skeletonDataAsset")]
        public string AnimationName;

        [HideInInspector] [SerializeField] private SkeletonDataAsset _skeletonDataAsset;

        public SpineAnimationName(SkeletonDataAsset skeletonDataAsset)
        {
            _skeletonDataAsset = skeletonDataAsset;
        }

        private bool AnimationNameValidateInput(string animationName)
        {
            return _skeletonDataAsset.GetAnimationStateData().SkeletonData.Animations.FindIndex(x => x.Name == animationName) != -1;
        }
    }
}