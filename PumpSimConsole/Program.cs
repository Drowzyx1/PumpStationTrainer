using System;
using System.Collections.Generic;

List<Component> parts = new List<Component>
{
    new Pump("Main Pump", 1200),
    new Valve("Supply Valve", 300),
    new Filter("Line Filter", 250),
};

Reports.PrintSummary(parts);

foreach (Component c in parts)
{
    Console.WriteLine($"{c.Describe()} | Faulty: {c.IsFaulty()}");

    if (c is IInspectable inspectable)
    {
        Console.WriteLine($"    Inspect: {inspectable.Inspect()}");
    }

    if (c is IRepairable repairable)
    {
        Console.WriteLine($"    Repair: {repairable.Repair()}");
    }

    Valve supply = (Valve)parts[1];
    PressureSystem pressure = new PressureSystem(supply);
    ValveLogger logger = new ValveLogger(supply);

    supply.Toggle();
    supply.Toggle();
}