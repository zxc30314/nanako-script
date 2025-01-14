using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace pruss.Tool.UI
{
    internal class AudioUIControl : UIComponent
    {
        [Required] [SerializeField] private RectTransform root;
        [Required] [SerializeField] private UnityEngine.UI.Slider slider;
        [ValidateInput("ValidateInput")] [ValueDropdown("GetExposed", ExpandAllMenuItems = true)] [SerializeField] private string exposed;
        [Inject] private AudioManager _audioManager;

        private bool _isShow;
        private RectTransform _rectTransform;
        public bool IsShow => root.gameObject.activeSelf;

        private void Awake()
        {
            slider.onValueChanged.AddListener(SliderValueChanged);
        }

        [Button]
        public override Tween Show()
        {
            return root.Show();
        }

        [Button]
        public override Tween Hide()
        {
            return root.Hide();
        }

        private bool ValidateInput(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        private IEnumerable<string> GetExposed()
        {
            var mixer = Resources.Load("Audio") as AudioMixer;
            var parameters = (Array)mixer.GetType().GetProperty("exposedParameters").GetValue(mixer, null);

            return parameters.Cast<object>()
                .Select((t, i) => parameters.GetValue(i))
                .Select(o => (string)o.GetType().GetField("name").GetValue(o))
                .ToList();
        }

        private void SliderValueChanged(float value)
        {
            _audioManager.SetVolume(exposed, value);
        }
    }
}