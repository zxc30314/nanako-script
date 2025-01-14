using Sirenix.OdinInspector;
using Spine;
using UniRx;
using UnityEngine;
using Zenject;
using Event = Spine.Event;

internal class CumPage : PageComponent
{
    private readonly CompositeDisposable _disposable = new();
    [Inject] private AudioManager _audioManager;
    [Inject] private Process _process;
    [Inject] private SpineAnimationControl _spineAnimationControl;
    [Inject] private UIComponents _uiComponents;

    private void OnEnable()
    {
        _spineAnimationControl.SubscribeAnimationEvent(AnimationEvent).AddTo(_disposable);
    }

    private void OnDisable()
    {
        _disposable.Clear();
    }

    private void NextProcess(TrackEntry trackEntry)
    {
        _process.NextProcess();
    }

    private void AnimationEvent(TrackEntry t, Event e)
    {
        if (e.Data.Name == "audio")
        {
            _audioManager.PlayVoice(_spineAnimationControl.GetCurrentAnimationName());
        }
    }

    [Button]
    private void Test()
    {
        var processName = _process.name;
        Debug.Log(processName);
    }

    public override void StartProcess()
    {
        _audioManager.StopVoice();
        _spineAnimationControl.SetAnimations(_process.CurrentProcessData.GetAnimations(), NextProcess);
    }

    public override void Show()
    {
        RectTransform.gameObject.SetActive(true);
        _uiComponents.Show(ProcessType.Cum);
    }

    public override void Hide()
    {
        RectTransform.gameObject.SetActive(false);
    }
}