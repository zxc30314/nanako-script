using System;
using Sirenix.OdinInspector;
using UniRx;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Required] public AudioManager audioManager;
    [Required] public UIControl uiControl;
    [Required] public SpineAnimationControl spineAnimationControl;
    [Required] public Process process;
    [Required] public MainProcess mainProcess;
    [Required] public MaskManager maskManager;

    public override void InstallBindings()
    {
        Container.Bind<AudioManager>().FromInstance(audioManager).AsSingle();
        Container.Bind<IUIControl>().To<UIControl>().FromInstance(uiControl).AsSingle();
        Container.Bind<SpineAnimationControl>().FromInstance(spineAnimationControl).AsSingle();
        Container.Bind<UIComponents>().FromInstance(uiControl.UIBaseData).AsSingle();
        Container.Bind<Process>().FromInstance(process).AsSingle();
        Container.Bind<PlayerData>().AsSingle();
        Container.Bind<MainProcess>().FromInstance(mainProcess);
        Container.Bind<MaskManager>().FromInstance(maskManager).AsSingle();
    }
}

public class PlayerData
{
    private int _currentStamina;
    private Action<int> _onValueChange;
    public int MaxStaminaStop => 100;

    public IDisposable Subscribe(Action<int> onValueChange)
    {
        _onValueChange += onValueChange;
        return Disposable.Create(() => _onValueChange -= onValueChange);
    }

    public void AddStamina(int value)
    {
        _currentStamina += value;
        _onValueChange?.Invoke(_currentStamina);
    }

    public int GetStamina()
    {
        return _currentStamina;
    }

    public void ClearStamina()
    {
        _currentStamina = 0;
        _onValueChange?.Invoke(_currentStamina);
    }
}
