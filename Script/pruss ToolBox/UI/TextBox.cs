using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace pruss.Tool.UI
{
    [RequireComponent(typeof(LocalizeStringEvent))]
    internal class TextBox : UIComponent
    {
        [Required] [SerializeField] private RectTransform root;
        [Required] [SerializeField] private TMP_Text text;
        [Required] [SerializeField] private LocalizeStringEvent localizeStringEvent;
        [SerializeField] private bool isTextGraduallyShow;
        private TweenerCore<string, string, StringOptions> _tweenerCore;
        public bool IsShow => root.gameObject.activeSelf;

        private void Awake()
        {
            localizeStringEvent.OnUpdateString.AddListener(ChangeText);
        }

        private void Reset()
        {
            localizeStringEvent = GetComponent<LocalizeStringEvent>();
            localizeStringEvent.StringReference.WaitForCompletion = true;
        }

        public void Clear()
        {
            _tweenerCore?.Kill();
            localizeStringEvent.StringReference = null;
            text.text = string.Empty;
        }

        private void ChangeText(string value)
        {
            if (!isTextGraduallyShow)
            {
                return;
            }

            var temp = "";
            _tweenerCore?.Complete();
            _tweenerCore = DOTween.To(() => temp, x => temp = x, value, value.Length * 0.1f)
                .SetEase(Ease.Linear)
                .OnUpdate(() => text.text = temp);
        }

        public void SetLocalizeString(LocalizedString value)
        {
            localizeStringEvent.StringReference = value;
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