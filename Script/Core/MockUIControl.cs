using UnityEngine;

public class MockUIControl : MonoBehaviour, IUIControl
{
    public UIComponents UIBaseData => null;

    public void ChangeState(ProcessType state, bool fadeIn)
    {
    }
}