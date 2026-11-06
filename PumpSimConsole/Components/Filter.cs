public class Filter : Component, IInspectable, IRepairable
{

    public Filter(string name, int hours) : base(name, hours) { }
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

    public string Repair()
    {
        Hours = 0;
        return "Filter has been repaired";
    }

}
