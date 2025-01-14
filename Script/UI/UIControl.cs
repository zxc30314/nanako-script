using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using pruss.Tool.UI;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

public class UIControl : MonoBehaviour, IUIControl
{
    [Required] [SerializeField] private UIComponents uiBaseData;
    [Required] [SerializeField] private Button infoButton;
    [Required] [SerializeField] private InfoAndSettingPanel infoPanel;
    [Required] [SerializeField] private PageComponent title;
    [Required] [SerializeField] private PageComponent talk;
    [Required] [SerializeField] private PageComponent makeLove;
    [Required] [SerializeField] private PageComponent cum;
    [Required] [SerializeField] private PageComponent phoneChat;
    private List<PageComponent> _allUI;
    [Inject] private AudioManager _audioManager;
    [Inject] private MaskManager _maskManager;

    private void Awake()
    {
        HideInfoPage();
        _allUI = FindObjectsByType<PageComponent>(FindObjectsSortMode.InstanceID).ToList();
        infoButton.Subscribe(ShowInfoPage).AddTo(this);
    }

    public UIComponents UIBaseData => uiBaseData;

    [Button]
    public void ChangeState(ProcessType state, bool fadeIn)
    {
        var intersectedList = state switch
        {
            ProcessType.Title => _allUI.Find(x => x == title),
            ProcessType.Talk => _allUI.Find(x => x == talk),
            ProcessType.MakeLove => _allUI.Find(x => x == makeLove),
            ProcessType.Cum => _allUI.Find(x => x == cum),
            ProcessType.PhoneChat => _allUI.Find(x => x == phoneChat),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

        ShowViewOnly(intersectedList, fadeIn);
    }

    private void ShowInfoPage()
    {
        infoPanel.gameObject.SetActive(true);
    }

    private void HideInfoPage()
    {
        infoPanel.gameObject.SetActive(false);
    }

    private void ShowViewOnly(PageComponent intersectedList, bool fadeIn)
    {
        if (fadeIn)
        {
            _maskManager.DoMask(ShowAndHide, StartProcess);
        }
        else
        {
            ShowAndHide();
            StartProcess();
        }

        return;

        void ShowAndHide()
        {
            foreach (var uiComponent in _allUI)
            {
                if (uiComponent == intersectedList)
                {
                    uiComponent.Show();
                }
                else
                {
                    uiComponent.Hide();
                }
            }
        }

        void StartProcess()
        {
            intersectedList.StartProcess();
        }
    }
}

public enum ProcessType
{
    Title,
    Talk,
    MakeLove,
    Cum,
    PhoneChat
}

[Serializable]
public class UIComponents
{
    [Required] [SerializeField] private List<UIComponent> allUI;
    [Required] [SerializeField] private List<UIComponent> title;
    [Required] [SerializeField] private List<UIComponent> talk;
    [Required] [SerializeField] private List<UIComponent> makeLove;
    [Required] [SerializeField] private List<UIComponent> cum;
    [Required] [SerializeField] private List<UIComponent> phoneChat;

    public Tween Show(ProcessType state)
    {
        var target = state switch
        {
            ProcessType.Title => title,
            ProcessType.Talk => talk,
            ProcessType.MakeLove => makeLove,
            ProcessType.Cum => cum,
            ProcessType.PhoneChat => phoneChat,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

        var sequence = DOTween.Sequence();
        foreach (var uiComponent in allUI)
        {
            if (target.Find(x => x == uiComponent))
            {
                sequence.Insert(0, uiComponent.Show());
            }
            else
            {
                uiComponent.Hide();
            }
        }

        return sequence.Play();
    }

    public Tween Hide(ProcessType state)
    {
        var target = state switch
        {
            ProcessType.Title => title,
            ProcessType.Talk => talk,
            ProcessType.MakeLove => makeLove,
            ProcessType.Cum => cum,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
        var sequence = DOTween.Sequence();
        foreach (var uiComponent in allUI)
        {
            if (target.Find(x => x == uiComponent))
            {
                sequence.Insert(0, uiComponent.Hide());
            }
        }

        return sequence.Play();
    }
}

public interface IUIControl
{
    UIComponents UIBaseData { get; }
    void ChangeState(ProcessType state, bool fadeIn);
}

public abstract class PageComponent : MonoBehaviour
{
    private RectTransform _rectTransform;

    protected RectTransform RectTransform
    {
        get
        {
            if (!_rectTransform)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }


    public bool IsShow => RectTransform.gameObject.activeSelf;
    public abstract void StartProcess();
    public abstract void Show();
    public abstract void Hide();
}