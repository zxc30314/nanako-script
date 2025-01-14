using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;
using Animation = Spine.Animation;

public class VoiceCheck : MonoBehaviour
{
    [AssetList(Path = "/Voice/", AutoPopulate = true)]
    public List<AudioClip> clips = new();

    [SerializeField] private SkeletonGraphic spine;
    private readonly string _prefix = "nanako";
    private Animation[] animations;

    // Update is called once per frame
    private void Update()
    {
    }

    // Start is called before the first frame update
    [Button]
    private void Check()
    {
        animations = spine.SkeletonData.Animations.ToArray();
        foreach (var animation1 in animations)
        {
            if (!clips.Find(x => x.name == $"{_prefix}_{animation1.Name}_cv"))
            {
                Debug.LogError($"Not Find {_prefix}_{animation1.Name}_cv");
                return;
            }

            if (!clips.Find(x => x.name == $"{_prefix}_{animation1.Name}_se"))
            {
                Debug.LogError($"Not Find {_prefix}_{animation1.Name}_se");
                return;
            }
        }
    }
}