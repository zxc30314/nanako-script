using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace pruss.Tool.UI
{
    internal class Bar : UIComponent
    {
        [Required] [SerializeField] private Image bar;
        [Required] [SerializeField] private RectTransform root;

        private Action<float> _onValueChange;
        public bool IsShow => root.gameObject.activeSelf;

        public void Awake()
        {
            bar.fillAmount = 0;
        }

        public void SetValue(float value)
        {
            DOTween.To(() => bar.fillAmount, x => bar.fillAmount = x, value, .5f);
            _onValueChange?.Invoke(value);
        }

        public IDisposable Subscribe(Action<float> onValueChange)
        {
            _onValueChange += onValueChange;
            return Disposable.Create(() => _onValueChange -= onValueChange);
        }

        public override Tween Show()
        {
            return root.Show();
        }

        public override Tween Hide()
        {
            return root.Hide();
        }
    }
}