using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup image;

    private void Awake()
    {
        image.alpha = 1;
    }

    [Button]
    public void DoMask(TweenCallback onShowEnd, TweenCallback onHideEnd)
    {
        image.blocksRaycasts = true;
        DOTween.Sequence()
            .Append(image.DOFade(1, 0.5f))
            .AppendCallback(onShowEnd)
            .AppendInterval(1f)
            .Append(image.DOFade(0, 0.5f))
            .AppendCallback(() =>
            {
                onHideEnd?.Invoke();
                image.blocksRaycasts = false;
            });
    }

    [Button]
    public void Flash(TweenCallback onEnd)
    {
        image.blocksRaycasts = true;
        DOTween.Sequence()
            .Append(image.DOFade(1, 0.3f))
            .Append(image.DOFade(0, 0.3f))
            .OnComplete(() =>
            {
                onEnd?.Invoke();
                image.blocksRaycasts = false;
            }).SetLoops(3);
    }
}