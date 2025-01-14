using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace pruss.Tool.UI
{
    [RequireComponent(typeof(LocalizeStringEvent))]
    internal class LocalizationSettingControl : UIComponent
    {
        [SerializeField] private UnityEngine.UI.Button next;
        [SerializeField] private UnityEngine.UI.Button last;

        [Required] [SerializeField] private TMP_Text text;
        [Required] [SerializeField] private RectTransform root;
        [Required] [SerializeField] private LocalizeStringEvent localize;

        private bool active;
        private int currentLocalesIndex;
        private int localesCount;

        public void Awake()
        {
            localesCount = LocalizationSettings.AvailableLocales.Locales.Count;
            localize.OnUpdateString.AddListener(UpdateString);
            next.onClick.AddListener(() => ChangeLocal(++currentLocalesIndex));
            last.onClick.AddListener(() => ChangeLocal(--currentLocalesIndex));
        }

        private void Reset()
        {
            root = GetComponent<RectTransform>();
            text = GetComponentInChildren<TMP_Text>();
            localize = GetComponent<LocalizeStringEvent>();
        }

        private void ChangeLocal(int localIndex)
        {
            currentLocalesIndex = (localIndex + localesCount) % localesCount;
            if (active)
            {
                return;
            }

            StartCoroutine(SetLocal(currentLocalesIndex));
        }

        private IEnumerator SetLocal(int localIndex)
        {
            active = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localIndex];
            active = false;
        }

        private void UpdateString(string value)
        {
            text.text = value;
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