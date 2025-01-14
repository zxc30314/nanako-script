using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;
using Button = pruss.Tool.UI.Button;

public class InfoAndSettingPanel : MonoBehaviour
{
    [Required] [SerializeField] private Button close;
    [Required] [SerializeField] private Button info;
    [Required] [SerializeField] private Button setting;
    [Required] [SerializeField] private GameObject infoPage;
    [Required] [SerializeField] private GameObject settingPage;
    [Required] [SerializeField] private InfoPageContent infoPageContentPrefab;
    [Required] [SerializeField] private Transform infoPageParent;
    [Required] [SerializeField] private List<InfoPageContentData> infoPageContentList;
    [Inject] private AudioManager _audioManager;

    // Start is called before the first frame update
    private void Start()
    {
        info.Subscribe(SwitchInfoPage).AddTo(this);
        setting.Subscribe(SwitchSettingPage).AddTo(this);
        close.Subscribe(Close).AddTo(this);

        foreach (var infoPageContentData in infoPageContentList)
        {
            var infoPageContent = Instantiate(infoPageContentPrefab, infoPageParent);
            infoPageContent.SetValue(infoPageContentData.sprite, infoPageContentData.localizedKey);
        }
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void SwitchInfoPage()
    {
        infoPage.SetActive(true);
        settingPage.SetActive(false);
    }

    private void SwitchSettingPage()
    {
        infoPage.SetActive(false);
        settingPage.SetActive(true);
    }

    [Serializable]
    private class InfoPageContentData
    {
        [Required] [SerializeField] public Sprite sprite;

        [ValidateInput("ValidateInput")] [SerializeField]
        public LocalizedString localizedKey;

        private bool ValidateInput(LocalizedString localizedKey)
        {
            return !localizedKey.IsEmpty;
        }
    }
}