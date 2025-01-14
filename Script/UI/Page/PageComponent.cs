using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace pruss.Tool.UI
{
    // [RequireComponent(typeof(RectTransform))]
    // internal class PageComponent : MonoBehaviour
    // {
    //     [Required]
    //     [SerializeReference]
    //     private IPageComponent _pageComponent;
    //
    //     private void Awake() => _pageComponent.Awake();
    //     private void OnEnable() => _pageComponent.OnEnable();
    //     private void OnDisable() => _pageComponent.OnDisable();
    //     private void Update() => _pageComponent.Update();
    //     public void Init(Process process, UIManagerComponents uiManagerComponents) => _pageComponent.Init( GetComponent<RectTransform>());
    //
    //     public void Show() => _pageComponent.Show();
    //     public void Hide() => _pageComponent.Hide();
    // }

    internal static class Tool
    {
        public static TweenerCore<Vector3, Vector3, VectorOptions> Show(this RectTransform root)
        {
            return root.DOScale(Vector3.one, .5f).SetEase(Ease.OutCirc).OnStart(() => root.gameObject.SetActive(true));
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> Hide(this RectTransform root)
        {
            return root.DOScale(Vector3.zero, .5f).SetEase(Ease.OutCirc).OnComplete(() => root.gameObject.SetActive(false));
        }
    }
}