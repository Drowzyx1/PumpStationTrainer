public class Program
{
    public static void Main(string[] args)
    {
        string name = "Pipe";
        int hours = 40;
        float PSI = 15.5f;

        Valve valve1 = new Valve("Valve 1");
        Valve valve2 = new Valve("Valve 2");

        bool IsSafeToOpen(float PSI)
        {
            return PSI <= 0;
        }

        valve1.Close();

        if (hours > 200) { Console.WriteLine("Replace Filter"); }
        else { Console.WriteLine("Filter OK"); }

        List<int> pressureReadings = new List<int> { 10, 12, 15, 20, 25 };
        int highestPressure = pressureReadings[0];
        foreach (int curr in pressureReadings)
        {
            if (curr > highestPressure)
            {
                highestPressure = curr;
            }
            Console.WriteLine(curr);
        }

        Console.WriteLine(valve1.IsOpen);
        Console.WriteLine(valve2.IsOpen);
        Console.WriteLine($"Highest Pressure: {highestPressure}");
        Console.WriteLine($"Is it safe to open? {IsSafeToOpen(0)}");
        Console.WriteLine($"Is it safe to open? {IsSafeToOpen(1500)}");
        Console.WriteLine($"{name}, {hours}, {PSI}");
    }
}