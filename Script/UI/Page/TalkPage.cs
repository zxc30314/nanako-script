using pruss.Tool.UI;
using Sirenix.OdinInspector;
using Spine;
using UniRx;
using UnityEngine;
using Zenject;
using Event = Spine.Event;

internal class TalkPage : PageComponent
{
    [Required] [SerializeField] private TextBox textBox;
    [Required] [SerializeField] private Button nextText;
    private readonly CompositeDisposable _disposable = new();
    [Inject] private AudioManager _audioManager;
    [Inject] private Process _process;
    [Inject] private SpineAnimationControl _spineAnimationControl;
    [Inject] private UIComponents _uiComponents;

    private void OnEnable()
    {
        nextText.Subscribe(NextText).AddTo(_disposable);
        _spineAnimationControl.SubscribeAnimationEvent(AnimationEvent).AddTo(_disposable);
    }

    private void OnDisable()
    {
        textBox.Clear();
        _disposable.Clear();
    }

    private void AnimationEvent(TrackEntry t, Event e)
    {
        if (e.Data.Name == "audio")
        {
            _audioManager.PlayVoice(_spineAnimationControl.GetCurrentAnimationName());
        }
    }

    public override void StartProcess()
    {
        _audioManager.StopVoice();
        NextText();
        _spineAnimationControl.SetAnimations(_process.CurrentProcessData.GetAnimations());
    }

    public override void Show()
    {
        RectTransform.gameObject.SetActive(true);
        _uiComponents.Show(ProcessType.Talk);
        textBox.Clear();
    }

    public override void Hide()
    {
        RectTransform.gameObject.SetActive(false);
    }

    private void NextText()
    {
        if (_process.CurrentProcessData is not TalkProcessData p)
        {
            return;
        }

        if (p.GetNextText(out var text))
        {
            textBox.SetLocalizeString(text);
        }
        else
        {
            _process.NextProcess();
        }
    }
}