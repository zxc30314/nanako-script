using pruss.Tool.UI;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

internal class TitlePage : PageComponent
{
    [Required] [SerializeField] private Button startGame;
    private readonly CompositeDisposable _disposable = new();
    [Inject] private AudioManager _audioManager;
    [Inject] private Process _process;
    [Inject] private SpineAnimationControl _spineAnimationControl;
    [Inject] private UIComponents _uiComponents;

    public override void StartProcess()
    {
        _audioManager.PlayBGM();
    }

    public override void Show()
    {
        RectTransform.gameObject.SetActive(true);
        startGame.Subscribe(_process.NextProcess).AddTo(_disposable);
        _uiComponents.Show(ProcessType.Title);
        _audioManager.StopVoice();
        _spineAnimationControl.SetAnimations(_process.CurrentProcessData.GetAnimations());
    }

    public override void Hide()
    {
        RectTransform.gameObject.SetActive(false);
        _disposable.Clear();
    }
}