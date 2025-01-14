using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace pruss.Tool.UI
{
    internal class Button : UIComponent
    {
        [Required] [SerializeField] protected UnityEngine.UI.Button button;

        [Required] [SerializeField] protected RectTransform root;

        [Required] [SerializeField] [AssetList(Path = "/Audio/", AutoPopulate = true)] [ValidateInput("AudioClipValidateInput")]
        private List<AudioClip> clips = new();

        [ValueDropdown("AudioClip")] [ValidateInput("ValidateInput")] [SerializeField]
        private string clickAudioClip;

        [Inject] private AudioManager _audioManager;
        public bool IsShow => button.gameObject.activeSelf;

        public bool Intractable
        {
            get => button.interactable;
            set => button.interactable = value;
        }

        private void Reset()
        {
            button = GetComponent<UnityEngine.UI.Button>();
            root = GetComponent<RectTransform>();
        }

        private void Start()
        {
            button.OnClickAsObservable().Subscribe(_ => { _audioManager.PlayShortSE(clickAudioClip); }).AddTo(root);
        }

        private bool AudioClipValidateInput(List<AudioClip> clips)
        {
            return clips?.Any() ?? false;
        }

        private IEnumerable<string> AudioClip()
        {
            return clips.Select(x => x.name);
        }

        private bool ValidateInput(string value)
        {
            return string.IsNullOrEmpty(value) || clips.Find(x => x.name == value);
        }

        public IDisposable Subscribe(Action onClick)
        {
            return button.OnClickAsObservable().Subscribe(_ => { onClick?.Invoke(); });
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