public class ValveLogger
{
    public ValveLogger(Valve supply)
    {
        supply.OnToggled += HandleValveToggled;
    }

    private void HandleValveToggled(bool isOpen)
    {
        Console.WriteLine($"Log: Valve is {(isOpen ? "open" : "closed")}");
    }
}