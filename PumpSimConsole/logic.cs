
public abstract class Component
{
    public string Name { get; protected set; }
    public int Hours { get; protected set; }
    public abstract bool IsFaulty();
    public virtual string Describe()
    {
        return $"Component: {Name}";
    }
}

public interface IInspectable { string Inspect(); }
public interface IRepairable { string Repair(); }

public class Pump : Component, IInspectable
{
    public override bool IsFaulty()
    {
        return Hours > 1000;
    }

    public string Inspect()
    {
        return IsFaulty() ? "Pump is faulty" : "Pump is OK";
    }
    public override string Describe() => base.Describe() + $", Hours: {Hours}";
}

public class Valve : Component, IRepairable
{
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
}

public class Filter : Component, IInspectable
{
    public override bool IsFaulty()
    {
        return Hours > 200;
    }

    public string Inspect()
    {
        return IsFaulty() ? "Filter is faulty" : "Filter is OK";
    }

    public override string Describe()
    {
        return $"Filter: {Name}, Hours: {Hours}";
    }
}
