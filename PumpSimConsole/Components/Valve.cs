public class Valve : Component, IRepairable
{

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

}
