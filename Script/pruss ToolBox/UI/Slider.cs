using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace pruss.Tool.UI
{
    internal class Slider : UIComponent
    {
        [Required] [SerializeField] private RectTransform _root;
        [Required] [SerializeField] private UnityEngine.UI.Slider _slider;

        public bool IsShow => _slider.gameObject.activeSelf;

        public override Tween Show()
        {
            return _root.Show();
        }

        public override Tween Hide()
        {
            return _root.Hide();
        }
    }
}