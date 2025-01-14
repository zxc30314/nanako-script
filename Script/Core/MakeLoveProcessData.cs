using UnityEngine;

internal class MakeLoveProcessData : ProcessData
{
    [Range(0, 100)] [SerializeField] private int _staminaOnTimeAddStep;

    [Range(0, 100)] [SerializeField] private int _staminaStepNextUnlock;

    public int StaminaOnTimeAddStep => _staminaOnTimeAddStep;
    public int StaminaStepNextUnlock => _staminaStepNextUnlock;
    public override ProcessType ProcessType => ProcessType.MakeLove;

    public override void Init()
    {
    }
}