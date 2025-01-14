public class MockProcessData : ProcessData
{
    public bool IsInitEnd;
    public override ProcessType ProcessType { get; }

    public override void Init()
    {
        IsInitEnd = true;
    }
}