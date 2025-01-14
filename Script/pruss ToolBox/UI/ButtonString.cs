using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace pruss.Tool.UI
{
    [RequireComponent(typeof(LocalizeStringEvent))]
    internal class ButtonString : Button
    {
        [Required] [SerializeField] private TMP_Text text;
        [Required] [SerializeField] private LocalizeStringEvent localize;

        public bool IsShow => button.gameObject.activeSelf;

        public bool Interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }

        public void Awake()
        {
            localize.OnUpdateString.AddListener(UpdateString);
        }

        private void Reset()
        {
            button = GetComponent<UnityEngine.UI.Button>();
            root = GetComponent<RectTransform>();
            text = GetComponentInChildren<TMP_Text>();
            localize = GetComponent<LocalizeStringEvent>();
        }

        private void UpdateString(string value)
        {
            text.text = value;
        }

        public void Subscribe(Action onClick)
        {
            button.OnClickAsObservable().Subscribe(_ => onClick?.Invoke()).AddTo(root);
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