using System.ComponentModel;
using System.Linq;

public static class Reports
{
    public static void PrintSummary(List<Component> components)
    {
        var faulty = components.Where(c => c.IsFaulty()).Select(c => c.Name);
        int hours = components.Sum(c => c.Hours);
        var sorted = components.OrderByDescending(c => c.Hours);

        Console.WriteLine($"Faulty components: {string.Join(", ", faulty)}");
        Console.WriteLine($"Total hours: {hours}");
        foreach (var c in sorted) Console.WriteLine($"   {c.Name}: {c.Hours}");
    }
}