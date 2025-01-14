using pruss.Tool.UI;
using Sirenix.OdinInspector;
using Spine;
using UniRx;
using UnityEngine;
using Zenject;
using Event = Spine.Event;

internal class MakeLovePage : PageComponent
{
    [Required] [SerializeField] private Button end;
    [Required] [SerializeField] private Button nextProcess;
    [Required] [SerializeField] private Button switchAnimation;
    [Required] [SerializeField] private Button switchSkin;
    [Required] [SerializeField] private Bar stamina;
    [Inject] private AudioManager _audioManager;
    [Inject] private PlayerData _playerData;
    [Inject] private Process _process;
    [Inject] private SpineAnimationControl _spineAnimationControl;
    [Inject] private UIComponents _uiComponents;
    private readonly CompositeDisposable _disposable = new();

    public void OnEnable()
    {
        _spineAnimationControl.SubscribeAnimationEvent(AnimationEvent).AddTo(_disposable);
        _playerData.Subscribe(SettStaminaBarValue).AddTo(_disposable);
        switchAnimation.Subscribe(_spineAnimationControl.SwitchAnimation).AddTo(_disposable);
        switchSkin.Subscribe(_spineAnimationControl.SwitchSkin).AddTo(_disposable);
        nextProcess.Subscribe(NextProcess).AddTo(_disposable);
        end.Subscribe(NextProcess).AddTo(_disposable);
        stamina.Subscribe(StaminaBarOnAnimationEnd).AddTo(_disposable);
    }

    public void OnDisable()
    {
        _disposable.Clear();
    }

    private void NextProcess()
    {
        _process.NextProcess();
    }

    private bool ParseProcessData(out MakeLoveProcessData processData)
    {
        processData = null;
        if (_process.CurrentProcessData is MakeLoveProcessData p)
        {
            processData = p;
        }

        return processData is not null;
    }

    private void StaminaBarOnAnimationEnd(float value)
    {
        if (ParseProcessData(out var p))
        {
            var staminaValue = (float)p.StaminaStepNextUnlock / _playerData.MaxStaminaStop;
            nextProcess.Intractable = value >= staminaValue;
            end.Intractable = value >= staminaValue;
        }
    }

    private void AnimationEvent(TrackEntry trackEntry, Event e)
    {
        if (e.Data.Name is "BarStamina")
        {
            if (!ParseProcessData(out var p))
            {
                return;
            }

            if (_playerData.GetStamina() < p.StaminaStepNextUnlock)
            {
                _playerData.AddStamina(p.StaminaOnTimeAddStep);
            }
        }

        if (e.Data.Name is "audio")
        {
            _audioManager.PlayVoice(_spineAnimationControl.GetCurrentAnimationName());
        }
    }

    private void SettStaminaBarValue(int value)
    {
        stamina.SetValue((float)value / _playerData.MaxStaminaStop);
    }

    public override void StartProcess()
    {
        _audioManager.StopVoice();
        _spineAnimationControl.SetAnimations(_process.CurrentProcessData.GetAnimations());
        SettStaminaBarValue(_playerData.GetStamina());
    }

    public override void Show()
    {
        RectTransform.gameObject.SetActive(true);
        _uiComponents.Show(ProcessType.MakeLove);

        if (_process.NextProcessData?.ProcessType == ProcessType.Cum && _process.NextProcessData is not null)
        {
            end.Show();
            nextProcess.Hide();
        }
        else
        {
            nextProcess.Show();
            end.Hide();
        }
    }

    public override void Hide()
    {
        RectTransform.gameObject.SetActive(false);
        if (_process.CurrentProcessData?.ProcessType == ProcessType.Cum)
        {
            _playerData.ClearStamina();
            SettStaminaBarValue(_playerData.GetStamina());
        }
    }
}