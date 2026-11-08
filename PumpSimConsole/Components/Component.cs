public abstract class Component
{
    [ShowInDebug]
    public string Name { get; protected set; }
    [ShowInDebug]
    public int Hours { get; protected set; }
    public abstract bool IsFaulty();
    public virtual string Describe()
    {
        return $"Component: {Name}";
    }
    protected Component(string name, int hours)
    {
        Name = name;
        Hours = hours;
    }
}