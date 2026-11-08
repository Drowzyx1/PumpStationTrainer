public class PressureSystem
{
    public float Psi { get; private set; } = 3000f;

    public PressureSystem(Valve supply)
    {
        supply.OnToggled += HandleValveToggled;
    }

    private void HandleValveToggled(bool isOpen)
    {
        Psi = isOpen ? 3000f : 0f;
        Console.WriteLine($"PSI is now {Psi} \nBar is now {Units.PsiToBar(Psi):0.0}");
    }
}