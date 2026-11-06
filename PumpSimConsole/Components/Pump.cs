public class Pump : Component, IInspectable
{

    public Pump(string name, int hours) : base(name, hours) { }
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