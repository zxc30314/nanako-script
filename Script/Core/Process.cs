using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public class Process : MonoBehaviour
{
    private readonly Queue<ProcessData> _runtimeProcessData = new();
    private Action<string> _currentProcess;
    private int _processIndex;
    [Inject] private MainProcess _mainProcess;
    [Inject] private IUIControl _uiControl;
    public ProcessData CurrentProcessData { get; private set; }
    public ProcessData NextProcessData { get; private set; }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        foreach (var data in _mainProcess.processData)
        {
            _runtimeProcessData.Enqueue(data);
            data.Init();
        }

        NextProcess();
    }

    public IDisposable SubscribeOnNextProcess(Action<string> currentProcess)
    {
        _currentProcess += currentProcess;
        return Disposable.Create(() => _currentProcess -= currentProcess);
    }

    public void NextProcess()
    {
        if (!NextProcessCheck())
        {
            Debug.Log("End");
            Init();
        }
    }

    private bool NextProcessCheck()
    {
        var hasValue = _runtimeProcessData.Any();
        if (hasValue)
        {
            var data = _runtimeProcessData.Dequeue();
            CurrentProcessData = data;
            NextProcessData = _runtimeProcessData.Any() ? _runtimeProcessData.Peek() : null;
            _uiControl.ChangeState(data.ProcessType, CurrentProcessData.FadeIn);
            _currentProcess?.Invoke(CurrentProcessData.ProcessType.ToString());
        }

        return hasValue;
    }
}