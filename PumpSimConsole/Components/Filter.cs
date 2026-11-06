public class Filter : Component, IInspectable
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

}
