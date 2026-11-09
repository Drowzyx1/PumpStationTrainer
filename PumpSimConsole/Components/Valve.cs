public class Valve : Component, IRepairable
{
    public event Action<bool>? OnToggled;
    [ShowInDebug]
    public bool IsOpen { get; private set; } = true;

    public Valve(string name, int hours) : base(name, hours) { }
    public override bool IsFaulty()
    {
        return Hours > 500;
    }

    public string Repair()
    {
        Hours = 0;
        return "Valve repaired";
    }

    public override string Describe()
    {
        return $"Valve: {Name}, Hours: {Hours}";
    }

    public void Toggle()
    {
        IsOpen = !IsOpen;
        OnToggled?.Invoke(IsOpen);
    }

}
